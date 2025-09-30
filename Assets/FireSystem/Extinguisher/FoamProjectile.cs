using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class FoamProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 2.0f;
    private FireType _foamType;
    private Rigidbody _rb;
    private ObjectPool<FoamProjectile> _pool;
    private float _timer;

    public void Initialize(FireType type, ObjectPool<FoamProjectile> pool)
    {
        _foamType = type;
        _pool = pool;
        _rb = GetComponent<Rigidbody>();
        _timer = 0f;
    }

    public void Fire(Vector3 direction, float speed)
    {
        _rb.linearVelocity = direction * speed;
        _timer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        IExtinguishable extinguishable = other.GetComponentInParent<IExtinguishable>();
        
        extinguishable?.Extinguish(_foamType);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= lifetime)
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
        _pool.ReturnToPool(this);
    }
}
