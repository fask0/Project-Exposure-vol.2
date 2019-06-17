using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFishBehaviour : FishBehaviour
{
    private FishZone _school;
    private Vector3 _checkpoint;

    private bool _hasAddedItselfToSchool = false;

    private bool _fishTooClose = false;
    private GameObject _fishThatsTooClose;
    private FishBehaviourParent _fishThatsTooCloseBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _checkpoint = transform.position;
    }

    private void GetSchool()
    {
        if (transform.parent.GetComponent<FishZone>())
        {
            _school = transform.parent.GetComponent<FishZone>();
            _school.AddFishToSchool(this.gameObject, true);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_hasAddedItselfToSchool)
        {
            GetSchool();
            _hasAddedItselfToSchool = true;
        }

        //Generate new checkpoint if too close
        if (InRangeOfCheckPoint())
        {
            _checkpoint = _school.GenerateNewCheckPoint(transform.position);
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
        if (_hasAddedItselfToSchool)
        {
            _dummy.transform.LookAt(_checkpoint, Vector3.up);

            transform.rotation = Quaternion.Lerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
        }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(_checkpoint, 2);
    }
}
