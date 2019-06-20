using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuCanvas : MonoBehaviour
{
    private GameObject _mainCanvas;
    private GameObject _mainCamera;
    private CameraBehaviour _cameraBehaviour;

    private List<GameObject> _menuScreens = new List<GameObject>();
    private MenuBehaviour _menuBehaviour;

    private DateTime _lastInput;

    // Start is called before the first frame update
    void Start()
    {
        //Populate menu list
        for (int i = 0; i < transform.childCount; i++)
        {
            _menuScreens.Add(transform.GetChild(i).gameObject);
        }

        _mainCanvas = GameObject.FindGameObjectWithTag("MainCanvas");
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _cameraBehaviour = _mainCamera.transform.parent.GetComponent<CameraBehaviour>();

        _menuBehaviour = _menuScreens[0].GetComponent<MenuBehaviour>();
        _lastInput = DateTime.Now.AddYears(10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            _lastInput = DateTime.Now;
        }
        if (DateTime.Now > _lastInput.AddSeconds(20))
        {
            EnableMenuScreen(_menuScreens[0]);
        }

        if (_mainCanvas == null) return;
        if (AnyMenusActive())
        {
            _mainCanvas.SetActive(false);
            _cameraBehaviour.SetTemporaryTarget(_menuBehaviour.GetCameraPoint());
        }
        else
        {
            _mainCanvas.SetActive(true);
            if (_cameraBehaviour.GetTarget() == _menuBehaviour.GetCameraPoint())
                _cameraBehaviour.SetToOriginalTarget();
        }
    }

    public void EnableMenuScreen(GameObject menu)
    {
        if (!_menuScreens.Contains(menu))
        {
            Debug.Log("GameObject " + menu.name + " is not a menu");
            return;
        }

        DisableAllMenus();
        menu.SetActive(true);
        _menuBehaviour = menu.GetComponent<MenuBehaviour>();
    }

    public void DisableAllMenus()
    {
        foreach (GameObject gameObject in _menuScreens)
        {
            gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        Destroy(Camera.main.transform.parent.gameObject);
        SceneManager.LoadScene("MainGameScene", LoadSceneMode.Additive);
    }

    private bool AnyMenusActive()
    {
        foreach (GameObject gameObject in _menuScreens)
        {
            if (gameObject.activeSelf)
            {
                return true;
            }
        }
        return false;
    }
}
