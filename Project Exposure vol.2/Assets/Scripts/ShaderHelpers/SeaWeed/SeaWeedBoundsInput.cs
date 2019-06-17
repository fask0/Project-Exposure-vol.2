using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWeedBoundsInput : MonoBehaviour
{
    private Renderer _renderer;
    private int _myVectorId;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();

        float rand = SingleTons.GameController.GetRandomRange(1, 1000);
        _renderer.material.SetFloat("_Offset", rand);


        _renderer.material.SetFloat("_HighestY", _renderer.bounds.center.y + _renderer.bounds.extents.y);
        _renderer.material.SetFloat("_LowestY", _renderer.bounds.center.y - _renderer.bounds.extents.y);

        _myVectorId = Shader.PropertyToID("_PlayerPos");
    }
}
