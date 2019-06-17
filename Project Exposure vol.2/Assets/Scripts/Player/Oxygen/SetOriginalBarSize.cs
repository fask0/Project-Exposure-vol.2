using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOriginalBarSize : MonoBehaviour
{
    private RectTransform _parentRectTransform;
    private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        _parentRectTransform = transform.parent.GetComponent<RectTransform>();
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        _rectTransform.offsetMax = new Vector2(_rectTransform.offsetMax.x, 1.0f + (1.0f - _parentRectTransform.offsetMax.y));
    }
}
