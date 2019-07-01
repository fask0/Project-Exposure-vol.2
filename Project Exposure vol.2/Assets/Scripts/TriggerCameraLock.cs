using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerCameraLock : MonoBehaviour
{
    [SerializeField]
    private Transform _targetTransform;
    [SerializeField]
    private bool _oneTimeActivation = true;
    [SerializeField]
    private int _lockTimeInMs = 5000;
    [SerializeField]
    private int _callStillAfterTimeInMs = 2500;
    [SerializeField]
    private float _movementSpeedMultiplier = 2;
    [SerializeField]
    private float _rotationSpeedMultiplier = 3;

    [SerializeField]
    private UnityEvent _startEvent;
    [SerializeField]
    private UnityEvent _stillEvent;
    [SerializeField]
    private UnityEvent _stillAfterTimeEvent;
    [SerializeField]
    private UnityEvent _stopEvent;

    private GameObject _player;
    private GameObject _camera;
    private CameraBehaviour _cameraBehaviour;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    private bool _playerLocked = false;
    private bool _playerAtTargetPosition = false;
    private bool _hasBeenActivatedBefore = false;

    private DateTime _playerUnlockTime;
    private DateTime _callStillEventTime;
    private bool _hasCalledStillEvent = false;

    // Start is called before the first frame update
    void Start()
    {
        _player = SingleTons.GameController.Player;
        _playerMovementBehaviour = _player.GetComponent<PlayerMovementBehaviour>();
        _camera = Camera.main.gameObject;
        _cameraBehaviour = _camera.transform.parent.GetComponent<CameraBehaviour>();

        foreach (AudioSource source in GetComponentsInChildren<AudioSource>())
        {
            source.Stop();
        }
    }

    private void FixedUpdate()
    {
        if (_playerLocked)
        {
            if (!_playerAtTargetPosition)
            {
                _player.transform.position = Vector3.Lerp(_player.transform.position, _targetTransform.position, Time.fixedDeltaTime * _movementSpeedMultiplier);
                _cameraBehaviour.SetDummyRotation(Quaternion.Slerp(_cameraBehaviour.GetDummyRotation(), _targetTransform.rotation, Time.fixedDeltaTime * _rotationSpeedMultiplier));
                _cameraBehaviour.transform.rotation = (Quaternion.Slerp(_cameraBehaviour.GetDummyRotation(), _targetTransform.rotation, Time.fixedDeltaTime * _rotationSpeedMultiplier));

                if (Vector3.Distance(_player.transform.position, _targetTransform.position) < 0.05f)
                {
                    //Player reached target position
                    _playerAtTargetPosition = true;
                    _playerUnlockTime = DateTime.Now.AddMilliseconds(_lockTimeInMs);
                    _stillEvent.Invoke();
                    _callStillEventTime = DateTime.Now.AddMilliseconds(_callStillAfterTimeInMs);
                }
            }
            else
            {
                if (!_hasCalledStillEvent && DateTime.Now > _callStillEventTime)
                {
                    _stillAfterTimeEvent.Invoke();
                    _hasCalledStillEvent = true;
                }

                if (DateTime.Now > _playerUnlockTime)
                {
                    //Unlock player
                    UnlockPlayer();
                    _hasCalledStillEvent = false;
                }
            }
        }
    }

    private void LockPlayer()
    {
        _playerLocked = true;
        _hasBeenActivatedBefore = true;
        _startEvent.Invoke();
    }

    private void UnlockPlayer()
    {
        _playerLocked = false;
        _playerAtTargetPosition = false;
        _stopEvent.Invoke();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_hasBeenActivatedBefore || !_oneTimeActivation)
        {
            if (other.tag == "Player")
            {
                LockPlayer();
            }
        }
    }
}
