using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class Builder : MonoBehaviour
{
    private Transform _floor;
    private List<Transform> _walls;
    private List<Transform> _pillars;
    [SerializeField] private float _wallHeight, _wallThickness;
    [SerializeField] private int _desiredPillars = 4;
    private void Awake()
    {
        _floor = transform;
        _walls = new List<Transform>();
        _pillars = new List<Transform>();
    }
    private void Update()
    {
        if(_walls.Count != _floor.childCount) UpdateWalls();
        if(_pillars.Count < _desiredPillars) UpdatePillars();
    }
    void UpdateWalls()
    {
        foreach(Transform wall in _floor)
        {
            if(!_walls.Contains(wall)) _walls.Add(wall);
        }
    }
    void UpdatePillars()
    {
        int neededPillars = _desiredPillars - _pillars.Count;
        for (int i = 0; i < neededPillars; i++)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SetPillarScale(pillar.transform);
            SetPillarPosition(pillar.transform, i);
            pillar.name = "Pillar";
            pillar.transform.parent = _floor;
            _pillars.Add(pillar.transform);
        }
    }
    void SetPillarScale(Transform pillar)
    {
        Vector3 desiredScale = new Vector3(_wallThickness, _wallHeight, _wallThickness);
        pillar.localScale = desiredScale;
    }
    void SetPillarPosition(Transform pillar, int index)
    {
        float xPosition;
        float zPosition;
        switch (index)
        {
            case 0: 
                xPosition = -5;
                zPosition = 5;
            break;
            case 1:
                xPosition = 5;
                zPosition = 5;
            break;
            case 2:
                xPosition = 5;
                zPosition = -5;
            break;
            case 3:
                xPosition = -5;
                zPosition = -5;
            break;
            default:
                xPosition = 0;
                zPosition = 0;
                Debug.LogError("Pillar index out of range");
            break;
        }
        Vector3 desiredPosition = new Vector3(xPosition, pillar.localScale.y/2, zPosition);
    }
    [ContextMenu("CheckHeight")]
    void CheckWallProperties()
    {
        foreach(Transform wall in _walls)
        {
            UpdateScale(wall);
            UpdateYPosition(wall);
        }
    }
    void UpdateYPosition(Transform wall)
    {
        Vector3 desiredPosition = wall.position;
        desiredPosition.y = wall.localScale.y/2;
        wall.position = desiredPosition;
    }
    void UpdateScale(Transform wall)
    {
        Vector3 desiredScale = wall.localScale;
        desiredScale.y = _wallHeight;
        desiredScale.x = _wallThickness;
        wall.localScale = desiredScale;
    }
}
