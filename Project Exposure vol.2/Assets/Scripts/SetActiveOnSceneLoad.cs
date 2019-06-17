using UnityEngine;

public class SetActiveOnSceneLoad : MonoBehaviour
{
    [SerializeField] private string _enableSceneName;
    [SerializeField] private string _disableSceneName;

    void Start()
    {
        SingleTons.GameController.onAllSceneLoadEvent += OnSceneLoad;
    }

    private void OnSceneLoad(string pName)
    {
        if (_enableSceneName == pName)
            gameObject.SetActive(true);
        else if (_disableSceneName == pName)
            gameObject.SetActive(false);
    }
}
