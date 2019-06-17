using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyfishBehaviour : MonoBehaviour
{
    private enum MovingState
    {
        MovingUp,
        MovingDown,
        Static
    };
    [SerializeField]
    private MovingState _movingState = MovingState.Static;

    [SerializeField]
    [Range(0, 10000)]
    private int _waitTimeInMs = 1000;
    [SerializeField]
    [Range(0, 10)]
    private float _startingYSpeed = 3.0f;
    [SerializeField]
    [Range(0, 1)]
    private float _moveUpDecay = 0.98f;
    [SerializeField]
    [Range(1, 2)]
    private float _moveDownAmplifier = 1.05f;
    private float _startYPosition;

    private float _yVelocity = 0;
    private DateTime _dateTime;
    private bool _hasSetDateTime = false;

    // Start is called before the first frame update
    private void Start()
    {
        _startYPosition = transform.position.y;
    }

    // Update is called once per frame
    private void Update()
    {
        switch (_movingState)
        {
            case MovingState.MovingUp:
                UpMovement();
                break;
            case MovingState.MovingDown:
                DownMovement();
                break;
            case MovingState.Static:
                StaticBehaviour();
                break;
        }
    }

    private void UpMovement()
    {
        _yVelocity *= _moveUpDecay;
        if (_yVelocity <= 0.1f)
        {
            _movingState = MovingState.MovingDown;
            _yVelocity = 0.1f;
        }
        transform.position += new Vector3(0, _yVelocity * Time.deltaTime, 0);
    }

    private void DownMovement()
    {
        _yVelocity *= _moveDownAmplifier;
        if (transform.position.y <= _startYPosition)
        {
            transform.position = new Vector3(transform.position.x, _startYPosition, transform.position.z);
            _movingState = MovingState.Static;
        }
        transform.position -= new Vector3(0, _yVelocity * Time.deltaTime, 0);
    }

    private void StaticBehaviour()
    {
        if (!_hasSetDateTime)
        {
            _dateTime = DateTime.Now;
            _hasSetDateTime = true;
        }

        if (DateTime.Now > _dateTime.AddMilliseconds(_waitTimeInMs))
        {
            _yVelocity = _startingYSpeed;
            _movingState = MovingState.MovingUp;
            _hasSetDateTime = false;
        }
    }
}
