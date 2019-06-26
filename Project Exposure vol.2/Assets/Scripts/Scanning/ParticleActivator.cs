using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleActivator : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private bool _hasBeenScanned = false;
    private AudioSource _audioSource;
    private int _frameCount;
    private float[] _samples0 = new float[1024];
    private float[] _samples1 = new float[1024];
    private ParticleSystem.MainModule _main;

    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = transform.parent.parent.GetComponent<AudioSource>();
        _main = _particleSystem.main;
        _particleSystem.Stop();
        ResetParticleSystem(_main);

        SingleTons.SoundWaveManager.onFishScanEvent += ScanEvent;
    }

    private void ResetParticleSystem(ParticleSystem.MainModule pMain)
    {
        pMain.loop = false;
        pMain.maxParticles = 5;
        pMain.duration = 4;
        pMain.startSize = 3;
        pMain.startLifetime = 2;
        pMain.simulationSpeed = 5;
        ParticleSystem.Burst burst = _particleSystem.emission.GetBurst(0);
        burst.time = 0.5f;
    }

    private void OnDisable()
    {
        SingleTons.SoundWaveManager.onFishScanEvent -= ScanEvent;
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

        if (other.tag == "Player" && other.gameObject.name == "Player")
        {
            _particleSystem.Play();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_hasBeenScanned || other.tag != "Player") return;
        if (other.gameObject.name != "Player") return;

        _frameCount++;
        if (_frameCount % 10 != 0) return;
        _audioSource.GetSpectrumData(_samples0, 0, FFTWindow.BlackmanHarris);
        _audioSource.GetSpectrumData(_samples1, 1, FFTWindow.BlackmanHarris);

        float b = 0.0f;
        for (int i = 0; i < 1024; i++)
        {
            float d = 0.0f;
            d = _samples0[i];
            b = Mathf.Max(d, b);
            d = _samples1[i];
            b = Mathf.Max(d, b);
        }

        if (b * 250 > 1)
        {
            _particleSystem.Stop();
            _particleSystem.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && other.gameObject.name == "Player")
        {
            _particleSystem.Stop();
            _particleSystem.Clear();
        }
    }
}
