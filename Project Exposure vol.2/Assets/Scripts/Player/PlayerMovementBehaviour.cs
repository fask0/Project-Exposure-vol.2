using System;
using UnityEngine;

public class PlayerMovementBehaviour : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 5.0f;
    [SerializeField] private float _acceleration = 1.0f;

    private Animator _animator;

    private JoystickBehaviour _joystickBehaviour;
    private Rigidbody _rigidbody;
    private CapsuleCollider _collider;

    private Vector3 _direction = Vector3.zero;

    private float _velocity;
    private float _waterResistance;

    //Stunns
    private bool _isStunned;
    private DateTime _stopStunTime;

    //Following
    private bool _isFollowing;
    private GameObject _followTarget;
    private Transform _followPoint;
    private bool _inFollowPosition;
    private float _timeToGoInFollowPosition;
    private bool _followeeDoesntHaveFollowPoints;

    void Start()
    {
        _waterResistance = _acceleration * 0.5f;
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();
        _joystickBehaviour = Camera.main.transform.GetChild(0).GetChild(1).GetComponent<JoystickBehaviour>();

        SingleTons.GameController.Player = this.gameObject;

        transform.parent.rotation = Quaternion.identity;
    }

    void Update()
    {
        _rigidbody.velocity = Vector3.zero;
        if (_isStunned)
        {
            if (DateTime.Now >= _stopStunTime)
            {
                _isStunned = false;
            }
        }
        else
        {
            if (_isFollowing)
            {
                _timeToGoInFollowPosition += Time.deltaTime;
                transform.position = Vector3.Slerp(transform.position, _followPoint.position, Time.deltaTime * 2);
                transform.rotation = Quaternion.Slerp(transform.rotation, _followPoint.rotation, Time.deltaTime * 2);
                _animator.SetBool("IsIdle", false);
                _animator.SetBool("IsSwimming", true);

                if (_timeToGoInFollowPosition > 0.5f)
                    _inFollowPosition = true;

                if (_joystickBehaviour.IsPressed())
                    StopFollowingGameObject(_followTarget);
            }
            else
            {
                //Rotation and movement
                if (_joystickBehaviour.IsPressed())
                {
                    if (_joystickBehaviour.GetTimeAtZero() >= 0.5f || _joystickBehaviour.Vertical() != 0)
                    {
                        _velocity += _acceleration * Time.deltaTime;
                        _animator.SetBool("IsIdle", false);
                        _animator.SetBool("IsSwimming", true);
                    }
                    else
                    {
                        _velocity -= _waterResistance * Time.deltaTime;

                        if (_velocity < 2)
                        {
                            _animator.SetBool("IsIdle", true);
                            _animator.SetBool("IsSwimming", false);
                        }
                    }
                }
                else
                {
                    _velocity -= _waterResistance * Time.deltaTime;
                    _rigidbody.velocity = Vector3.zero;
                    _animator.SetBool("IsIdle", true);
                    _animator.SetBool("IsSwimming", false);
                }

                _direction = Camera.main.transform.forward;
                _velocity = Mathf.Clamp(_velocity, 0, _maxSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(90 * ((_velocity / _maxSpeed) - _joystickBehaviour.Vertical()),
                                                                                           Camera.main.transform.parent.transform.rotation.eulerAngles.y + 90 * _joystickBehaviour.Horizontal(),
                                                                                           0), 2 * Time.deltaTime);

                transform.Translate(_direction * _velocity * Time.deltaTime, Space.World);
            }
        }
    }

    public void StartFollowingGameObject(GameObject pGameObject)
    {
        if (_followTarget == pGameObject) return;

        _followTarget = pGameObject;
        _followPoint = pGameObject.GetComponent<SetFollowPoints>().GetClosestPoint(transform);
        if (_followPoint == null)
        {
            _followeeDoesntHaveFollowPoints = true;
            return;
        }
        else
        {
            _followeeDoesntHaveFollowPoints = false;
        }

        _isFollowing = true;
        _inFollowPosition = false;
        Physics.IgnoreCollision(pGameObject.GetComponent<MeshCollider>(), GetComponent<CapsuleCollider>(), true);
    }

    public void StopFollowingGameObject(GameObject pGameObject)
    {
        _isFollowing = false;
        _followTarget = null;
        _followeeDoesntHaveFollowPoints = false;
        Physics.IgnoreCollision(pGameObject.GetComponent<MeshCollider>(), GetComponent<CapsuleCollider>(), false);
    }

    public float GetVelocity()
    {
        return _velocity;
    }

    public void StunPlayer(int stunTimeInMs)
    {
        _isStunned = true;
        _stopStunTime = DateTime.Now.AddMilliseconds(stunTimeInMs);
    }

    public bool CheckIfFollowingGameObject(GameObject pGameObject)
    {
        if (pGameObject == _followTarget && _inFollowPosition)
            return true;
        else
            return _followeeDoesntHaveFollowPoints;
    }

    public GameObject GetFollowTarget()
    {
        return _followTarget;
    }

    public bool GetIsFollowing()
    {
        return _isFollowing;
    }
}
