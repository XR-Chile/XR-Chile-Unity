using System;
using UnityEngine;
public class FireObject : MonoBehaviour, IExtinguishable
{
    [Header("Base Settings")]
    [SerializeField] private FireType _type;
    public float explosionRadius = 0.0f;
    public GameObject fireParticlesPrefab;

    [Header("Expansion Settings")]
    [SerializeField, Tooltip("Seconds needed to reach peak expansion")] private float _expansionTime = 2.5f; //Seconds
    private float _expansionTimer;

    [Header("Burn Settings")]
    [Tooltip("Percentage needed to start burning")] public float combustibility = 0.5f;
    public float burnTime = 5.0f; //Seconds
    private float _burnTimer = 0.0f;
    private bool _isBurning = false;
    private bool _hasBurnt = false;

    [Header("Life Settings")]
    [SerializeField, Range(0.01f, 0.1f)] private float _extinguisherBasePower = 0.0625f;
    private float _maxLife = 1;
    private float _currentLife;

    private Material _material;
    private FireParticle _fireParticle;
    private GameObject _fireParticleGameObject;

    public bool IsBurning => _isBurning;
    public bool HasBurnt => _hasBurnt;
    public float BurnTimer => _burnTimer;

    public static event Action Extinguishing;

    public float GetPercentCombusted() => Mathf.Clamp01((burnTime - _burnTimer) / burnTime);
    public float GetPercentPropagated() => Mathf.Clamp01((_expansionTime - _expansionTimer) / _expansionTime); 
    private float GetExpansionMultiplier()
    {
        float lifePercentage = _currentLife / _maxLife;
        return lifePercentage *= _hasBurnt ? .66f : 1;
    }

    void Awake()
    {
        _fireParticleGameObject = GameObject.Instantiate(fireParticlesPrefab, transform);
        _fireParticleGameObject.transform.position = transform.position;
    }
    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _fireParticle = _fireParticleGameObject.GetComponent<FireParticle>();
    }

    public void Ignite()
    {
        if (_isBurning || _hasBurnt)
        {
            return;
        }

        _isBurning = true;
        _burnTimer = burnTime;
        _expansionTimer = _expansionTime;
        _currentLife = _maxLife;

        Debug.Log($"{gameObject.name} has ignited!");
    }

    // Update is called once per frame
    public void BurnUpdate()
    {
        if (!_isBurning) return;

        _burnTimer -= Time.deltaTime;
        _expansionTimer -= Time.deltaTime * GetExpansionMultiplier();

        //Calculate progress (0 to 1)
        float burningProgress = Mathf.Clamp01(1.0f - (_burnTimer / burnTime));
        float expansionProgress = Mathf.Clamp01(1.0f - (_expansionTimer / _expansionTime));

        _material.SetFloat("_BurnProgress", burningProgress);
        _fireParticle.UpdateValues(expansionProgress);

        if (_burnTimer <= 0.0f && !_hasBurnt) BurnOut();
    }

    private void BurnOut()
    {
        // isBurning = false;
        _hasBurnt = true;
        _material.SetFloat("_BurnProgress", 1f);
        Debug.Log($"{gameObject.name} has burnt out.");
    }

    public void Extinguish(FireType foamType)
    {
        if (!_isBurning) return; //If not burning do nothing

        float effectiveness = foamType == _type ? 1.0f : 0.66f;

        float extinguishEffect = effectiveness * _extinguisherBasePower;

        Debug.LogError("Extinguishing Fire");

        _currentLife = Mathf.Max(_currentLife -= extinguishEffect, 0);

        Extinguishing?.Invoke();

        if (_currentLife <= 0) CompletelyExtinguish();
    }

    private void CompletelyExtinguish()
    {
        _isBurning = false;
        _fireParticle.Extinguish();
        Debug.Log($"{gameObject.name} has been completely extinguished.");
    }
}

