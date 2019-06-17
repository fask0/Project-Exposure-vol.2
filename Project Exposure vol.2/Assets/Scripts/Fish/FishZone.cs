using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishZone : MonoBehaviour
{
    private List<GameObject> _fishToAvoid = new List<GameObject>();
    private List<FishBehaviour> _fishToAvoidBehaviours = new List<FishBehaviour>();
    private List<GameObject> _otherFish = new List<GameObject>();

    //School fish stuff
    private List<GameObject> _schoolFish = new List<GameObject>();
    private List<SchoolFishBehaviour> _schoolFishBehaviours = new List<SchoolFishBehaviour>();

    private List<GameObject> _leaders = new List<GameObject>();
    private List<SchoolFishLeaderBehaviour> _leaderBehaviours = new List<SchoolFishLeaderBehaviour>();
    private int _leaderIndex = 0;

    [SerializeField]
    private Vector3 ZoneTransform = new Vector3();
    [SerializeField]
    private bool DrawZoneCube = true;
    [SerializeField]
    private bool AlwaysDraw = false;

    private BoxCollider boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.FishManager.AddFishZone(this);

        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
    }

    public void AddFishToSchool(GameObject Fish, bool RandomFish = false, bool Leader = false)
    {
        if (!RandomFish)
        {
            if (Leader)
            {
                _leaderBehaviours.Add(Fish.GetComponent<SchoolFishLeaderBehaviour>());
                _leaders.Add(Fish);
                _schoolFish.Add(Fish);
            }
            else
            {
                _schoolFishBehaviours.Add(Fish.GetComponent<SchoolFishBehaviour>());
                _schoolFish.Add(Fish);
                _schoolFishBehaviours[_schoolFishBehaviours.Count - 1].SetSchoolFishLeader(_leaderBehaviours[_leaderIndex]);
                _leaderBehaviours[_leaderIndex]._schoolFishWithLeader.Add(Fish);
                _leaderBehaviours[_leaderIndex]._schoolFishWithLeaderBehaviours.Add(Fish.GetComponent<SchoolFishBehaviour>());
                _leaderIndex++;
                if (_leaderIndex >= _leaders.Count) { _leaderIndex = 0; }
            }
        }
        _fishToAvoid.Add(Fish);
        _fishToAvoidBehaviours.Add(Fish.GetComponent<FishBehaviour>());
    }

    public Vector3 GenerateNewCheckPoint(Vector3 fishPos)
    {
        Vector3 checkPoint = fishPos - transform.position;
        while (Vector3.Distance(checkPoint + transform.position, fishPos) < 10)
        {
            float randomX = UnityEngine.Random.Range(-Mathf.Abs(ZoneTransform.x) / 2, Mathf.Abs(ZoneTransform.x) / 2);
            float randomY = UnityEngine.Random.Range(-Mathf.Abs(ZoneTransform.y) / 2, Mathf.Abs(ZoneTransform.y) / 2);
            float randomZ = UnityEngine.Random.Range(-Mathf.Abs(ZoneTransform.z) / 2, Mathf.Abs(ZoneTransform.z) / 2);
            checkPoint = new Vector3(randomX, randomY, randomZ);
        }

        return (checkPoint + transform.position);
    }

    public List<GameObject> GetSchoolFish() { return _schoolFish; }
    public List<GameObject> GetFishToAvoid() { return _fishToAvoid; }
    public List<FishBehaviour> GetFishToAvoidBehaviours() { return _fishToAvoidBehaviours; }
    public List<SchoolFishBehaviour> GetSchoolFishBehaviours() { return _schoolFishBehaviours; }

    private void OnDrawGizmosSelected()
    {
        if (DrawZoneCube && !AlwaysDraw)
        {
            Gizmos.color = new Color(0, 1, 0.8f, 0.5f);
            Gizmos.DrawCube(transform.position, ZoneTransform);
        }
    }

    private void OnDrawGizmos()
    {
        if (AlwaysDraw)
        {
            Gizmos.color = new Color(0, 1, 0.8f, 0.5f);
            Gizmos.DrawCube(transform.position, ZoneTransform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach (SchoolFishBehaviour behaviour in _schoolFishBehaviours)
        {
            behaviour._isAvoiding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (SchoolFishBehaviour behaviour in _schoolFishBehaviours)
        {
            behaviour._isAvoiding = false;
        }
    }
}
