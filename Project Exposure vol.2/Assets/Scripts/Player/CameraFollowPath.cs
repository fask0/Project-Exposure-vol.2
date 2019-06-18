using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraFollowPath : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private GameObject[] _pathPoints;
    private int _pathPointIndex = 0;

    private GameObject _camera;
    private CameraBehaviour _cameraBehaviour;

    private enum CameraState
    {
        None,
        FollowingPath,
        BacktrackingPath,
        Still
    };
    private CameraState _cameraState = CameraState.None;
    private DateTime _startBacktrackTime;

    [SerializeField]
    private GameObject _objectToFocusOn;
    [SerializeField]
    private bool _startCameraPath = false;
    [SerializeField]
    private int _standStillTimeInMs = 5000;

    [SerializeField]
    private float _cameraLookSpeed = 2.0f;
    [SerializeField]
    private float _cameraPathTurnSpeed = 8.0f;

    public UnityEvent StillEvent;
    public UnityEvent StartEvent;
    public UnityEvent StartEndEvent;
    public UnityEvent EndEvent;

    // Start is called before the first frame update
    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _pathPoints = new GameObject[_lineRenderer.positionCount];
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.position = _lineRenderer.GetPosition(i) + transform.position;
            _pathPoints[i] = obj;
        }

        _lineRenderer.enabled = false;
        _camera = Camera.main.transform.parent.gameObject;
        _cameraBehaviour = _camera.GetComponent<CameraBehaviour>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_startCameraPath)
        {
            _startCameraPath = false;
            StartPathFollow();
        }

        ActOnState();
    }

    public void StartPathFollow()
    {
        _cameraState = CameraState.FollowingPath;
        _pathPoints[_pathPoints.Length - 1].transform.LookAt(_objectToFocusOn.transform);
        for (int i = 0; i < _pathPoints.Length; i++)
        {
            _pathPoints[i].transform.rotation = Quaternion.Slerp(_camera.transform.rotation, _pathPoints[_pathPoints.Length - 1].transform.rotation, (float)i / (float)(_pathPoints.Length - 1));
        }

        StartEvent.Invoke();
    }

    private void StartStill()
    {
        _cameraState = CameraState.Still;
        _cameraBehaviour.SetTemporaryTarget(_objectToFocusOn, true, 0, 5.0f, 8.0f);
        _startBacktrackTime = DateTime.Now.AddMilliseconds(_standStillTimeInMs);

        StillEvent.Invoke();
    }

    private void StartBacktracking()
    {
        _cameraState = CameraState.BacktrackingPath;
        _pathPointIndex--;

        StartEndEvent.Invoke();
    }

    private void Stop()
    {
        _cameraState = CameraState.None;
        _cameraBehaviour.SetToOriginalTarget();
        EndEvent.Invoke();
    }

    private void ActOnState()
    {
        switch (_cameraState)
        {
            case CameraState.FollowingPath:
                FollowPath();
                break;
            case CameraState.BacktrackingPath:
                BacktrackPath();
                break;
            case CameraState.Still:
                Still();
                break;
            default:
                break;
        }
    }

    private void FollowPath()
    {
        if (_pathPointIndex < _pathPoints.Length)
        {
            if (Vector3.Distance(_camera.transform.GetChild(0).position, _pathPoints[_pathPointIndex].transform.position) > 3.0f)
            {
                _cameraBehaviour.SetTemporaryTarget(_pathPoints[_pathPointIndex], true, 3.0f, _cameraLookSpeed, _cameraPathTurnSpeed, _objectToFocusOn);
            }
            else
            {
                _pathPointIndex++;
            }
        }
        else
            StartStill();
    }

    private void BacktrackPath()
    {
        if (_pathPointIndex > 0)
        {
            if (Vector3.Distance(_camera.transform.GetChild(0).position, _pathPoints[_pathPointIndex].transform.position) > 3.0f)
            {
                _cameraBehaviour.SetTemporaryTarget(_pathPoints[_pathPointIndex], true, 3.0f, _cameraLookSpeed, _cameraPathTurnSpeed, _objectToFocusOn);
            }
            else
            {
                _pathPointIndex--;
            }
        }
        else
            Stop();
    }

    private void Still()
    {
        if (DateTime.Now > _startBacktrackTime)
        {
            StartBacktracking();
            _cameraState = CameraState.BacktrackingPath;
        }
    }
}
