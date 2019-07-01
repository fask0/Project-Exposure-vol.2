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
         "Laura Omloop",
         "Billy Herington",
         "Joe Rogen",
         "Ben Shapiro",
         "Pewdiepie",
         "Mr. Beast",
         "Joost Posthuma",
         "Jeff",
         "José",
        "Firstname Lastname",
        "Tim van der Kaap",
        "Kasper Roosen",
        "Tim Frank",
        "Iris van der Brand",
        "Jarvis 3.1",
        "Justin Mensink",
        "Amber Brands",
        "Martijn Boros",
        "Mark Oosthof",
        "Rebecca van Stralendorff",
        "Tristan Kuijer",
        "Daan Herwijnen",
        "Bas Wieling",
        "Diederik Hagemeier",
        "Bram Fleur",
        "Christian Kemps",
        "Bart Dijs",
        "Annemiek de Aldrey",
        "Dominique Kamping",
        "Deuwe de Natris",
        "Fynn Gerben",
        "Captain Obvious",
        "Harry Leine",
        "Hendrik Polman",
        "Helena Harrison",
        "Job Vieler",
        "SsSniperwolf",
        "Linda Picker",
        "Lisanne van der Werf",
        "Gloria Borger",
        "Poppy Harlow",
        "Kimi Keemstar",
        "Dude Mannson",
        "Not White Power Ranger",
        "Leonardo Rafaello",
        "Donatelo Mikelangelo",
        "Strongest Avenger",
        "Roe Joegan",
        "Bro Hogan",
        "Steve van der Minecraft",
        "Alex van Oranje",
        "Shaggy Rogers",
        "Ben Shapiro",
        "Skeletor Boneman",
        "Mr. Beast",
        "Pewdiepie UwU",
        "Belle Delphine",
        "Smurf Account",
        "She Woman",
        "He Man",
        "Rick Harrison",
        "Tai Lopez"};

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

    private void OnDisable()
    {
        _inputField.text = string.Empty;
    }
}
