using Oculus.Interaction.HandGrab;
using UnityEngine;

public class Extinguisher : MonoBehaviour, IHandGrabUseDelegate
{
    [Header("Settings")]
    [SerializeField] private FireType _type;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private FoamProjectile _foamPrefab;
    [SerializeField] private float _foamSpeed = 10f;
    [SerializeField] private float _fireRate = 0.1f;
    [SerializeField] private int _maxCapacity = 100;

    private ObjectPool<FoamProjectile> _pool;
    private float _fireTimer;
    private int _currentFoam;
    [SerializeField] private bool _isExtinguishing;
    public float GetFoamPercent() => Mathf.Clamp01((float)_currentFoam / _maxCapacity);

    void Awake()
    {
        _pool = new ObjectPool<FoamProjectile>(_foamPrefab, _maxCapacity, transform);
        _currentFoam = _maxCapacity;
    }

    void Update()
    {
        if (!_isExtinguishing || _currentFoam <= 0) return;

        _fireTimer += Time.deltaTime;
        if (_fireTimer >= _fireRate)
        {
            _fireTimer = 0f;
            FireFoam();
        }
    }

    private void FireFoam()
    {
        FoamProjectile foam = _pool.GetFromPool();
        foam.transform.position = _firePoint.position;
        foam.transform.rotation = _firePoint.rotation;
        foam.Initialize(_type, _pool);
        foam.Fire(_firePoint.forward, _foamSpeed);

        _currentFoam--;
        if (_currentFoam <= 0)
        {
            Debug.LogWarning($"Extinguisher ran out of foam");
        }
    }

    public void StartExtinguishing() => _isExtinguishing = true;
    public void StopExtinguishing() => _isExtinguishing = false;

    public void BeginUse() => _isExtinguishing = true;

    public void EndUse() => _isExtinguishing = false;

    public float ComputeUseStrength(float strength)
    {
        throw new System.NotImplementedException();
    }

}
