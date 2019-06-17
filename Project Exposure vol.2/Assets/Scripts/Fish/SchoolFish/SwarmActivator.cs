using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmActivator : MonoBehaviour
{
    [SerializeField]
    private SwarmableArea _swarmArea;
    [SerializeField]
    private FishZone _fishZone;

    private enum ActivationShapeRange
    {
        Sphere,
        Box
    };

    [SerializeField]
    private ActivationShapeRange activationShape = ActivationShapeRange.Box;

    [SerializeField]
    private float _fishDisperseRange;
    [SerializeField]
    private float _fishSwarmRange;
    [SerializeField]
    private bool _onlyActivateOnce = false;
    [Header("Rubber banding")]
    [SerializeField]
    private int _secondsUntilActivation = 0;

    private bool _hasActivated = false;

    private DateTime _activationTime;

    // Start is called before the first frame update
    void Start()
    {
        _activationTime = DateTime.Now.AddSeconds(_secondsUntilActivation);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            _swarmArea.SwarmArea(_fishZone);
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            _swarmArea.StopSwarming();
        }

        if (DateTime.Now > _activationTime)
        {
            float diff = (transform.position - SingleTons.GameController.Player.transform.position).magnitude;
            if (diff * diff < _fishDisperseRange * _fishDisperseRange)
            {
                _swarmArea.StopSwarming();
            }
            else if (diff * diff < _fishSwarmRange * _fishSwarmRange)
            {
                if (_onlyActivateOnce && !_hasActivated)
                {
                    _swarmArea.SwarmArea(_fishZone);
                    _hasActivated = true;
                }
                else if (!_onlyActivateOnce)
                {
                    _swarmArea.SwarmArea(_fishZone);
                }
            }
            else
            {
                _swarmArea.StopSwarming();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch (activationShape)
        {
            case ActivationShapeRange.Sphere:
                Gizmos.color = new Color(255, 165, 0);
                Gizmos.DrawWireSphere(transform.position, _fishDisperseRange);
                Gizmos.color = new Color(128, 0, 128);
                Gizmos.DrawWireSphere(transform.position, _fishSwarmRange);
                break;
            case ActivationShapeRange.Box:
                break;
        }
    }
}
