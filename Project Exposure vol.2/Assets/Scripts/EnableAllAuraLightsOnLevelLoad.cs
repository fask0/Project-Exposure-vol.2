using AuraAPI;
using UnityEngine;

public class EnableAllAuraLightsOnLevelLoad : MonoBehaviour
{
    void Start()
    {
        SingleTons.GameController.onSceneLoadEvent += OnLastSceneLoad;

        AuraLight[] lights = GameObject.FindObjectsOfType<AuraLight>();
        for (int i = 0; i < lights.Length; i++)
            lights[i].enabled = true;
    }

    public void OnLastSceneLoad(string pName)
    {
        AuraLight[] lights = GameObject.FindObjectsOfType<AuraLight>();
        for (int i = 0; i < lights.Length; i++)
            lights[i].enabled = true;
    }

    private void OnDisable()
    {
        SingleTons.GameController.onSceneLoadEvent -= OnLastSceneLoad;
    }
}
