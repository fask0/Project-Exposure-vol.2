using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform, Vector3.up);
    }
}
