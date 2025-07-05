
using UnityEngine;

public class FireObject : MonoBehaviour
{
    private Material material;
    private FireParticle fireParticle;
    public float burn_time = 5.0f; //Seconds;
    public float combustibility = 0.5f;
    public float explosion_radius = 0.0f;
    public GameObject fireParticles;

    [HideInInspector] public bool is_burning = false;
    [HideInInspector] public bool is_burnt = false;
    [HideInInspector] public float burn_timer = 0.0f;

    void Awake()
    {
        GameObject.Instantiate(fireParticles, transform);
    }
    void Start()
    {
        material = GetComponent<Renderer>().material;
        fireParticle = GetComponentInChildren<FireParticle>();
    }

    public void Ignite()
    {
        if (is_burning || is_burnt)
        {
            return;
        }

        is_burning = true;
        burn_timer = burn_time;
        Debug.Log($"{gameObject.name} has ignited!");
    }

    public float GetPercentCombusted()
    {
        return Mathf.Clamp01((burn_time - burn_timer) / burn_time);
    }

    // Update is called once per frame
    public void BurnUpdate()
    {
        if (is_burning)
        {
            burn_timer -= Time.deltaTime;
            float progress = Mathf.Clamp01(1.0f - (burn_timer / burn_time));
            SetBurnProgress(progress);

            if (burn_timer <= 0.0f)
            {
                BurnOut();
            }
        }
    }

    private void BurnOut()
    {
        is_burning = false;
        is_burnt = true;
        SetBurnProgress(1.0f);
        Debug.Log($"{gameObject.name} has burnt out.");
    }

    private void SetBurnProgress(float value)
    {
        material.SetFloat("_BurnProgress", value);
        fireParticle.SetSize(value);
    }
    
}
