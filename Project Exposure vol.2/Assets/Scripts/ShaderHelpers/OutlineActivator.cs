using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineActivator : MonoBehaviour
{
    [SerializeField]
    [Range(0.0f, 2.0f)]
    private float _maxOutlineWidth = 1.05f;

    private Material[] _materials;

    private bool _showOutline = true;
    private Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _materials = _renderer.materials;

        SingleTons.SoundWaveManager.onFishScanEvent += ScanEvent;
    }

    private void ScanEvent(GameObject pGameObject)
    {
        if (pGameObject.name == gameObject.name)
        {
            _showOutline = false;
            SetOutlineWidth(0.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_showOutline) return;

        if (other.tag == "Player")
        {
            SetOutlineWidth(_maxOutlineWidth);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            SetOutlineWidth(0.0f);
        }
    }

    private void SetOutlineWidth(float _width)
    {
        foreach (Material mat in _materials)
        {
            mat.SetFloat("_OutlineWidth", _width);
        }
    }
}
