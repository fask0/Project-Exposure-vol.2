using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private List<string> _loadScenes;
    [SerializeField] private List<string> _unloadScenes;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.gameObject.layer != 8) return;

        foreach (string name in _loadScenes)
            if (name != "")
                SingleTons.GameController.Load(name);

        if (_unloadScenes.Count > 0)
            StartCoroutine("UnloadScenes");
    }

    private IEnumerator UnloadScenes()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (string name in _unloadScenes)
            if (name != "")
                SingleTons.GameController.Unload(name);
    }
}
