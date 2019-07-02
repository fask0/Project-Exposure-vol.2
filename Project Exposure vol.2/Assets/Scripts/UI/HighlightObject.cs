using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightObject : MonoBehaviour
{
    [SerializeField]
    private int _timeInMs = 3000;

    [Header("Blinking variables")]
    [SerializeField]
    private int _blinkAmount = 3;
    [SerializeField]
    [Range(0, 1)]
    private float _maxOpacity = 0.5f;

    [Header("Note: smoother in-game")]
    [SerializeField]
    private bool _start = false;

    private bool _isHighlighting = false;
    private DateTime _stopHighlightingTime;

    private RectTransform _rectTransform;
    private Renderer _renderer;

    private Color _defaultMaterialColor;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _renderer = GetComponent<Renderer>();

        _defaultMaterialColor = _renderer.material.GetColor("_ScanInbetweenColor");

        _start = false;
        _isHighlighting = false;
    }

    // Update is called once per frame
    void LateUpdate()
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
    }


    public void StartHighlight()
    {
        _isHighlighting = true;
        _stopHighlightingTime = DateTime.Now.AddMilliseconds(_timeInMs);

        foreach (Renderer renderer in GetComponents<Renderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.SetColor("_ScanInbetweenColor", new Color(0.5f, 0.5f, 0.5f));
                material.SetFloat("_IsScanning", 1);
                material.SetFloat("_ScanLines", 0);
                material.SetFloat("_ScanLineWidth", 0);
            }
        }
    }

    public void ResetHighlight()
    {
        _isHighlighting = false;

        foreach (Renderer renderer in GetComponents<Renderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.SetColor("_ScanInbetweenColor", _defaultMaterialColor);
                material.SetFloat("_IsScanning", 0);
                material.SetFloat("_ScanLines", 1);
                material.SetFloat("_ScanLineWidth", 2);
            }
        }
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
        {
            foreach (Renderer renderer in GetComponents<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    material.SetFloat("_ScanLines", Mathf.Max(perc * 100, 1));
                    material.SetFloat("_IsScanning", 1);
                    material.SetFloat("_ScanLineWidth", 0);
                }
            }
        }
        else
        {
            foreach (Renderer renderer in GetComponents<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    material.SetFloat("_ScanLines", Mathf.Max((1 - (perc - 1)) * 100, 1));
                    material.SetFloat("_IsScanning", 1);
                    material.SetFloat("_ScanLineWidth", 0);
                }
            }
        }
    }
}
