using AuraAPI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightIntensityUpdater : MonoBehaviour
{
    private float _startIntensity;
    private AuraLight _auraLight;

    [SerializeField]
    private float _minValue = 1;

    // Start is called before the first frame update
    void Start()
    {
        _auraLight = GetComponent<AuraLight>();
        _startIntensity = _auraLight.strength;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            float dot = Vector3.Dot(Vector3.up, Camera.main.transform.forward);
            if (dot > 0)
            {
                _auraLight.strength = Mathf.Max(_startIntensity * (1 - dot), _minValue);
            }
            else
            {
                _auraLight.strength = _startIntensity;
            }
        }
    }
}
