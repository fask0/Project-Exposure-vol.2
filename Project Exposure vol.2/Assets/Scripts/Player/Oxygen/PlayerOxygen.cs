using System;
using UnityEngine.UI;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField]
    private float _maximumOxygen;
    [SerializeField]
    private float _oxygenDrainRate;
    [SerializeField]
    private float _surviveTimeWithoutOxygen = 20.0f;

    private float _currentOxygen;
    private float _timeWithoutOxygen;

    private bool _oxygenDrainIsPaused = false;
    private DateTime _resumeTime;

    private Image _oxygenBarFill;

    // Start is called before the first frame update
    void Start()
    {
        _currentOxygen = _maximumOxygen;

        _oxygenBarFill = Camera.main.transform.GetChild(0).GetChild(8).GetChild(0).GetChild(1).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_oxygenDrainIsPaused)
        {
            _currentOxygen -= _oxygenDrainRate * Time.deltaTime;

            UpdateOxygen();
            CheckIfDed();
        }
        else
        {
            if (DateTime.Now >= _resumeTime)
            {
                _oxygenDrainIsPaused = false;
            }
        }
    }

    private void CheckIfDed()
    {
        if (_currentOxygen <= 0)
        {
            _timeWithoutOxygen += Time.deltaTime;
            if (_timeWithoutOxygen >= _surviveTimeWithoutOxygen)
            {
                //DO the ded stuff
            }
        }
    }

    private void UpdateOxygen()
    {
        float percentage = _currentOxygen / _maximumOxygen;
        _oxygenBarFill.fillAmount = percentage;
    }

    public void PauseOxygenDrain(int pauseTimeInMs)
    {
        _resumeTime = DateTime.Now.AddMilliseconds(pauseTimeInMs);
        _oxygenDrainIsPaused = true;
    }

    public void DrainOxygen(int oxygenAmount)
    {
        _currentOxygen -= oxygenAmount;
    }
}
