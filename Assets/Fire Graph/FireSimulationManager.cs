using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FireSimulationManager : MonoBehaviour
{
    public string fireGraphFileName = "fire_graph";
    public string startingNodeId;

    public GameObject prefabExplosionVisual;

    private Dictionary<string, FireObject> fireObjects = new Dictionary<string, FireObject>();
    private Dictionary<string, FireGraphNode> fireGraph = new Dictionary<string, FireGraphNode>();

    private HashSet<string> burnt = new HashSet<string>();
    private List<string> burning = new List<string>();
    private List<string> pendingIgnitions = new List<string>();

    void Start()
    {
        LoadFireGraph();
        CacheFireObjects();

        if (fireObjects.ContainsKey(startingNodeId))
        {
            FireObject start = fireObjects[startingNodeId];
            start.Ignite();
            burning.Add(startingNodeId);
        }
    }

    void Ignite(string nodeId)
    {
        if (!fireObjects.ContainsKey(nodeId) || burnt.Contains(nodeId) || burning.Contains(nodeId))
            return;

        fireObjects[nodeId].Ignite();
        pendingIgnitions.Add(nodeId);
    }

    void Update()
    {
        if (burning.Count == 0)
            return;

        //List of old objects that are still burning after our loop
        List<string> existingBurning = new List<string>();

        //Update all burning objects
        foreach (string id in burning)
        {
            if (!fireObjects.ContainsKey(id) || !fireGraph.ContainsKey(id)) continue;

            //Store references to make code easier
            FireObject currentFireObj = fireObjects[id];
            FireGraphNode currentNode = fireGraph[id];

            currentFireObj.BurnUpdate();
            if(currentFireObj.is_burnt)
            {
                burnt.Add(id);
                if (currentFireObj.explosion_radius > 0f)
                {
                    PropagateExplosion(id, currentFireObj.explosion_radius);
                    SpawnExplosionVisual(currentFireObj.transform.position, currentFireObj.explosion_radius);
                }
            }
            else
            {
                existingBurning.Add(id);
            }

            foreach (FireGraphEdge edge in currentNode.edges)
            {
                string targetId = edge.targetId;
                //Ignore if this has been burnt, is burning, or will burn in the next iteration
                if (burnt.Contains(targetId) || burning.Contains(targetId) || existingBurning.Contains(targetId))
                    continue;

                //Somehow we've stumbed on an object that doesn't exist.
                if (!fireObjects.ContainsKey(targetId)) continue;

                FireObject neighbor = fireObjects[targetId];
                //Are we above the combustibility threshold to set out neigbor on fire?
                if (currentFireObj.GetPercentCombusted() >= neighbor.combustibility)
                {
                    Ignite(targetId);
                }
            }
        }

        //Update what is still burnign after an update with new ignitions. 
        burning = existingBurning;
        burning.AddRange(pendingIgnitions);
        pendingIgnitions.Clear();

        if (burning.Count == 0)
        {
            Debug.Log("Fire simulation complete.");
        }
    }

        void LoadFireGraph()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(fireGraphFileName);
        if (jsonFile == null)
        {
            Debug.LogError("Could not find fire_graph.json in Resources");
            return;
        }

        FireGraph graph = JsonUtility.FromJson<FireGraph>(jsonFile.text);
        foreach (var node in graph.nodes)
        {
            fireGraph[node.id] = node;
        }
    }

    void PropagateExplosion(string originId, float radius)
    {
        if (!fireGraph.ContainsKey(originId)) return;

        Queue<(string nodeId, float totalDist)> queue = new();
        HashSet<string> visited = new();

        queue.Enqueue((originId, 0f));
        visited.Add(originId);

        while (queue.Count > 0)
        {
            var (currentId, accumulated) = queue.Dequeue();

            foreach (var edge in fireGraph[currentId].edges)
            {
                string neighborId = edge.targetId;
                float newDist = accumulated + edge.distance;

                if (newDist > radius || visited.Contains(neighborId))
                    continue;

                visited.Add(neighborId);
                Ignite(neighborId);  // Optional: delay ignite until all collected?
                queue.Enqueue((neighborId, newDist));
            }
        }
    }

    void CacheFireObjects()
    {
        FireObject[] fire_objects = GameObject.FindObjectsByType<FireObject>(FindObjectsSortMode.None);
        foreach (var o in fire_objects)
        {
            fireObjects[o.gameObject.name] = o;
            o.burn_timer = 0f; // Reset burn timer
        }
    }
    void SpawnExplosionVisual(Vector3 position, float radius)
    {
        GameObject visual = Instantiate(prefabExplosionVisual, position, Quaternion.identity);
        ExplosionVisual ev = visual.AddComponent<ExplosionVisual>();
        ev.radius = radius;
    }

}
