using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviour : FishBehaviourParent
{
    protected Rigidbody _rigidBody;

    [SerializeField]
    protected float _minSpeed = 10;
    [SerializeField]
    protected float _maxSpeed = 15;
    [SerializeField]
    protected float _turningSpeed = 10;
    [SerializeField]
    [Range(0, 100)]
    protected float _speedUpRate = 10;
    [SerializeField]
    protected GameObject _dummy;

    protected float _currentSpeed;

    [SerializeField]
    protected List<FishManager.AvoidableCreatures> _creaturesToAvoid = new List<FishManager.AvoidableCreatures>();

    [SerializeField]
    protected bool _alwaysAvoid = false;

    public bool _isAvoiding = false;

    // Start is called before the first frame update
    void Start()
    {
        _currentSpeed = _minSpeed;
        SingleTons.FishManager.AddAvoidableCreature(_thisCreatureType, this);
        Debug.Log("Added " + gameObject.name + " to " + _thisCreatureType);
    }

    public float GetMinSpeed()
    {
        return _minSpeed;
    }

    public float GetMaxSpeed()
    {
        return _maxSpeed;
    }

    public float GetTurningSpeed()
    {
        return _turningSpeed;
    }

    public GameObject GetDummy()
    {
        return _dummy;
    }

    public float GetCurrentSpeed()
    {
        return _currentSpeed;
    }
}
