using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScanEvent : MonoBehaviour
{
    public UnityEvent afterScan;

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.SoundWaveManager.scanEvents.Add(gameObject, afterScan);
    }

    private void OnDisable()
    {
        SingleTons.SoundWaveManager.scanEvents.Remove(gameObject);
    }
}
