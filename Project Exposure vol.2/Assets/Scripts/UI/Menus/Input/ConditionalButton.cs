using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConditionalButton : MonoBehaviour
{
    [SerializeField]
    private GameObject _button;
    [SerializeField]
    private Sprite _imageToSwapTo;

    private TMP_InputField _inputField;
    private Sprite _originalImage;
    private Image _imageComponent;
    private Button _buttonbehaviour;

    // Start is called before the first frame update
    void Start()
    {
        _inputField = GetComponent<TMP_InputField>();
        _imageComponent = _button.GetComponent<Image>();
        _originalImage = _imageComponent.sprite;
        _buttonbehaviour = _button.GetComponent<Button>();

        _imageComponent.sprite = _imageToSwapTo;
        _buttonbehaviour.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        string testString = _inputField.text.Replace(" ", string.Empty);
        if (testString.Length > 0)
        {
            _imageComponent.sprite = _originalImage;
            _buttonbehaviour.enabled = true;
        }
        else
        {
            _imageComponent.sprite = _imageToSwapTo;
            _buttonbehaviour.enabled = false;
        }
    }
}
