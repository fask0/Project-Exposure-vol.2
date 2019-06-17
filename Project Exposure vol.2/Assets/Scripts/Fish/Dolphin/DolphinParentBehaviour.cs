using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinParentBehaviour : MonoBehaviour
{
    [SerializeField]
    private FishZone _startingRandomZone;
    [SerializeField]
    private FishZone _endingRandomZone;
    [SerializeField]
    private LineRenderer _guidingPath;
    [SerializeField]
    private DolphinPathingStartActivator _startPathingBehaviour;
    [SerializeField]
    private DolphinBehaviour _dolphinBehaviour;

    public enum DolphinState
    {
        RandomStartingBehaviour,
        PathingBehaviour,
        RandomEndBehaviour
    };

    // Start is called before the first frame update
    void Start()
    {
        if (_startingRandomZone == null ||
            _endingRandomZone == null ||
            _guidingPath == null ||
            _startPathingBehaviour == null ||
            _dolphinBehaviour == null)
        {
            Debug.Log("All inspector variables of the DolphinParentBehaviour need to be filled in");
            enabled = false;
            return;
        }

        if (_dolphinBehaviour.GetDolphinState() == DolphinState.RandomStartingBehaviour)
        {
            _dolphinBehaviour.transform.position = _startingRandomZone.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_dolphinBehaviour.GetDolphinState() == DolphinState.RandomStartingBehaviour)
        {
            if (Vector3.Distance(SingleTons.GameController.Player.transform.position, _startPathingBehaviour.transform.position) < _startPathingBehaviour.GetRadius())
            {
                _dolphinBehaviour.SetDolphinState(DolphinState.PathingBehaviour);
            }
        }
    }

    public FishZone GetStartingFishZone()
    {
        return _startingRandomZone;
    }

    public FishZone GetEndingFishZone()
    {
        return _endingRandomZone;
    }

    public LineRenderer GetGuidingPath()
    {
        return _guidingPath;
    }
}
