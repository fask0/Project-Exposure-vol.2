using UnityEngine.PostProcessing;
using UnityEngine;

public class SetPostprocessingValues : MonoBehaviour
{
    void Start()
    {
        FogEffect camFg = Camera.main.GetComponent<FogEffect>();
        FogEffect disFg = GetComponent<FogEffect>();
        camFg.mat = disFg.mat;
        camFg._fogColor = disFg._fogColor;
        camFg._silhouetteColor = disFg._silhouetteColor;
        camFg._depthStart = disFg._depthStart;
        camFg._depthDistance = disFg._depthDistance;
        camFg._depthFadeDistance = disFg._depthFadeDistance;
        camFg._fogBeforeFadeMultiplier = disFg._fogBeforeFadeMultiplier;
        camFg._fogStrength = disFg._fogStrength;

        Camera.main.GetComponent<PostProcessingBehaviour>().profile = GetComponent<PostProcessingBehaviour>().profile;
    }
}
