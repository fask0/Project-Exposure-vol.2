using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreTextUpdater : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;

    private float _currentScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _currentScore = Mathf.Lerp(_currentScore, (float)SingleTons.ScoreManager.GetScore() + 0.85f, Time.deltaTime * 5);
        _textMesh.text = ((int)_currentScore).ToString();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SingleTons.ScoreManager.AddScore(200);
        }
    }
}
