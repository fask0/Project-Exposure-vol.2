using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DolphinPathingStartActivator : MonoBehaviour
{
    [SerializeField]
    private float _radius = 5;

    public float GetRadius()
    {
        return _radius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
