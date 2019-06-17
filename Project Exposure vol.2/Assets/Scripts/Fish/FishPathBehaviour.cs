using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPathBehaviour : FishBehaviour
{
    private LineRenderer _lineRenderer;
    private Vector3[] _pathPositions;

    private int _index = 0;
    private int _targetCount = 0;
    private bool _hasTarget = false;

    private Vector3 _currentTarget;
    private bool _fishTooClose = false;
    private GameObject _fishThatsTooClose;
    private FishBehaviourParent _fishThatsTooCloseBehaviour;
    private Animator _animator;

    // Start is called before the first frame update
    private void Start()
    {
        //School the designers rq
        if (transform.parent.GetComponent<LineRenderer>() == null)
        {
            Debug.Log("ADD A FAKIGN LOINE RENDERER TO THE PARENT OF THE FISH YA SMELLY WANKER");
            return;
        }
        _lineRenderer = transform.parent.GetComponent<LineRenderer>();
        _targetCount = _lineRenderer.positionCount;
        //Ditto
        if (_targetCount <= 0)
        {
            Debug.Log("MAKE SOME FUCKING POINTS YA STUPID DESIGNER");
            return;
        }

        //Create and populate position array
        _pathPositions = new Vector3[_lineRenderer.positionCount];
        for (int i = 0; i < _targetCount; i++)
        {
            _pathPositions[i] = _lineRenderer.GetPosition(i);
        }

        //Disable line renderer
        _lineRenderer.enabled = false;

        //Get starting target
        GetNewTarget();

        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckIfInRangeOfTarget();

        if (!_hasTarget)
            GetNewTarget();

        if (!_fishTooClose)
        {
            RotateTowardsCheckPoint();
        }
        else
        {
            RotateAwayFromFish();
        }

        SpeedUpAndDown();
        transform.position += (transform.forward * Time.deltaTime * _currentSpeed);

        _rigidBody.velocity = Vector3.zero;

        if (!_fishTooClose)
        {
            //Iterate over creatures to avoid
            foreach (FishManager.AvoidableCreatures creatureType in _creaturesToAvoid)
            {
                foreach (FishBehaviourParent fishBehaviour in SingleTons.FishManager.GetAvoidableCreatures(creatureType))
                {
                    AvoidFish(fishBehaviour);
                }
            }
        }
        else
        {
            //Check if still too close to certain creature
            if (Vector3.Distance(transform.position, _fishThatsTooClose.transform.position) > _fishThatsTooCloseBehaviour.GetThreatFleeRange())
            {
                _fishTooClose = false;
            }
            return;
        }
    }

    private void CheckIfInRangeOfTarget()
    {
        if ((transform.position - _currentTarget).magnitude * (transform.position - _currentTarget).magnitude < 2)
        {
            _hasTarget = false;
        }
    }

    private void AvoidFish(FishBehaviourParent fish)
    {
        GameObject fishObj = fish.gameObject;
        Vector3 fishPos = fishObj.transform.position;

        if (Vector3.Distance(transform.position, fishPos) < fish.GetThreatRange())
        {
            _fishThatsTooClose = fishObj;
            _fishThatsTooCloseBehaviour = fish;
            _fishTooClose = true;
            return;
        }
    }

    private void RotateTowardsCheckPoint()
    {
        _dummy.transform.LookAt(_currentTarget, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
    }

    private void RotateAwayFromFish()
    {
        _dummy.transform.LookAt(Reflect(_currentTarget, _fishThatsTooClose.transform.position), Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
    }

    private void SpeedUpAndDown()
    {
        if (_fishTooClose)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, Mathf.Min(_minSpeed * _fishThatsTooCloseBehaviour.GetThreatLevel(), _maxSpeed), Time.fixedDeltaTime * _speedUpRate);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, _minSpeed, Time.fixedDeltaTime * _speedUpRate);
        }
    }

    private Vector3 Reflect(Vector3 _checkPoint, Vector3 _otherFishPos)
    {
        Vector3 diff = transform.position - _otherFishPos;
        Vector3 subtractingValue = (_checkPoint - transform.position).normalized * diff.magnitude;

        return transform.position + (diff * 2 - new Vector3(0, 0, subtractingValue.z));
    }

    private void GetNewTarget()
    {
        //Reset index
        if (_index >= _targetCount) { _index = 0; }

        //Get new target
        _currentTarget = _pathPositions[_index] + transform.parent.position;
        if (_animator != null)
        {
            if (Vector3.Dot(transform.right, _pathPositions[_index] - transform.localPosition) > 0)
                _animator.SetTrigger("_TurnRight");
            else
                _animator.SetTrigger("_TurnLeft");
        }
        _hasTarget = true;
        _index++;
    }
}
