using UnityEngine;
using UnityEngine.UI;

public class AssignMainCamera : MonoBehaviour
{
    private Canvas _canvas;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas.worldCamera == null)
        {
            _canvas.worldCamera = Camera.main;
            _canvas.planeDistance = 0.3f;
        }
    }
}
