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
        while (_radarArrowTime.Count < transform.childCount)
            _radarArrowTime.Add(120);
        while (_radarArrowTime.Count > transform.childCount)
            _radarArrowTime.RemoveAt(_radarArrowTime.Count - 1);
    }

    public int GetArrowTime(int index)
    {
        return _radarArrowTime[index];
    }
}
