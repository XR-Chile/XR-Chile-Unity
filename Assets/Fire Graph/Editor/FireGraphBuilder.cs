using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Watona.Variables;
public class FireGraphBuilder : EditorWindow
{
    private FloatVariable connectionThreshold; // Distance away from other nodes
    private static FireGraphBuilder current;

    [MenuItem("Tools/Build Fire Graph")]
    public static void ShowWindow()
    {
        current = GetWindow<FireGraphBuilder>("Fire Graph Builder");
        SceneView.duringSceneGui -= OnSceneGUIStatic;
        SceneView.duringSceneGui += OnSceneGUIStatic;
    }

    /*
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUIStatic;
    }
    */

    private void OnGUI()
    {
        connectionThreshold = (FloatVariable)EditorGUILayout.ObjectField(
            "Max Connection Distance", 
            connectionThreshold, 
            typeof(FloatVariable), 
            false);

        if (GUILayout.Button("Generate Fire Graph"))
        {
            GenerateGraph();
            SceneView.RepaintAll(); // Immediately reflect changes
        }

        if (GUILayout.Button("Refresh Visualization"))
        {
            SceneView.RepaintAll();
        }
    }

    private static void OnSceneGUIStatic(SceneView view)
    {
        TextAsset json = Resources.Load<TextAsset>("fire_graph");
        if (json == null) return;

        FireGraph graph = JsonUtility.FromJson<FireGraph>(json.text);
        Dictionary<string, FireGraphNode> node_lookup = new Dictionary<string, FireGraphNode>();

        foreach (var node in graph.nodes)
        {
            node_lookup[node.id] = node;
        }

        Handles.color = Color.yellow;

        foreach (var node in graph.nodes)
        {
            Handles.SphereHandleCap(0, node.position, Quaternion.identity, 0.3f, EventType.Repaint);
            foreach (var edge in node.edges)
            {
                if (node_lookup.TryGetValue(edge.targetId, out FireGraphNode n_node))
                {
                    Handles.DrawLine(node.position, n_node.position);
                }
            }
        }
    }

    void GenerateGraph()
    {
        FireObject[] fire_objects = GameObject.FindObjectsOfType<FireObject>();
        FireGraph graph = new FireGraph();
        Dictionary<string, FireGraphNode> node_map = new Dictionary<string, FireGraphNode>();

        foreach (FireObject fireobject in fire_objects)
        {
            FireGraphNode node = new FireGraphNode(fireobject.gameObject.name, fireobject.transform.position);
            node_map[node.id] = node;
            graph.nodes.Add(node);
        }

        foreach (var a in graph.nodes)
        {
            foreach (var b in graph.nodes)
            {
                if (a == b) continue;

                float dist = Vector3.Distance(a.position, b.position);
                if (dist <= connectionThreshold.Value)
                {
                    a.edges.Add(new FireGraphEdge(b.id, dist));
                }
            }
        }

        string json = JsonUtility.ToJson(graph, true);
        string path = Application.dataPath + "/Resources/fire_graph.json";
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
        Debug.Log("Fire graph has been saved to /Resources/fire_graph.json!");
    }
}