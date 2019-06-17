using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwarmableArea : MonoBehaviour
{
    public enum SwarmFormation
    {
        Sphere,
        Donut
    };
    [SerializeField]
    private SwarmFormation _swarmFormation;

    [SerializeField]
    [Range(0.1f, 20)]
    private float _minSwarmDistance = 1;
    [SerializeField]
    [Range(0.1f, 20)]
    private float _maxSwarmDistance = 2;

    [SerializeField]
    [Range(0.1f, 20)]
    private float _swarmHeight = 1;

    [SerializeField]
    [Range(0.1f, 100)]
    private float _swarmSpeed = 1;

    private List<GameObject> _swarmingFish = new List<GameObject>();
    private bool _hasSwarmedBefore = false;

    public void SwarmArea(List<GameObject> fish)
    {
        //if (!_hasSwarmedBefore)
        //{
        _swarmingFish = fish;
        ToggleBehaviours(true);
        _hasSwarmedBefore = true;
        //}
    }

    public void SwarmArea(FishZone fishZone)
    {
        //if (!_hasSwarmedBefore)
        //{
        _swarmingFish = fishZone.GetSchoolFish();
        ToggleBehaviours(true);
        _hasSwarmedBefore = true;
        //}
    }

    public void StopSwarming()
    {
        ToggleBehaviours(false);
        _swarmingFish = new List<GameObject>();
    }

    private void ToggleBehaviours(bool swarming)
    {
        if (swarming)
        {
            foreach (GameObject fish in _swarmingFish)
            {
                fish.GetComponent<FishBehaviour>().enabled = false;
                if (fish.GetComponent<SwarmAreaBehaviour>() == null)
                {
                    fish.AddComponent<SwarmAreaBehaviour>();
                }
                fish.GetComponent<SwarmAreaBehaviour>().SetSwarmArea(this);
                fish.GetComponent<SwarmAreaBehaviour>().enabled = true;
            }
        }
        else
        {
            foreach (GameObject fish in _swarmingFish)
            {
                fish.GetComponent<SwarmAreaBehaviour>().Reset();
                fish.GetComponent<SwarmAreaBehaviour>().enabled = false;
                fish.GetComponent<FishBehaviour>().enabled = true;
            }
        }
    }

    public float GetSwarmSpeed()
    {
        return _swarmSpeed;
    }

    public float GetSwarmDistance()
    {
        return SingleTons.GameController.GetRandomRange(_minSwarmDistance, _maxSwarmDistance);
    }

    public float GetRandomY()
    {
        return transform.position.y + (SingleTons.GameController.GetRandomRange(0, _swarmHeight) - _swarmHeight / 2);
    }

    public SwarmFormation GetSwarmFormation()
    {
        return _swarmFormation;
    }

    private void OnDrawGizmosSelected()
    {
        switch (_swarmFormation)
        {
            case SwarmFormation.Sphere:
                Gizmos.color = new Color(1.0f, 0.7f, 0.3f, 0.5f);
                Gizmos.DrawSphere(transform.position, _minSwarmDistance);

                Gizmos.color = new Color(0.6f, 0, 0.6f, 0.5f);
                Gizmos.DrawSphere(transform.position, _maxSwarmDistance);
                break;
            case SwarmFormation.Donut:
                Gizmos.color = new Color(1.0f, 0.7f, 0.3f, 0.5f);
                Gizmos.DrawCube(transform.position, new Vector3(_minSwarmDistance * 2, _swarmHeight, _minSwarmDistance * 2));

                Gizmos.color = new Color(0.6f, 0, 0.6f, 0.5f);
                Gizmos.DrawCube(transform.position, new Vector3(_maxSwarmDistance * 2, _swarmHeight, _maxSwarmDistance * 2));
                break;
        }
    }
}
