using cakeslice;
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
    private Outline _outline;
    private bool _canShowAllCreatureOutlines = false;

    // Start is called before the first frame update
    void Start()
    {
        _outline = GetComponent<Outline>();
        _renderer = GetComponent<Renderer>();
        _materials = _renderer.materials;

        SingleTons.SoundWaveManager.onFishScanEvent += ScanEvent;
        SingleTons.CollectionsManager.onDolphinScanEvent += EnableOutlines;
        _outline.enabled = false;
    }

    private void OnDisable()
    {
        SingleTons.SoundWaveManager.onFishScanEvent -= ScanEvent;
        SingleTons.CollectionsManager.onDolphinScanEvent -= EnableOutlines;
    }

    private void ScanEvent(GameObject pGameObject)
    {
        if (pGameObject.name == gameObject.name)
        {
            _showOutline = false;
            SetOutlineWidth(0.0f);

            if (_outline != null)
            {
                _outline.enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_showOutline) return;

        if (other.tag == "Player")
        {
            if (!_canShowAllCreatureOutlines && gameObject.name != "Dolphin") return;
            SetOutlineWidth(_maxOutlineWidth);

            if (_outline != null)
            {
                _outline.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            SetOutlineWidth(0.0f);

            if (_outline != null)
            {
                _outline.enabled = false;
            }
        }
    }

    private void SetOutlineWidth(float _width)
    {
        foreach (Material mat in _materials)
        {
            mat.SetFloat("_OutlineWidth", _width);
        }

        if (_outline != null)
        {
            if (_width > 0.01f)
                _outline.enabled = true;
            else
                _outline.enabled = false;
        }
    }

    public void EnableOutlines()
    {
        _canShowAllCreatureOutlines = true;
    }
}
