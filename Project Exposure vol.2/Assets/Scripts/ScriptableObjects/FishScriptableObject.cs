using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "insertFishName", menuName = "ScriptableObjects/FishData", order = 1)]
public class FishScriptableObject : ScriptableObject
{
    [HideInInspector] public string Name;
    public TextAsset DescriptionFile;
    public AudioClip AudioClip;
    public Sprite Sprite;
    public Mesh Mesh;
    public Texture Texture;

    private void OnEnable()
    {
        Name = name;
    }
}
