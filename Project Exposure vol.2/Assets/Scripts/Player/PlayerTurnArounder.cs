using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerTurnArounder : MonoBehaviour
{
    private GameObject _player;
    private GameObject _camera;
    private CameraBehaviour _cameraBehaviour;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    private DateTime _stopMovingAwayTime;
    private bool _hasDisabledPlayerMovement = false;
    private bool _playerHasExited = false;

    [SerializeField]
    private int _swimAwayTimeInMs = 500;
    [SerializeField]
    private int _turnSpeedMultiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        _player = SingleTons.GameController.Player;
        _playerMovementBehaviour = _player.GetComponent<PlayerMovementBehaviour>();
        _camera = Camera.main.gameObject;
        _cameraBehaviour = _camera.transform.parent.GetComponent<CameraBehaviour>();
    }

    private void Update()
    {
        if (_hasDisabledPlayerMovement)
        {
            if (DateTime.Now > _stopMovingAwayTime && _playerHasExited)
            {
                _cameraBehaviour.enabled = true;
                _hasDisabledPlayerMovement = false;
                _cameraBehaviour.GetJoystickBehaviour().gameObject.SetActive(true);
            }
            else
                MovePlayer();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            _hasDisabledPlayerMovement = true;
            _playerHasExited = false;
            _cameraBehaviour.GetJoystickBehaviour().gameObject.SetActive(false);
        }
    }

    private void MovePlayer()
    {
        Vector3 rot;
        if (transform.rotation.eulerAngles == Vector3.zero)
        {
            _cameraBehaviour.GetDummy().transform.LookAt(_cameraBehaviour.transform.position + (transform.up * 10));
            rot = new Vector3(90, Camera.main.transform.parent.localRotation.eulerAngles.y, Camera.main.transform.parent.localRotation.eulerAngles.z);// _cameraBehaviour.GetDummy().transform.localRotation.eulerAngles;//new Vector3(_cameraBehaviour.GetDummy().transform.rotation.eulerAngles.x * transform.right.x, _cameraBehaviour.GetDummy().transform.rotation.eulerAngles.y * transform.right.y, _cameraBehaviour.GetDummy().transform.rotation.eulerAngles.z * transform.right.z);
            Camera.main.transform.parent.localRotation = Quaternion.Slerp(Camera.main.transform.parent.localRotation, Quaternion.Euler(rot), Time.fixedDeltaTime * 2 * _turnSpeedMultiplier);
            _cameraBehaviour.SetDummyRotation(Camera.main.transform.parent.rotation);
        }
        else
        {
            _cameraBehaviour.GetDummy().transform.LookAt(_cameraBehaviour.transform.position + (transform.up * 10));
            rot = new Vector3(_cameraBehaviour.GetDummy().transform.rotation.eulerAngles.x * transform.right.x, _cameraBehaviour.GetDummy().transform.rotation.eulerAngles.y * transform.right.y, _cameraBehaviour.GetDummy().transform.rotation.eulerAngles.z * transform.right.z);
            Camera.main.transform.parent.rotation = Quaternion.Slerp(Camera.main.transform.parent.rotation, Quaternion.Euler(rot), Time.fixedDeltaTime * 2 * _turnSpeedMultiplier);
        }
        _playerMovementBehaviour.Swim();
    }

    private void OnTriggerExit(Collider other)
    {
        _stopMovingAwayTime = DateTime.Now.AddMilliseconds(_swimAwayTimeInMs);
        _playerHasExited = true;
    }
}
