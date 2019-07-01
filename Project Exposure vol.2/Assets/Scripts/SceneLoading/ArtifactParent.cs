using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ArtifactParent : MonoBehaviour
{
    [SerializeField]
    private List<int> _radarArrowTime = new List<int>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_radarArrowTime.Count == 0) return;

        while (_radarArrowTime.Count < GetComponentsInChildren<AudioSource>().Length)
            _radarArrowTime.Add(120);
        while (_radarArrowTime.Count > GetComponentsInChildren<AudioSource>().Length)
            _radarArrowTime.RemoveAt(_radarArrowTime.Count - 1);
    }

    public int GetArrowTime(int index)
    {
        if (index >= _radarArrowTime.Count) return int.MaxValue;
        return _radarArrowTime[index];
    }

    public List<int> GetRadarTimesList()
    {
        return _radarArrowTime;
    }
}
