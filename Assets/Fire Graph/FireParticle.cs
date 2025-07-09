using Watona.Variables;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireParticle : MonoBehaviour
{
    [SerializeField] private FloatVariable _fireDistance;
    [SerializeField] private ParticleSystem _particle;
    private ParticleSystem.ShapeModule _shape;
    private ParticleSystem.EmissionModule _emission;
    private ParticleSystem.SubEmittersModule _subEmitters;
    private float _basicRateOverTime, _maximunRateOverTime;
    void Start()
    {
        _particle = GetComponent<ParticleSystem>();
        _shape = _particle.shape;
        _emission = _particle.emission;
        _subEmitters = _particle.subEmitters;
        _basicRateOverTime = _particle.emission.rateOverTime.constant;
        _maximunRateOverTime = _basicRateOverTime * _fireDistance.Value;
    }
    public void UpdateValues(float progress)
    {
        if (!_particle.isEmitting) _particle.Play();
        _shape.radius = Mathf.Lerp(1, _fireDistance.Value, progress);
        _emission.rateOverTime = Mathf.Lerp(_basicRateOverTime, _maximunRateOverTime, progress);
        float emitterProbability = Mathf.Lerp(0.3f, 0.6f, progress);
        _subEmitters.SetSubEmitterEmitProbability(0,emitterProbability);
    }
}