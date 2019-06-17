using UnityEngine;

public class SeaWeedContainer : MonoBehaviour
{
    private Renderer[] _renderers;

    // Start is called before the first frame update
    void Start()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player") return;
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material.SetVector("_PlayerPos", SingleTons.GameController.Player.transform.position);
        }
    }
}
