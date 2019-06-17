using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    private TMP_InputField _inputField;
    private MenuCanvas _menuCanvas;

    private string[] _randomNames = new string[]
        {"Rimme",
         "Vasil",
         "Alex",
         "Micheal",
         "Elitsa",
         "Emilija",
         "Bas",
         "Skeletor",
         "Keemstar",
         "LauraOmloop",
         "BillyHerington",
         "JoeRogen",
         "BenShapiro",
         "Pewdiepie",
         "MrBeast",
         "JoostPosthuma",
         "Jeff",
         "José"};

    // Start is called before the first frame update
    void Start()
    {
        _inputField = GetComponent<TMP_InputField>();
        _menuCanvas = gameObject.transform.parent.parent.GetComponent<MenuCanvas>();
    }

    public void KeyPress(string key)
    {
        if (_inputField.text.Length < 16)
            _inputField.text += key;
    }

    public void RemoveLetter()
    {
        if (_inputField.text.Length > 0)
            _inputField.text = _inputField.text.Substring(0, _inputField.text.Length - 1);
    }

    public void RandomName()
    {
        _inputField.text = _randomNames[Random.Range(0, _randomNames.Length)];
    }

    public void SetName()
    {
        StartCoroutine("setName");
    }

    public IEnumerator setName()
    {
        while (SingleTons.ScoreManager == null)
            yield return new WaitForSeconds(0.001f);
        SingleTons.ScoreManager.SetName(_inputField.text);
        _menuCanvas.DisableAllMenus();
    }
}
