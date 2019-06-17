using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPoint : MonoBehaviour
{
    [SerializeField] private Vector3 _offset = Vector3.zero;
    private Transform _player;
    private PlayerMovementBehaviour _playerMovementBehaviour;

    private void Start()
    {
        _player = transform.parent.transform.GetChild(0).transform;
        _playerMovementBehaviour = _player.gameObject.GetComponent<PlayerMovementBehaviour>();
    }

    void Update()
    {
        transform.position = _player.position + _offset;
    }
}
