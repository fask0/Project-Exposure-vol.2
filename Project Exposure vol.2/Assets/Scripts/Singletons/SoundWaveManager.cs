using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Events;

public class SoundWaveManager : MonoBehaviour
{
    [SerializeField] private Color _highIntensityColor;
    [SerializeField] private Color _lowIntensityColor;
    [SerializeField] private int _heightMultiplier = 300;

    [SerializeField] private UnityEvent _afterFishScanEvent;
    [SerializeField] private UnityEvent _afterArtifactScanEvent;

    private const int SpectrumSize = 4096;

    public delegate void OnFishScan(GameObject pGameObject);
    public event OnFishScan onFishScanEvent;

    public Dictionary<GameObject, UnityEvent> scanStartEvents = new Dictionary<GameObject, UnityEvent>();
    public Dictionary<GameObject, UnityEvent> scanEvents = new Dictionary<GameObject, UnityEvent>();
    public Dictionary<GameObject, UnityEvent> scanCancelEvents = new Dictionary<GameObject, UnityEvent>();
    private bool _isScanning = false;
    private bool _isPlayerScanningCreature = false;
    private bool _isPlayerScanningTarget = false;

    ///Fields
    //Spectrogram
    private int _texWidth;
    private int _texHeight;
    private float[] _subtractSpecturm;
    private float[] _exponentialSpectrum;

    //PlayerSoundWave
    //Left
    private GameObject _playerSoundWaveLeft;
    private Material _playerLeftImageMaterial;
    private float[] _playerOutputDataLeft;
    private int _playerLeftColumn;
    //Right
    private GameObject _playerSoundWaveRight;
    private Material _playerRightImageMaterial;
    private int _playerRightColumn;
    private float[] _playerOutputDataRight;

    //TargetSoundWave
    private GameObject _targetSoundWave;
    private AudioSource _targetAudioSource;
    private Material _targetImageMaterial;
    private int _targetColumn;
    private float[] _targetOutputData;

    //CursomSoundWave
    private GameObject _customSoundWave;
    private AudioSource _customAudioSource;
    private Material _customImageMaterial;
    private int _customColumn;
    private float[] _customOutputData;
    private bool _shouldUpdateCustom;

    //Scanning
    private GameObject _currentScan;
    private Image _scanProgress;
    private float _scanDuration;
    private float _scanTimeLeft;

    private List<GameObject> _listeningToCollected = new List<GameObject>();
    private List<GameObject> _listeningToAll = new List<GameObject>();

    ArtifactParent _artifactParent;
    private CameraBehaviour _cameraBehaviour;
    private int _frameCount;

    void Start()
    {
        SingleTons.SoundWaveManager = this;

        InitPlayerSoundWave();
        InitTargetSoundWave();
        _subtractSpecturm = new float[SpectrumSize];

        int exponentialSize = 0;
        float bandSize = 1.1f;
        float crossover = bandSize;
        for (int i = 0; i < SpectrumSize; i++)
        {
            if (i > crossover)
            {
                crossover *= bandSize;
                exponentialSize++;
            }
        }
        _exponentialSpectrum = new float[exponentialSize];

        _scanProgress = MainCanavasManager.Scanprogress.GetComponent<Image>();
        _scanProgress.enabled = false;
        _scanDuration = 3.0f;
        _scanTimeLeft = _scanDuration;

        _texWidth = _playerLeftImageMaterial.mainTexture.width;
        _texHeight = _playerLeftImageMaterial.mainTexture.height;
        _playerLeftColumn = 0;

        _cameraBehaviour = Camera.main.transform.parent.GetComponent<CameraBehaviour>();
        _artifactParent = GameObject.FindObjectOfType<ArtifactParent>();
        SingleTons.GameController.onSceneLoadEvent += OnNewSceneLoad;
    }

    void Update()
    {
        _frameCount++;

        UpdatePlayerSoundWave();
        UpdateTargetSoundWave();
        UpdateCustomSoundWave();
    }

    private void OnNewSceneLoad(string pName)
    {
        _listeningToAll.Clear();
    }

    private void InitPlayerSoundWave()
    {
        //Left
        _playerSoundWaveLeft = MainCanavasManager.Spectrograms.transform.GetChild(0).gameObject;
        _playerLeftImageMaterial = _playerSoundWaveLeft.transform.GetChild(1).GetChild(0).GetComponent<Image>().material;
        ResetTexture(_playerLeftImageMaterial);
        _playerOutputDataLeft = new float[SpectrumSize];
        //Right
        _playerSoundWaveRight = MainCanavasManager.Spectrograms.transform.GetChild(1).gameObject;
        _playerRightImageMaterial = _playerSoundWaveRight.transform.GetChild(1).GetChild(0).GetComponent<Image>().material;
        ResetTexture(_playerRightImageMaterial);
        _playerOutputDataRight = new float[SpectrumSize];
    }

    private void UpdatePlayerSoundWave()
    {
        //Update Texture
        if ((_frameCount + 3) % 3 == 0)
        {
            AudioListener.GetSpectrumData(_playerOutputDataLeft, 0, FFTWindow.BlackmanHarris);
            DrawSpectrogram(_playerLeftImageMaterial, _playerOutputDataLeft, _playerLeftColumn, _subtractSpecturm);
            _playerLeftColumn--;
            if (_playerLeftColumn < 0) _playerLeftColumn = _texWidth - 1;
        }

        if ((_frameCount + 2) % 3 == 0)
        {
            AudioListener.GetSpectrumData(_playerOutputDataRight, 1, FFTWindow.BlackmanHarris);
            DrawSpectrogram(_playerRightImageMaterial, _playerOutputDataRight, _playerRightColumn, _subtractSpecturm);
            _playerRightColumn--;
            if (_playerRightColumn < 0) _playerRightColumn = _texWidth - 1;
        }
    }

    private void InitTargetSoundWave()
    {
        _targetSoundWave = MainCanavasManager.Spectrograms.transform.GetChild(2).gameObject;
        _targetImageMaterial = _targetSoundWave.transform.GetChild(1).GetChild(0).GetComponent<Image>().material;
        ResetTexture(_targetImageMaterial);
        _targetAudioSource = transform.parent.GetChild(5).GetComponent<AudioSource>();
        _targetOutputData = new float[SpectrumSize];
    }

    private void UpdateTargetSoundWave()
    {
        //Update Texture
        if ((_frameCount + 1) % 3 == 0)
        {
            _targetAudioSource.GetSpectrumData(_targetOutputData, 0, FFTWindow.BlackmanHarris);
            DrawSpectrogram(_targetImageMaterial, _targetOutputData, _targetColumn);
            _targetColumn--;
            if (_targetColumn < 0) _targetColumn = _texWidth - 1;
        }
    }

    private void UpdateCustomSoundWave()
    {
        if (!_shouldUpdateCustom) return;

        if (_frameCount % 3 == 0)
        {
            _customAudioSource.GetSpectrumData(_customOutputData, 0, FFTWindow.BlackmanHarris);
            DrawSpectrogram(_customImageMaterial, _customOutputData, _customColumn);
            _customColumn--;
            if (_customColumn < 0) _customColumn = _texWidth - 1;
        }
    }

    /// <summary>
    /// asd
    /// </summary>
    /// <param name="pMesh">Original mesh</param>
    /// <param name="pMaterial">Original material</param>
    /// <param name="pSpectrum">Linear spectrum</param>
    private void DrawSpectrogram(Material pMaterial, float[] pSpectrum, int pColumn, float[] pSubtract = null)
    {
        // Transfer from Linear spectrum to an Exponential one:
        float bandSize = 1.1f;
        float crossover = bandSize;
        float b = 0.0f;
        int arrayIndex = 0;
        for (int i = 0; i < SpectrumSize; i++)
        {
            float d = 0.0f;

            if (pSubtract == null)
                d = pSpectrum[i];
            else
                d = pSpectrum[i] - pSubtract[i];

            b = Mathf.Max(d, b);
            if (i > crossover)
            {
                crossover *= bandSize;
                _exponentialSpectrum[arrayIndex] = b;
                b = 0;
                arrayIndex++;
            }
        }

        // Every pixel represents this many data points from the spectrum:
        float segmentSize = (float)_exponentialSpectrum.Length / (float)_texHeight;

        // Draw the pixels and apply them to the original texture:
        Texture2D newTex = pMaterial.mainTexture as Texture2D;
        for (int y = 0; y < _texHeight; y++)
        {
            int x = _texWidth - pColumn;
            newTex.SetPixel(x, y, GetGradient(_exponentialSpectrum[(int)(y * segmentSize)] * _heightMultiplier, y));
        }
        newTex.Apply();
        pMaterial.mainTexture = newTex;

        // Offset the texture:
        pMaterial.mainTextureOffset += new Vector2(1 / ((float)_texWidth), 0);
    }

    /// <summary>
    /// Returns a color for each value between 0 and 1, chosen from a smooth gradient
    /// </summary>
    /// <param name="pValue"></param>
    /// <returns></returns>
    private Color GetGradient(float pValue, float pIndex)
    {
        Color color = _highIntensityColor * pValue * 2 + _lowIntensityColor * (1 - pValue * 2);
        color.a = Mathf.Clamp(pValue * 5, 0.0f, 1.0f);
        return color;
    }

    public void StartDrawingCustomSpectrogram(GameObject pGameObject, AudioSource pAudioSource)
    {
        _customSoundWave = pGameObject;
        _customAudioSource = pAudioSource;
        _customAudioSource.Play();
        _customImageMaterial = _customSoundWave.GetComponent<Image>().material;
        _customImageMaterial.mainTextureOffset = new Vector2(0, -0.0065f);
        _customColumn = 0;
        _customOutputData = new float[SpectrumSize];

        _shouldUpdateCustom = true;
    }

    public void StopDrawingCustomSpectrogram()
    {
        if (_customAudioSource != null)
            _customAudioSource.Stop();
        _shouldUpdateCustom = false;
    }

    public void ResetTexture(Material pMaterial)
    {
        Texture2D tex = pMaterial.mainTexture as Texture2D;
        for (int x = 0; x < tex.width; x++)
            for (int y = 0; y < tex.height; y++)
                tex.SetPixel(x, y, new Color(0, 0, 0, 0));
        tex.Apply();
        pMaterial.mainTexture = tex;
        pMaterial.mainTextureOffset = new Vector2(0, -0.0065f);
    }

    public void ResetPlayerTextures()
    {
        ResetTexture(_playerLeftImageMaterial);
        _playerLeftColumn = 0;
        ResetTexture(_playerRightImageMaterial);
        _playerRightColumn = 0;
    }

    public void ScanCreature(GameObject pScannedCreature)
    {
        Renderer[] renderers = pScannedCreature.GetComponentsInChildren<Renderer>();
        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
            foreach (Material material in renderer.materials)
                materials.Add(material);

        if (Input.GetKeyDown(KeyCode.Mouse0) && _currentScan != pScannedCreature)
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30.0f, ~(1 << 8));
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.isTrigger || hits[i].collider.gameObject != pScannedCreature) continue;

                if (!SingleTons.CollectionsManager.IsCollected(pScannedCreature.name))
                {
                    if (_currentScan != null)
                        CallScanEvents();

                    _isPlayerScanningCreature = true;
                    _isPlayerScanningTarget = false;

                    HideProgress(_currentScan);
                    _currentScan = pScannedCreature;

                    CallScanEvents();
                }
                else
                {
                    _isPlayerScanningCreature = false;
                    _isPlayerScanningTarget = false;
                }
                SingleTons.GameController.Player.GetComponent<PlayerMovementBehaviour>().StartFollowingGameObject(pScannedCreature);

                break;
            }
        }

        if (!_isPlayerScanningCreature)
        {
            if (SingleTons.CollectionsManager.IsCollected(pScannedCreature.name)) return;

            HideProgress(pScannedCreature);
            _currentScan = null;
            _scanProgress.fillAmount = (_scanDuration - _scanTimeLeft) / _scanDuration;

            foreach (Material material in materials)
            {
                material.SetFloat("_IsScanning", 0);
                material.SetFloat("_ScanLines", 0);
                material.SetFloat("_ScanLineWidth", 0);
            }

            return;
        }
        else if (_currentScan != pScannedCreature) return;

        _scanTimeLeft -= Time.deltaTime;

        foreach (Material material in materials)
        {
            material.SetFloat("_IsScanning", 1);
            material.SetFloat("_ScanLines", (_scanDuration - _scanTimeLeft) * 20);
            material.SetFloat("_ScanLineWidth", _scanDuration - _scanTimeLeft);
        }

        if (_scanTimeLeft <= 0)
        {
            int score = 100;
            SingleTons.ScoreManager.AddScore(score);
            SingleTons.CollectionsManager.AddToCollection(pScannedCreature);

            for (int i = 0; i < _listeningToAll.Count; i++)
                if (_listeningToAll[i].name == pScannedCreature.name)
                    _listeningToCollected.Add(_listeningToAll[i]);

            if (onFishScanEvent != null)
                onFishScanEvent(pScannedCreature);

            _afterFishScanEvent.Invoke();

            foreach (KeyValuePair<GameObject, UnityEvent> unityEvent in scanEvents)
                if (unityEvent.Key == pScannedCreature)
                    unityEvent.Value.Invoke();

            CallScanEvents();
            HideProgress(pScannedCreature);
            _isPlayerScanningCreature = false;
        }

        _scanProgress.fillAmount = (_scanDuration - _scanTimeLeft) / _scanDuration;
    }

    public void ScanTarget(GameObject pScannedTarget)
    {
        Material mat = pScannedTarget.GetComponentInChildren<Renderer>().material;
        Renderer[] renderers = pScannedTarget.GetComponentsInChildren<Renderer>();
        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
            foreach (Material material in renderer.materials)
                materials.Add(material);

        if (Input.GetKeyDown(KeyCode.Mouse0) && _currentScan != pScannedTarget)
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30.0f, ~(1 << 8));
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.isTrigger || hits[i].collider.gameObject != pScannedTarget) continue;

                if (hits[i].transform.gameObject == pScannedTarget)
                {
                    if (!SingleTons.CollectionsManager.HasTargetBeenScanned(pScannedTarget.tag))
                    {
                        if (_currentScan != null)
                            CallScanEvents();

                        _isPlayerScanningTarget = true;
                        _isPlayerScanningCreature = false;

                        HideProgress(_currentScan);
                        _currentScan = pScannedTarget;
                        _cameraBehaviour.StartScanningArtifact(pScannedTarget);

                        CallScanEvents();
                    }
                    else
                    {
                        _isPlayerScanningTarget = false;
                        _isPlayerScanningCreature = false;
                    }
                    break;
                }
            }
        }

        if (!_isPlayerScanningTarget)
        {
            HideProgress(pScannedTarget);
            _currentScan = null;
            _scanProgress.fillAmount = (_scanDuration - _scanTimeLeft) / _scanDuration;

            foreach (Material material in materials)
            {
                material.SetFloat("_IsScanning", 0);
                material.SetFloat("_ScanLines", 0);
                material.SetFloat("_ScanLineWidth", 0);
            }

            return;
        }
        else if (_currentScan != pScannedTarget) return;

        int index = 0;
        int.TryParse(pScannedTarget.tag.Substring(6), out index);

        _scanTimeLeft -= Time.deltaTime;

        foreach (Material material in materials)
        {
            material.SetFloat("_IsScanning", 1);
            material.SetFloat("_ScanLines", (_scanDuration - _scanTimeLeft) * 20);
            material.SetFloat("_ScanLineWidth", _scanDuration - _scanTimeLeft);
        }

        if (_scanTimeLeft <= 0)
        {
            float timeMultiplier = 0;
            for (int i = 0; i < index + 1; i++)
                timeMultiplier += (i == index) ? _artifactParent.GetRadarTimesList()[i] * 1.25f : _artifactParent.GetRadarTimesList()[i];
            timeMultiplier -= Time.time - _artifactParent.GetRadarTimesList()[index];
            timeMultiplier /= _artifactParent.GetRadarTimesList()[index];
            timeMultiplier = Mathf.Clamp(timeMultiplier, 1.0f, 3.0f);
            int score = (int)(50 * (1 + index) * timeMultiplier);

            if (index == SingleTons.QuestManager.GetCurrentTargetIndex())
            {
                SingleTons.ScoreManager.AddScore(score);
                SingleTons.QuestManager.NextTargetAudio();
            }
            else if (index > SingleTons.QuestManager.GetCurrentTargetIndex())
            {
                SingleTons.ScoreManager.AddScore(score);
                SingleTons.QuestManager.SetTargetAudio(index + 1);
            }
            else if (index < SingleTons.QuestManager.GetCurrentTargetIndex())
            {
                SingleTons.ScoreManager.AddScore(score);
            }

            SingleTons.CollectionsManager.CollectArtifact(index);

            _cameraBehaviour.StopScanningArtifact();

            if (onFishScanEvent != null)
                onFishScanEvent(pScannedTarget);

            _afterArtifactScanEvent.Invoke();

            foreach (KeyValuePair<GameObject, UnityEvent> unityEvent in scanEvents)
                if (unityEvent.Key == pScannedTarget)
                    unityEvent.Value.Invoke();

            CallScanEvents();
            HideProgress(pScannedTarget);
            _isPlayerScanningTarget = false;
        }

        _scanProgress.fillAmount = (_scanDuration - _scanTimeLeft) / _scanDuration;
    }

    public void ShowProgress(GameObject pCurrentScan)
    {
        if (_currentScan == null || _currentScan != pCurrentScan) return;

        _scanProgress.enabled = true;
    }

    public void HideProgress(GameObject pCurrentScan)
    {
        if (_currentScan == null || _currentScan != pCurrentScan) return;

        Material mat = pCurrentScan.GetComponentInChildren<Renderer>().material;
        Renderer[] renderers = pCurrentScan.GetComponentsInChildren<Renderer>();
        List<Material> mats = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                mats.Add(material);
            }
        }

        foreach (Material material in mats)
        {
            material.SetFloat("_IsScanning", 0);
            material.SetFloat("_ScanLines", 0);
            material.SetFloat("_ScanLineWidth", 0);
        }

        _scanTimeLeft = _scanDuration;
        _currentScan = null;
        _scanProgress.enabled = false;
    }

    public void StopScanning(GameObject pCurrentScan)
    {
        if (_currentScan == null || _currentScan != pCurrentScan) return;

        CallScanEvents();
        HideProgress(pCurrentScan);

        _isPlayerScanningCreature = false;
        _isPlayerScanningTarget = false;
    }

    public void AddSource(GameObject pGameObject)
    {
        _listeningToCollected.Add(pGameObject);
    }

    public List<GameObject> GetListeningToCollected
    {
        get { return _listeningToCollected; }
    }

    public List<GameObject> GetListeningToAll
    {
        get { return _listeningToAll; }
    }

    private void OnDestroy()
    {
        if (_customImageMaterial != null)
            ResetTexture(_customImageMaterial);
        ResetTexture(_playerLeftImageMaterial);
        ResetTexture(_playerRightImageMaterial);
        ResetTexture(_targetImageMaterial);
    }

    private void CallScanEvents()
    {
        if (_scanTimeLeft == _scanDuration && !_isScanning)
        {
            //Start
            _isScanning = true;
            foreach (KeyValuePair<GameObject, UnityEvent> unityEvent in scanStartEvents)
            {
                if (unityEvent.Key == _currentScan)
                {
                    unityEvent.Value.Invoke();
                }
            }
        }
        else if (_scanTimeLeft < _scanDuration && _isScanning)
        {
            //Cancel
            _isScanning = false;
            foreach (KeyValuePair<GameObject, UnityEvent> unityEvent in scanCancelEvents)
            {
                if (unityEvent.Key == _currentScan)
                {
                    unityEvent.Value.Invoke();
                }
            }
        }
    }
}
