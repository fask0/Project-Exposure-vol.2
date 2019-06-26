using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AuraAPI;

public class LightFlickering : MonoBehaviour
{
    private int _timeInbetweenFlickering = 10000;
    private int _flickeringChance = 3; // 3 out of 100
    private int _stayFlickeringTimeInMs = 2000;
    private bool _bothLights = false;
    private int _lightIndex = 0;

    private DateTime _startFlickeringTime;
    private DateTime _flickeringTime = DateTime.MinValue;
    private Light[] _lights;
    private AuraLight[] _auraLights;

    private float _startStrength;

    // Start is called before the first frame update
    void Start()
    {
        _startFlickeringTime = DateTime.Now.AddMilliseconds(_timeInbetweenFlickering);
        _lights = transform.GetComponentsInChildren<Light>();
        _auraLights = transform.GetComponentsInChildren<AuraLight>();

        if (!_bothLights)
        {
            _lightIndex = Mathf.FloorToInt(SingleTons.GameController.GetRandomRange(0, 1.9f));
        }

        //Generate new flickering chance
        _flickeringChance = Mathf.RoundToInt(SingleTons.GameController.GetRandomRange(0, 15));
        _startStrength = _auraLights[0].strength;
    }

    // Update is called once per frame
    void Update()
    {
        if (_bothLights)
            BothLightFLickering();
        else
            SingleLightFlickering();
    }

    private void BothLightFLickering()
    {
        //Start flickering
        if (DateTime.Now > _startFlickeringTime)
        {
            if (_flickeringTime < _startFlickeringTime)
            {
                //Set stop flickering time
                _flickeringTime = DateTime.Now.AddMilliseconds(_stayFlickeringTimeInMs + SingleTons.GameController.GetRandomRange(0, _stayFlickeringTimeInMs));
            }
            else
            {
                if (DateTime.Now > _flickeringTime)
                {
                    //Stop flickering
                    _startFlickeringTime = DateTime.Now.AddMilliseconds(SingleTons.GameController.GetRandomRange(0, _timeInbetweenFlickering));

                    //Randomly choose what light to flicker next
                    if (SingleTons.GameController.GetRandomRange(0, 100) > 50)
                        _lightIndex = 0;
                    else
                        _lightIndex = 1;

                    //Generate new flickering chance
                    _flickeringChance = Mathf.RoundToInt(SingleTons.GameController.GetRandomRange(0, 15));
                }
                else
                {
                    //Flicker light
                    if (SingleTons.GameController.GetRandomRange(0, 100) > 100 - _flickeringChance)
                    {
                        if (_lights[_lightIndex].enabled)
                        {
                            _lights[_lightIndex].enabled = false;
                            _lights[_lightIndex + 2].enabled = false;
                            //_auraLights[_lightIndex].enabled = false;
                            _auraLights[_lightIndex].strength = 0;
                        }
                        else
                        {
                            _lights[_lightIndex].enabled = true;
                            _lights[_lightIndex + 2].enabled = true;
                            //_auraLights[_lightIndex].enabled = true;
                            _auraLights[_lightIndex].strength = _startStrength;
                        }
                    }
                }
            }
        }
    }

    private void SingleLightFlickering()
    {
        //Start flickering
        if (DateTime.Now > _startFlickeringTime)
        {
            if (_flickeringTime < _startFlickeringTime)
            {
                //Set stop flickering time
                _flickeringTime = DateTime.Now.AddMilliseconds(_stayFlickeringTimeInMs + SingleTons.GameController.GetRandomRange(0, _stayFlickeringTimeInMs));
            }
            else
            {
                if (DateTime.Now > _flickeringTime)
                {
                    //Stop flickering
                    _startFlickeringTime = DateTime.Now.AddMilliseconds(SingleTons.GameController.GetRandomRange(0, _timeInbetweenFlickering));

                    //Generate new flickering chance
                    _flickeringChance = Mathf.RoundToInt(SingleTons.GameController.GetRandomRange(0, 15));
                }
                else
                {
                    //Flicker light
                    if (SingleTons.GameController.GetRandomRange(0, 100) > 100 - _flickeringChance)
                    {
                        if (_lights[_lightIndex].enabled)
                        {
                            _lights[_lightIndex].enabled = false;
                            _lights[_lightIndex + 2].enabled = false;
                            //_auraLights[_lightIndex].enabled = false;
                            _auraLights[_lightIndex].strength = 0;
                        }
                        else
                        {
                            _lights[_lightIndex].enabled = true;
                            _lights[_lightIndex + 2].enabled = true;
                            //_auraLights[_lightIndex].enabled = true;
                            _auraLights[_lightIndex].strength = _startStrength;
                        }
                    }
                }
            }
        }
    }
}
