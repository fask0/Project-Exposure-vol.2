using UnityEngine.UI;
using UnityEngine;

public class CleanUpTextureOnEnable : MonoBehaviour
{
    private void OnEnable()
    {
        if (gameObject.activeSelf)
            SingleTons.SoundWaveManager.ResetTexture(GetComponent<Image>().material);
    }
}
