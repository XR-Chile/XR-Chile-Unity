using Watona.Variables;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class FireParticle : MonoBehaviour
{
    public FloatVariable fireDistance;
    private ParticleSystem particle;
    private ParticleSystem.ShapeModule shape;
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        shape = particle.shape;
    }
    public void SetSize(float progress)
    {
        shape.radius = Mathf.Lerp(fireDistance.Value, 0, progress);
    }
}