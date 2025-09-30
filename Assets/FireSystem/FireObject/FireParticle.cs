using Watona.Variables;
using UnityEngine;
using System;

[RequireComponent(typeof(ParticleSystem))]
public class FireParticle : MonoBehaviour
{
    public static event Action ParticleCollision;
    [SerializeField] private FloatVariable _fireDistance;
    [SerializeField] private ParticleSystem _particle;
    [SerializeField] private BoxCollider _collider;
    private ParticleSystem.ShapeModule _shape;
    private ParticleSystem.EmissionModule _emission;
    private ParticleSystem.SubEmittersModule _subEmitters;
    private float _basicRateOverTime, _maximunRateOverTime;
    private Vector3 _colliderSize;
    void Start()
    {
        _particle = GetComponent<ParticleSystem>();
        _shape = _particle.shape;
        _emission = _particle.emission;
        _subEmitters = _particle.subEmitters;
        _basicRateOverTime = _particle.emission.rateOverTime.constant;
        _maximunRateOverTime = _basicRateOverTime * _fireDistance.Value;
        _collider.enabled = true;
    }
    public void UpdateValues(float progress)
    {
        if (!_particle.isEmitting) _particle.Play();

        float radius = Mathf.Lerp(1, _fireDistance.Value, progress);
        float emitterProbability = Mathf.Lerp(0.3f, 0.6f, progress);
        _colliderSize = new Vector3(radius * 2, 0.125f, radius * 2);

        _shape.radius = radius;
        _collider.size = _colliderSize;
        _emission.rateOverTime = Mathf.Lerp(_basicRateOverTime, _maximunRateOverTime, progress);
        _subEmitters.SetSubEmitterEmitProbability(0, emitterProbability);
    }
    public void Extinguish()
    {
        _particle.Stop();
        _collider.enabled = false;
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.LogWarning("Something hit the flames");
        if (other.GetComponentInParent<Extinguisher>() != null)
        {
            Debug.LogWarning("Foam hitted the flames");
            ParticleCollision?.Invoke();
        }
    }
}