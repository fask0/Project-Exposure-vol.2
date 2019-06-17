using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blinking : MonoBehaviour
{
    [SerializeField]
    private Image _blinkImage;
    [SerializeField]
    private int _blinkAmount = 3;
    [SerializeField]
    private int _blinkTimeInMs = 3000;
    [SerializeField]
    [Range(0, 1)]
    private float _maxOpacity = 0.5f;

    [SerializeField]
    private bool _startBlinking = false;

    private bool _isBlinking;
    private DateTime _stopBlinkingTime;

    // Start is called before the first frame update
    void Start()
    {
        _blinkImage.color = new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (_startBlinking)
        {
            _startBlinking = false;
            StartBlinking();
        }

        if (_isBlinking)
        {
            float _blinkDivider = _blinkTimeInMs / _blinkAmount;

            float _timeInBlink = (float)(_stopBlinkingTime - DateTime.Now).TotalMilliseconds;
            while (_timeInBlink > _blinkDivider)
            {
                _timeInBlink -= _blinkDivider;
            }

            float perc = (1 - (_timeInBlink / _blinkDivider)) * 2;

            if (perc <= 1)
                _blinkImage.color = Color.Lerp(new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, 0), new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, _maxOpacity), perc);
            else
                _blinkImage.color = Color.Lerp(new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, 0), new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, _maxOpacity), 1 - (perc - 1));


            if (DateTime.Now > _stopBlinkingTime)
            {
                _isBlinking = false;
                _blinkImage.color = new Color(_blinkImage.color.r, _blinkImage.color.g, _blinkImage.color.b, 0);
            }
        }
    }

    public void StartBlinking()
    {
        _isBlinking = true;
        _stopBlinkingTime = DateTime.Now.AddMilliseconds(_blinkTimeInMs);
    }
}
