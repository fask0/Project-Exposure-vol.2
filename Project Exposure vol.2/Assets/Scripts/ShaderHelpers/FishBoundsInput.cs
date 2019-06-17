using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBoundsInput : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float _multiplier = 1;
    [SerializeField]
    private bool _hasAnimation;

    new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        if (!_hasAnimation)
            renderer = GetComponent<Renderer>();
        else
            renderer = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();

        float length;
        float rand;
        float speed;
        length = Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.z, renderer.bounds.size.y);
        rand = SingleTons.GameController.GetRandomRange(0, 1);
        speed = transform.parent.GetComponent<FishBehaviour>().GetMinSpeed();
        foreach (Material material in renderer.materials)
        {
            material.SetFloat("_FishLength", (length / 2) / transform.localScale.x);

            material.SetFloat("_Offset", rand);

            material.SetFloat("_WobbleSpeed", speed * 100 * _multiplier);
        }
    }
}
