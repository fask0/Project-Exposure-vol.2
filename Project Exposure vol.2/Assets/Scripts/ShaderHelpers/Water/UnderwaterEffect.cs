using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class UnderwaterEffect : MonoBehaviour
{
    public Material mat;

    [Range(0.001f, 0.1f)]
    public float _pixelOffset;
    [Range(0.1f, 20.0f)]
    public float _noiseScale;
    [Range(0.1f, 20.0f)]
    public float _noiseFrequency;
    [Range(0.1f, 30.0f)]
    public float _noiseSpeed;

    public float _depthStart;
    public float _depthDistance;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_NoiseFrequency", _noiseFrequency);
        mat.SetFloat("_NoiseScale", _noiseScale);
        mat.SetFloat("_NoiseSpeed", _noiseSpeed);
        mat.SetFloat("_PixelOffset", _pixelOffset);

        mat.SetFloat("_DepthStart", _depthStart);
        mat.SetFloat("_DepthDistance", _depthDistance);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mat);
    }
}
