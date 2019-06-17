﻿using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private bool _isMainMenuScene = false;
    [SerializeField] private float _followSpeed = 10.0f;
    [SerializeField] private float _clampAngle = 80.0f;
    [SerializeField] private float _inputSensitivity = 10.0f;
    [SerializeField] private float _menuFollowSpeed = 6.0f;

    private float _rotX = 0.0f;
    private float _rotY = 0.0f;

    private GameObject _originalTarget;
    private GameObject _target;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    private JoystickBehaviour _joystickBehaviour;
    private Vector3 _initialCamPointPos;
    private GameObject _dummyGO;
    private GameObject _dummyGOGO;
    private GameObject _dummyGOGOGO;
    private GameObject _artifact;
    private Quaternion _dummyRotation;
    private Vector2 _clamp = Vector2.zero;

    private float _currentFollowSpeed;
    private bool _isScanningArtifact;
    private bool _targetIsFollowPath = false;
    private float _turnSpeed = 4.0f;
    private float _positionTurnSpeed = 4.0f;
    private GameObject _endTarget = null;

    void Start()
    {
        if (_isMainMenuScene) return;
        _originalTarget = SingleTons.GameController.Player.transform.parent.GetChild(1).gameObject;
        _target = _originalTarget;
        _playerMovementBehaviour = SingleTons.GameController.Player.GetComponent<PlayerMovementBehaviour>();
        transform.position = _target.transform.position;
        _joystickBehaviour = Camera.main.transform.GetChild(0).GetChild(1).GetComponent<JoystickBehaviour>();
        _currentFollowSpeed = _followSpeed;
        _dummyGO = new GameObject();
        _dummyGO.transform.SetParent(transform.GetChild(0));
        _dummyGO.transform.position = new Vector3(0, 0, 0);
        _dummyGOGO = new GameObject();
        _dummyGOGO.transform.SetParent(_dummyGO.transform);
        _dummyGOGO.transform.localPosition = Vector3.zero;
        _dummyGOGOGO = new GameObject();
        _dummyGOGOGO.transform.SetParent(transform.GetChild(0));
    }

    void Update()
    {
        if (_isMainMenuScene) return;
        if (_target != _originalTarget)
        {
            _dummyGO.transform.localPosition = new Vector3(0, 0, 0);
            _dummyGOGO.transform.localPosition = Vector3.zero;
            _dummyGOGOGO.transform.localPosition = new Vector3(0, 0, -8);
            if (!_targetIsFollowPath)
                transform.rotation = Quaternion.Slerp(transform.rotation, _target.transform.rotation, Time.deltaTime * _turnSpeed);
            else
            {
                if (_endTarget != null)
                    _dummyGOGOGO.transform.LookAt(_endTarget.transform);

                else
                    _dummyGOGOGO.transform.LookAt(_target.transform);
                transform.rotation = Quaternion.Slerp(transform.rotation, _dummyGOGOGO.transform.rotation, Time.deltaTime * _turnSpeed);
            }
        }
        else
        {
            if (_playerMovementBehaviour.GetIsFollowing())
            {
                transform.LookAt(_target.transform.position + _playerMovementBehaviour.gameObject.transform.up + _playerMovementBehaviour.gameObject.transform.forward * 0.35f);
            }
            else
            {
                if (_isScanningArtifact)
                {
                    _dummyGO.transform.position = transform.position;
                    _dummyGO.transform.LookAt(_artifact.transform);
                    transform.rotation = Quaternion.Slerp(transform.rotation, _dummyGO.transform.rotation, Time.deltaTime);
                    _dummyRotation = transform.rotation;
                }

                if (_joystickBehaviour.IsPressed())
                {
                    _isScanningArtifact = false;
                    if (_joystickBehaviour.Vertical() != 0)
                        _rotX += -_joystickBehaviour.Vertical() * _inputSensitivity * Time.deltaTime;
                    if (_joystickBehaviour.Horizontal() != 0)
                        _rotY += _joystickBehaviour.Horizontal() * _inputSensitivity * Time.deltaTime * 1.5f;

                    _clamp = Vector2.Lerp(_clamp, new Vector2(_clampAngle * Mathf.Abs(_joystickBehaviour.Vertical()), 0), Time.deltaTime * 3);
                    _rotX = Mathf.Clamp(_rotX, -_clamp.x, _clamp.x);

                    Quaternion localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_rotX, _rotY, 0), Time.deltaTime * 2);
                    transform.rotation = localRotation;

                    _dummyRotation = transform.rotation;
                }

                if (_joystickBehaviour.Vertical() == 0)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(((_dummyRotation.eulerAngles.x > 180) ? _dummyRotation.eulerAngles.x - 360 : _dummyRotation.eulerAngles.x) * 0.66f, transform.eulerAngles.y, 0), Time.deltaTime);
                    _rotX = transform.rotation.eulerAngles.x;
                    _rotX = (_rotX > 180) ? _rotX - 360 : _rotX;
                }

                if (_joystickBehaviour.Horizontal() == 0)
                {
                    _rotY = transform.localRotation.eulerAngles.y;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (_target != _originalTarget && Vector3.Distance(transform.position, _target.transform.position) > 0.1f && _targetIsFollowPath)
        {
            _dummyGOGO.transform.LookAt(_target.transform);
            _dummyGO.transform.rotation = Quaternion.Slerp(_dummyGO.transform.rotation, _dummyGOGO.transform.rotation, Time.deltaTime * _positionTurnSpeed);
            transform.position += _dummyGO.transform.forward * _currentFollowSpeed * Time.deltaTime;
        }
        else
            transform.position = Vector3.Slerp(transform.position, _target.transform.position, _currentFollowSpeed * Time.deltaTime);
    }

    public void SetTemporaryTarget(GameObject gameObject, bool isFollowPath = false, float followSpeedMultiplier = 1.0f, float turnSpeed = 4.0f, float posTurnSpeed = 4.0f, GameObject endTarget = null)
    {
        _target = gameObject;
        _targetIsFollowPath = isFollowPath;
        _currentFollowSpeed = _menuFollowSpeed * followSpeedMultiplier;
        _turnSpeed = turnSpeed;
        _positionTurnSpeed = posTurnSpeed;
        _endTarget = endTarget;
    }

    public void SetToOriginalTarget()
    {
        _target = _originalTarget;
        _currentFollowSpeed = _followSpeed;
        _targetIsFollowPath = false;
        _endTarget = null;
    }

    public GameObject GetTarget()
    {
        return _target;
    }

    public void StartScanningArtifact(GameObject pGameObject)
    {
        _isScanningArtifact = true;
        _artifact = pGameObject;
    }

    public void StopScanningArtifact()
    {
        _isScanningArtifact = false;
    }
}