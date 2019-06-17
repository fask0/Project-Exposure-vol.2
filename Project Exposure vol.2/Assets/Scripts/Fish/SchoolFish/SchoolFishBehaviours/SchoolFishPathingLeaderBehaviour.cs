using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolFishPathingLeaderBehaviour : SchoolFishLeaderBehaviour
{
    private LineRenderer _lineRenderer;
    private Vector3[] _pathPositions;
    private int _pathIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _checkpoint = transform.position;

        _lineRenderer = transform.parent.GetComponent<LineRenderer>();

        _pathPositions = new Vector3[_lineRenderer.positionCount];
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            _pathPositions[i] = _lineRenderer.GetPosition(i) + _lineRenderer.transform.position;
        }
        _lineRenderer.enabled = false;
    }

    private void GetSchool()
    {
        if (transform.parent.parent.GetComponent<FishZone>())
        {
            _school = transform.parent.parent.GetComponent<FishZone>();
            _school.AddFishToSchool(this.gameObject, false, true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _dummy.transform.LookAt(_checkpoint, Vector3.up);

        if (Vector3.Distance(transform.position, _checkpoint) < 8)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed * (Vector3.Distance(transform.position, _checkpoint)));
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
        }

        transform.position += (transform.forward * Time.fixedDeltaTime * _minSpeed);
    }

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
            GetNewCheckpoint();
        }

        //Correct schoolfish movement
        CorrectSchoolFishMovement();
    }

    private void CorrectSchoolFishMovement()
    {
        if (fishCheckingIndex > fishCheckingSubdivision)
        {
            fishCheckingIndex = 0;
        }

        float count = _schoolFishWithLeader.Count / (fishCheckingSubdivision + 1);
        int startIndex = (int)(count * fishCheckingIndex);

        for (int i = startIndex; i < startIndex + count; i++)
        {
            if (!_schoolFishWithLeaderBehaviours[i].IsFishTooClose())
            {
                for (int j = startIndex; j < _schoolFishWithLeader.Count; j++)
                {
                    if (j >= startIndex && j <= i) { continue; }

                    if (Vector3.Distance(_schoolFishWithLeader[i].transform.position, _schoolFishWithLeader[j].transform.position) < _schoolFishWithLeaderBehaviours[j].GetThreatRange())
                    {
                        _schoolFishWithLeaderBehaviours[i].SetFishToAvoid(_schoolFishWithLeaderBehaviours[j]);
                        _schoolFishWithLeaderBehaviours[j].SetFishToAvoid(_schoolFishWithLeaderBehaviours[i]);
                        break;
                    }
                }
            }
        }

        fishCheckingIndex++;
    }

    private void GetNewCheckpoint()
    {
        if (_pathIndex >= _pathPositions.Length)
            _pathIndex = 0;
        _checkpoint = _pathPositions[_pathIndex];
        _pathIndex++;
    }

    private bool InRangeOfCheckPoint()
    {
        if (Vector3.Distance(transform.position, _checkpoint) < 2)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_checkpoint, 2);
    }
}
