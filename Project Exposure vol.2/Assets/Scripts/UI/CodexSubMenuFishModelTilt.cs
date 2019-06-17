using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class CodexSubMenuFishModelTilt : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Image _relativePosition;
    private Image _overlay;
    private Vector3 _inputVector;

    void Start()
    {
        _relativePosition = GetComponent<Image>();
        _overlay = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_overlay.rectTransform,
                                                                    eventData.position,
                                                                    eventData.pressEventCamera,
                                                                    out position))
        {
            position.x = position.x / (_overlay.rectTransform.sizeDelta.x * 0.5f);
            position.y = position.y / (_overlay.rectTransform.sizeDelta.y * 0.5f);

            _inputVector = new Vector3(position.x, position.y, 0);
            _inputVector = (_inputVector.magnitude > 1) ? _inputVector.normalized : _inputVector;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 position;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_relativePosition.rectTransform,
                                                                 eventData.position,
                                                                 eventData.pressEventCamera,
                                                                 out position))
        {
            _overlay.transform.localPosition = position;
        }
    }

    public float Vertical()
    {
        return _inputVector.y;
    }
}
