using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [SerializeField] private float _minDistance = 1.0f;
    [SerializeField] private float _maxDistance = 4.0f;
    [SerializeField] private float _smoothing = 10.0f;

    private Vector3 _dollyDir;
    private Vector3 _dollyDirAdjust;
    private float _distance;

    private LayerMask _ignoreLayer;

    void Awake()
    {
        _dollyDir = transform.localPosition.normalized;
        _distance = transform.localPosition.magnitude;
        transform.localPosition = _dollyDir * _distance;
    }

    void Update()
    {
        Vector3 newCameraPos = transform.parent.TransformPoint(_dollyDir * _maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, newCameraPos, out hit))
        {
            if (!hit.collider.isTrigger && hit.transform.tag != "Player")
            {
                _distance = Mathf.Clamp(hit.distance * 0.85f, _minDistance, _maxDistance);
            }
        }
        else
        {
            _distance = _maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, _dollyDir * _distance, _smoothing * Time.deltaTime);
    }
}
