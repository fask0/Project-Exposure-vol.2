using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleActivator : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private bool _hasBeenScanned = false;

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        SingleTons.SoundWaveManager.onFishScanEvent += ScanEvent;
    }

    private void ScanEvent(GameObject pGameObject)
    {
        if (pGameObject.name == gameObject.name)
        {
            _particleSystem.Stop();
            _particleSystem.Clear();

            _hasBeenScanned = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasBeenScanned) return;

        if (other.tag == "Player")
        {
            _particleSystem.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            _particleSystem.Stop();
            _particleSystem.Clear();
        }
    }
}
