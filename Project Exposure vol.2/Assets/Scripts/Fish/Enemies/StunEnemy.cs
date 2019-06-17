using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEnemy : MonoBehaviour
{
    [SerializeField]
    private float _stunRange = 5;
    [SerializeField]
    private int _stunTimeInMs = 3000;
    [SerializeField]
    private int _stunCooldownInMs = 8000;
    [SerializeField]
    private bool _drawRange = false;

    private PlayerMovementBehaviour _playerMovementBehaviour;
    private GameObject _player;

    private bool _stunOnCooldown;
    private DateTime _continueStunTime;

    // Start is called before the first frame update
    void Start()
    {
        _player = SingleTons.GameController.Player;
        _playerMovementBehaviour = _player.GetComponent<PlayerMovementBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_stunOnCooldown)
        {
            if (DateTime.Now > _continueStunTime)
            {
                _stunOnCooldown = false;
            }
        }
        else
        {
            Vector3 diffVector = (transform.position - _player.transform.position);
            if (Vector3.Distance(transform.position, _player.transform.position) < _stunRange)
            {
                _playerMovementBehaviour.StunPlayer(_stunTimeInMs);
                CameraShake.Shake(_stunTimeInMs * 0.001f, 0.75f);
                _stunOnCooldown = true;
                _continueStunTime = DateTime.Now.AddMilliseconds(_stunCooldownInMs);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_drawRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _stunRange);
        }
    }
}
