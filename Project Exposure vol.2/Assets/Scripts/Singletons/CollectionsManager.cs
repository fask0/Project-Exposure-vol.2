﻿using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CollectionsManager : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float _defaultAudioVolume = 0.8f;
    [SerializeField] [Range(0, 50)] private int _defaultAudioDistanace = 8;
    [SerializeField] private float _subCodexMenuModelScaleMultiplier = 2.5f;
    [SerializeField] private List<FishScriptableObject> _fishScriptableObjects;

    [HideInInspector] public List<GameObject> collectableAudioSources = new List<GameObject>();
    [HideInInspector] public Dictionary<string, AudioSource> collectedAudioSources = new Dictionary<string, AudioSource>();
    [HideInInspector] public List<GameObject> _allAudioSources = new List<GameObject>();

    private List<GameObject> _codexMainMenu = new List<GameObject>();
    private List<int> _collectedArtifacts = new List<int>();

    private Mesh _undiscoveredSpeciesMesh;
    private Texture _undiscoveredSpeciesTexture;

    private GameObject _codexSubFishModel;
    private CodexSubMenuFishModelTilt _codexSubMenuFishModelTilt;
    private GameObject _codexSubPlayButton;
    private GameObject _codexSubStopButton;
    private AudioSource _codexSubSoundwave;
    private TextMeshProUGUI _codexSubDescription;

    private float _angleToRotate;

    void Start()
    {
        SingleTons.CollectionsManager = this;
        SingleTons.GameController.onSceneLoadEvent += GetAudioCourcesInScene;
        GetAudioCourcesInScene("");

        //Save MainMenu Elements
        GameObject codexMainMenu = Camera.main.transform.GetChild(0).GetChild(4).GetChild(1).gameObject;
        //All Fish Elements in the container to a List<GameObject>
        MeshFilter[] fish = codexMainMenu.transform.GetChild(0).GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < fish.Length; i++)
        {
            SetScale(fish[i].gameObject, 0.5f);
            _codexMainMenu.Add(fish[i].gameObject);
        }
        //Save Default Mesh/Texture
        _undiscoveredSpeciesMesh = fish[0].sharedMesh;
        _undiscoveredSpeciesTexture = fish[0].gameObject.GetComponent<MeshRenderer>().material.mainTexture;

        //Save SubMenu Elements
        GameObject codexSubMenu = Camera.main.transform.GetChild(0).GetChild(4).GetChild(2).gameObject;
        //Main Model
        _codexSubFishModel = codexSubMenu.transform.GetChild(1).gameObject;
        _codexSubFishModel.GetComponent<MeshFilter>().sharedMesh = _undiscoveredSpeciesMesh;
        _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
        SetScale(_codexSubFishModel, _subCodexMenuModelScaleMultiplier * 0.5f);
        //Main Model Overlay
        _codexSubMenuFishModelTilt = codexSubMenu.transform.GetChild(0).GetComponent<CodexSubMenuFishModelTilt>();
        //SoundWave
        _codexSubSoundwave = codexSubMenu.transform.GetChild(3).GetChild(0).GetChild(1).GetChild(0).GetComponent<AudioSource>();
        SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
        //Buttons
        _codexSubPlayButton = codexSubMenu.transform.GetChild(3).GetChild(1).gameObject;
        _codexSubStopButton = codexSubMenu.transform.GetChild(3).GetChild(2).gameObject;
        _codexSubStopButton.SetActive(false);
        //Description
        _codexSubDescription = codexSubMenu.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _codexSubDescription.text = "Unknown creature...";

        //Disable Codex
        Camera.main.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_codexSubFishModel.activeSelf) return;

        //Rotate all objects in the container
        for (int i = 0; i < _codexMainMenu.Count; i++)
            _codexMainMenu[i].transform.localRotation *= Quaternion.Euler(0, Time.deltaTime * 100, 0);

        //Rotate the object in the CodexSubMenu
        _angleToRotate += Time.deltaTime * 50;
        _codexSubFishModel.transform.localRotation = Quaternion.Slerp(_codexSubFishModel.transform.localRotation,
                                                                      Quaternion.Euler(30 * _codexSubMenuFishModelTilt.Vertical(), _angleToRotate, 0),
                                                                      Time.deltaTime * 5);
    }

    private void GetAudioCourcesInScene(string pSceneName)
    {
        //Normalize the sound of all objects that have an AudioSource in the scene
        collectableAudioSources.Clear();
        AudioSource[] gos = FindObjectsOfType<AudioSource>();
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i].gameObject.layer != 10) continue;

            AudioSource aSource = gos[i];
            aSource.spatialBlend = 1.0f;
            aSource.volume = _defaultAudioVolume;
            aSource.maxDistance = _defaultAudioDistanace;
            aSource.rolloffMode = AudioRolloffMode.Linear;
            aSource.loop = true;
            aSource.Play();
            _allAudioSources.Add(gos[i].gameObject);

            if (gos[i].gameObject.tag == "Collectable")
                if (!IsCollected(gos[i].gameObject.name))
                    collectableAudioSources.Add(gos[i].gameObject);
        }
    }

    /// <summary>
    /// Checks whether an object is already collected
    /// </summary>
    /// <param name="pGameObjectName"></param>
    /// <returns></returns>
    public bool IsCollected(string pGameObjectName)
    {
        foreach (string key in collectedAudioSources.Keys)
        {
            if (key == pGameObjectName)
                return true;
        }

        return false;
    }

    public bool HasTargetBeenScanned(string pTag)
    {
        int index = 0;
        int.TryParse(pTag.Substring(6), out index);
        foreach (int targetIndex in _collectedArtifacts)
            if (targetIndex == index)
                return true;

        return false;
    }

    /// <summary>
    /// Add a gameObject.name and an AudioSource to a Dictionary
    /// </summary>
    /// <param name="pGameObject"></param>
    public void AddToCollection(GameObject pGameObject)
    {
        bool isAlreadyCollected = true;
        for (int i = 0; i < collectableAudioSources.Count; i++)
        {
            if (collectableAudioSources[i] == pGameObject)
            {
                collectedAudioSources.Add(pGameObject.name, pGameObject.GetComponent<AudioSource>());
                isAlreadyCollected = false;
                Debug.Log(string.Format("New audio sample found: {0}", pGameObject.transform.name.ToUpper()));

                for (int j = 0; j < _codexMainMenu.Count; j++)
                {
                    if (_codexMainMenu[j].transform.parent.name.ToLower() == pGameObject.name.ToLower())
                    {
                        for (int k = 0; k < _fishScriptableObjects.Count; k++)
                        {
                            if (_codexMainMenu[j].transform.parent.name.ToLower() == _fishScriptableObjects[k].name.ToLower())
                            {
                                _codexMainMenu[j].GetComponent<MeshFilter>().sharedMesh = _fishScriptableObjects[k].Mesh;
                                _codexMainMenu[j].GetComponent<MeshRenderer>().material.mainTexture = _fishScriptableObjects[k].Texture;
                                SetScale(_codexMainMenu[j]);
                                break;
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }

        if (isAlreadyCollected)
        {
            print("Object already collected!");
            return;
        }

        for (int i = collectableAudioSources.Count - 1; i >= 0; i--)
            if (collectableAudioSources[i].name == pGameObject.name)
                collectableAudioSources.RemoveAt(i);
    }

    public void CollectArtifact(int pIndex)
    {
        foreach (int index in _collectedArtifacts)
            if (index == pIndex) return;

        _collectedArtifacts.Add(pIndex);
    }

    public int GetMaxDistance
    {
        get { return _defaultAudioDistanace; }
    }

    public void GotoDescription(GameObject pGameObject)
    {
        StopAudioSample();
        for (int i = 0; i < _fishScriptableObjects.Count; i++)
        {
            if (_fishScriptableObjects[i].Name.ToLower() == pGameObject.name.ToLower())
            {
                if (IsCollected(pGameObject.name))
                {
                    //Discovered Species
                    _codexSubFishModel.GetComponent<MeshFilter>().sharedMesh = _fishScriptableObjects[i].Mesh;
                    _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _fishScriptableObjects[i].Texture;
                    SetScale(_codexSubFishModel, _subCodexMenuModelScaleMultiplier);
                    _codexSubSoundwave.clip = _fishScriptableObjects[i].AudioClip;
                    _codexSubDescription.text = _fishScriptableObjects[i].DescriptionFile.text;
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
                }
                else
                {
                    //Undiscovered Species
                    _codexSubFishModel.GetComponent<MeshFilter>().sharedMesh = _undiscoveredSpeciesMesh;
                    _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
                    SetScale(_codexSubFishModel, _subCodexMenuModelScaleMultiplier * 0.5f);
                    _codexSubSoundwave.clip = null;
                    _codexSubDescription.text = "Unknown creature...";
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
                }
                return;
            }
        }

        //Undiscovered Species
        _codexSubFishModel.GetComponent<MeshFilter>().sharedMesh = _undiscoveredSpeciesMesh;
        _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
        SetScale(_codexSubFishModel, _subCodexMenuModelScaleMultiplier * 0.5f);
        _codexSubSoundwave.clip = null;
        _codexSubDescription.text = "Unknown creature...";
        SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
    }

    public void GotoDescription(string pFishName)
    {
        StopAudioSample();
        for (int i = 0; i < _fishScriptableObjects.Count; i++)
        {
            if (_fishScriptableObjects[i].Name == pFishName)
            {
                if (IsCollected(pFishName))
                {
                    //Discovered Species
                    _codexSubFishModel.GetComponent<MeshFilter>().sharedMesh = _fishScriptableObjects[i].Mesh;
                    _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _fishScriptableObjects[i].Texture;
                    SetScale(_codexSubFishModel, _subCodexMenuModelScaleMultiplier);
                    _codexSubSoundwave.clip = _fishScriptableObjects[i].AudioClip;
                    _codexSubDescription.text = _fishScriptableObjects[i].DescriptionFile.text;
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
                }
                else
                {
                    //Undiscovered Species
                    _codexSubFishModel.GetComponent<MeshFilter>().sharedMesh = _undiscoveredSpeciesMesh;
                    _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
                    SetScale(_codexSubFishModel, _subCodexMenuModelScaleMultiplier * 0.5f);
                    _codexSubSoundwave.clip = null;
                    _codexSubDescription.text = "Unknown creature...";
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
                }
                return;
            }
        }

        //Undiscovered Species
        _codexSubFishModel.GetComponent<MeshFilter>().sharedMesh = _undiscoveredSpeciesMesh;
        _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
        SetScale(_codexSubFishModel, _subCodexMenuModelScaleMultiplier * 0.5f);
        _codexSubSoundwave.clip = null;
        _codexSubDescription.text = "Unknown creature...";
        SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
    }

    public void GotoDescriptionFromSpectrogram(List<GameObject> pSpectrogramAudioList, GameObject pButton)
    {
        switch (pButton.transform.parent.parent.name)
        {
            case "0":
                AudioSource collected0 = pSpectrogramAudioList[0].GetComponent<AudioSource>();
                foreach (KeyValuePair<string, AudioSource> entry in collectedAudioSources)
                {
                    if (entry.Value == collected0)
                    {
                        GotoDescription(entry.Key);
                        break;
                    }
                }
                break;

            case "1":
                AudioSource collected1 = pSpectrogramAudioList[1].GetComponent<AudioSource>();
                foreach (KeyValuePair<string, AudioSource> entry in collectedAudioSources)
                {
                    if (entry.Value == collected1)
                    {
                        GotoDescription(entry.Key);
                        break;
                    }
                }
                break;

            case "2":
                AudioSource collected2 = pSpectrogramAudioList[2].GetComponent<AudioSource>();
                foreach (KeyValuePair<string, AudioSource> entry in collectedAudioSources)
                {
                    if (entry.Value == collected2)
                    {
                        GotoDescription(entry.Key);
                        break;
                    }
                }
                break;
        }

        ReduceAllVolume();
    }

    /// <summary>
    /// Reduces the volume of all AudioSources
    /// </summary>
    public void ReduceAllVolume()
    {
        for (int i = 0; i < _allAudioSources.Count; i++)
            if (_allAudioSources[i] != null)
                _allAudioSources[i].GetComponent<AudioSource>().volume = _defaultAudioVolume * 0.1f;
    }

    /// <summary>
    /// Sets the volume of all AudioSources to the default
    /// </summary>
    public void IncreaseAllVolume()
    {
        for (int i = 0; i < _allAudioSources.Count; i++)
            if (_allAudioSources[i] != null)
                _allAudioSources[i].GetComponent<AudioSource>().volume = _defaultAudioVolume;
    }

    public void PlayAudioSample()
    {
        if (_codexSubSoundwave.clip != null)
        {
            SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
            SingleTons.SoundWaveManager.StartDrawingCustomSpectrogram(_codexSubSoundwave.gameObject, _codexSubSoundwave);
            _codexSubPlayButton.SetActive(false);
            _codexSubStopButton.SetActive(true);
        }
    }

    public void StopAudioSample()
    {
        SingleTons.SoundWaveManager.StopDrawingCustomSpectrogram();
        _codexSubPlayButton.SetActive(true);
        _codexSubStopButton.SetActive(false);
    }

    private void SetScale(GameObject pGameObject, float pMultiplier = 1)
    {
        Vector3 bounds = pGameObject.GetComponent<MeshFilter>().sharedMesh.bounds.extents;
        float heighestExtent = Mathf.Max(Mathf.Max(bounds.x, bounds.y), bounds.z);
        float maxScale = 100 * pMultiplier;
        float ratio = maxScale / heighestExtent;
        pGameObject.transform.localScale = new Vector3(1, 1, 1) * ratio;
    }
}
