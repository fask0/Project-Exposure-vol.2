using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HighlightUIElement : MonoBehaviour
{
    [SerializeField]
    private int _timeInMs = 3000;

    [Header("Blinking variables")]
    [SerializeField]
    private Image _blinkImage;
    [SerializeField]
    private int _blinkAmount = 3;
    [SerializeField]
    [Range(0, 1)]
    private float _maxOpacity = 0.5f;

    [Header("Scale variables")]
    [SerializeField]
    [Range(0, 10)]
    private float _scaleSize = 2;
    [SerializeField]
    private float _scaleSpeed = 2;
    [SerializeField]
    private List<RectTransform> _otherObjectsToScale = new List<RectTransform>();

    [Header("Note: smoother in-game")]
    [SerializeField]
    private bool _start = false;

    private bool _isHighlighting = false;
    private DateTime _stopHighlightingTime;

    private RectTransform _rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        _blinkImage.color = new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, 0);
        _rectTransform = GetComponent<RectTransform>();

        _start = false;
        _isHighlighting = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_start)
        {
            _start = false;
            StartHighlight();
        }

        if (_isHighlighting)
        {
            Blinking();

            if (DateTime.Now > _stopHighlightingTime)
            {
                ResetHighlight();
            }
        }
        Scaling();
    }


    public void StartHighlight()
    {
        _isHighlighting = true;
        _stopHighlightingTime = DateTime.Now.AddMilliseconds(_timeInMs);
    }

    public void ResetHighlight()
    {
        _isHighlighting = false;
        _blinkImage.color = new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, 0);
    }

    public void Blinking()
    {
        float _blinkDivider = _timeInMs / _blinkAmount;

        float _timeInBlink = (float)(_stopHighlightingTime - DateTime.Now).TotalMilliseconds;
        while (_timeInBlink > _blinkDivider)
        {
            _timeInBlink -= _blinkDivider;
        }

        float perc = (1 - (_timeInBlink / _blinkDivider)) * 2;

        if (perc <= 1)
            _blinkImage.color = Color.Lerp(new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, 0), new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, _maxOpacity), perc);
        else
            _blinkImage.color = Color.Lerp(new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, 0), new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, _maxOpacity), 1 - (perc - 1));
    }

    public void Scaling()
    {
        Vector3 newScale = _rectTransform.localScale;
        if (_isHighlighting)
        {
            newScale = Vector3.Lerp(_rectTransform.localScale, Vector3.one * _scaleSize, Time.deltaTime * _scaleSpeed);
        }
        else
        {
            newScale = Vector3.Lerp(_rectTransform.localScale, Vector3.one, Time.deltaTime * _scaleSpeed);
        }
        _rectTransform.localScale = newScale;
        foreach (RectTransform rect in _otherObjectsToScale)
        {
            rect.localScale = newScale;
        }
    }
}
