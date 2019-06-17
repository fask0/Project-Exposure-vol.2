using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinBehaviour : FishBehaviour
{
    private DolphinParentBehaviour _dolphinParent;

    [SerializeField]
    private DolphinParentBehaviour.DolphinState _dolphinState = DolphinParentBehaviour.DolphinState.RandomStartingBehaviour;

    private bool _fishTooClose = false;
    private GameObject _fishThatsTooClose;
    private FishBehaviourParent _fishThatsTooCloseBehaviour;
    private Vector3 _checkpoint;

    private int _pathIndex = 0;
    private Vector3[] _pathPositions;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _dolphinParent = transform.parent.GetComponent<DolphinParentBehaviour>();
        _rigidBody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _dolphinParent.GetGuidingPath().enabled = true;
        if (_dolphinParent == null)
        {
            Debug.Log("The parent of the dolphin needs to have a DolphinParentBehaviour script attached to it");
            enabled = false;
            return;
        }

        _pathPositions = new Vector3[_dolphinParent.GetGuidingPath().positionCount];
        for (int i = 0; i < _dolphinParent.GetGuidingPath().positionCount; i++)
        {
            _pathPositions[i] = _dolphinParent.GetGuidingPath().GetPosition(i) + _dolphinParent.transform.position;
        }
        _dolphinParent.GetGuidingPath().enabled = false;

        GenerateNewCheckpoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (InRangeOfCheckPoint())
        {
            GenerateNewCheckpoint();
        }

        _rigidBody.velocity = Vector3.zero;

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

    private void GenerateNewCheckpoint()
    {
        switch (_dolphinState)
        {
            case DolphinParentBehaviour.DolphinState.RandomStartingBehaviour:
                _checkpoint = _dolphinParent.GetStartingFishZone().GenerateNewCheckPoint(transform.position);
                break;
            case DolphinParentBehaviour.DolphinState.PathingBehaviour:
                if (_pathIndex < _pathPositions.Length)
                {
                    _checkpoint = _pathPositions[_pathIndex];
                    _pathIndex++;
                }
                else
                {
                    _dolphinState = DolphinParentBehaviour.DolphinState.RandomEndBehaviour;
                }
                break;
            case DolphinParentBehaviour.DolphinState.RandomEndBehaviour:
                _checkpoint = _dolphinParent.GetEndingFishZone().GenerateNewCheckPoint(transform.position);
                break;
        }

        if (_animator == null) return;
        if (Vector3.Dot(transform.right, _checkpoint - transform.position) > 0)
            _animator.SetTrigger("_TurnRight");
        else
            _animator.SetTrigger("_TurnLeft");
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
        _dummy.transform.LookAt(_checkpoint, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
    }

    private void RotateAwayFromFish()
    {
        _dummy.transform.LookAt(Reflect(_checkpoint, _fishThatsTooClose.transform.position), Vector3.up);

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

    private bool InRangeOfCheckPoint()
    {
        if (Vector3.Distance(transform.position, _checkpoint) < 2)
        {
            return true;
        }
        return false;
    }

    public void SetDolphinState(DolphinParentBehaviour.DolphinState dolphinState)
    {
        _dolphinState = dolphinState;
        GenerateNewCheckpoint();
    }

    public DolphinParentBehaviour.DolphinState GetDolphinState()
    {
        return _dolphinState;
    }
}
