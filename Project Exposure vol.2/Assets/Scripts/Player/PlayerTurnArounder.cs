using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnArounder : MonoBehaviour
{
    private GameObject _player;
    private GameObject _camera;
    private CameraBehaviour _cameraBehaviour;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        _player = SingleTons.GameController.Player;
        _playerMovementBehaviour = _player.GetComponent<PlayerMovementBehaviour>();
        _camera = Camera.main.gameObject;
        _cameraBehaviour = _camera.transform.parent.GetComponent<CameraBehaviour>();
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    if (Vector3.Dot(transform.up, (transform.position - _player.transform.position).normalized) > 0)
    //    {
    //        _cameraBehaviour.enabled = false;
    //        _cameraBehaviour.GetDummy().transform.LookAt(_cameraBehaviour.transform.position + transform.up * 10);
    //        Camera.main.transform.parent.rotation = Quaternion.Slerp(Camera.main.transform.parent.rotation, _cameraBehaviour.GetDummy().transform.rotation, Time.fixedDeltaTime);
    //    }
    //    else
    //    {
    //        _cameraBehaviour.enabled = true;
    //    }
    //}
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            Vector3 rot;
            if (transform.rotation.eulerAngles == Vector3.zero)
            {
                _cameraBehaviour.enabled = false;
                _cameraBehaviour.GetDummy().transform.LookAt(_cameraBehaviour.transform.position + (transform.up * 10));
                rot = new Vector3(90, Camera.main.transform.parent.localRotation.eulerAngles.y, Camera.main.transform.parent.localRotation.eulerAngles.z);// _cameraBehaviour.GetDummy().transform.localRotation.eulerAngles;//new Vector3(_cameraBehaviour.GetDummy().transform.rotation.eulerAngles.x * transform.right.x, _cameraBehaviour.GetDummy().transform.rotation.eulerAngles.y * transform.right.y, _cameraBehaviour.GetDummy().transform.rotation.eulerAngles.z * transform.right.z);
                Camera.main.transform.parent.localRotation = Quaternion.Slerp(Camera.main.transform.parent.localRotation, Quaternion.Euler(rot), Time.fixedDeltaTime * 2);
                _cameraBehaviour.SetDummyRotation(Camera.main.transform.parent.rotation);
            }
            else
            {
                _cameraBehaviour.enabled = false;
                _cameraBehaviour.GetDummy().transform.LookAt(_cameraBehaviour.transform.position + (transform.up * 10));
                rot = new Vector3(_cameraBehaviour.GetDummy().transform.rotation.eulerAngles.x * transform.right.x, _cameraBehaviour.GetDummy().transform.rotation.eulerAngles.y * transform.right.y, _cameraBehaviour.GetDummy().transform.rotation.eulerAngles.z * transform.right.z);
                Camera.main.transform.parent.rotation = Quaternion.Slerp(Camera.main.transform.parent.rotation, Quaternion.Euler(rot), Time.fixedDeltaTime * 2);
            }
            _playerMovementBehaviour.Swim();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _cameraBehaviour.enabled = true;
    }
}
