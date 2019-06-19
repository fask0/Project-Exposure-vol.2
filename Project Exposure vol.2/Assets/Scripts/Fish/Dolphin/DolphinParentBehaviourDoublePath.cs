using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinParentBehaviourDoublePath : MonoBehaviour
{
    [SerializeField]
    private FishZone _endingRandomZone;
    [SerializeField]
    private LineRenderer _guidingPath;
    [SerializeField]
    private LineRenderer _startGuidingPath;
    [SerializeField]
    private DolphinBehaviourDoublePath _dolphinBehaviour;

    public enum DolphinState
    {
        StartPathBehaviour,
        PathingBehaviour,
        RandomEndBehaviour
    };

    // Start is called before the first frame update
    void Start()
    {
        if (_endingRandomZone == null ||
            _guidingPath == null ||
            _startGuidingPath == null ||
            _dolphinBehaviour == null)
        {
            Debug.Log("All inspector variables of the DolphinParentBehaviour need to be filled in");
            enabled = false;
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public LineRenderer GetStartPathBehaviour()
    {
        return _startGuidingPath;
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
