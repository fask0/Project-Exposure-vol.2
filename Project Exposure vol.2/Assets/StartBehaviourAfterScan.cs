using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartBehaviourAfterScan : MonoBehaviour
{
    private FishBehaviour fishBehaviour;

    [SerializeField]
    private bool _startBehaviour = true;
    [SerializeField]
    private UnityEvent onScanEvent;
    [SerializeField]
    private UnityEvent afterScanEvent;
    [SerializeField]
    private UnityEvent onCancelScanEvent;

    // Start is called before the first frame update
    void Start()
    {
        if (_startBehaviour)
            fishBehaviour = GetComponent<FishBehaviour>();
        if (fishBehaviour != null)
        {
            SingleTons.SoundWaveManager.onFishScanEvent += StartFishBehaviour;
        }

        SingleTons.SoundWaveManager.scanEvents.Add(gameObject, afterScanEvent);
        SingleTons.SoundWaveManager.scanStartEvents.Add(gameObject, onScanEvent);
        SingleTons.SoundWaveManager.scanCancelEvents.Add(gameObject, onCancelScanEvent);
    }

    private void OnDisable()
    {
        SingleTons.SoundWaveManager.onFishScanEvent -= StartFishBehaviour;
        SingleTons.SoundWaveManager.scanEvents.Remove(gameObject);
    }

    void StartFishBehaviour(GameObject obj)
    {
        if (obj.name == this.name)
        {
            fishBehaviour.enabled = true;
        }
        else
        {
            Debug.Log(obj.name);
        }
    }
}
