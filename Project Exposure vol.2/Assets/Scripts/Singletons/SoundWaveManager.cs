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

    private const int SpectrumSize = 4096;

    public delegate void OnFishScan(GameObject pGameObject);
    public event OnFishScan onFishScanEvent;

    public Dictionary<GameObject, UnityEvent> scanEvents = new Dictionary<GameObject, UnityEvent>();

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
    //CollectedSoundWave
    //0
    private GameObject _collected0;
    private GameObject _collected0Child0;
    private GameObject _collected0Child1;
    private GameObject _collected0Child2;
    private Material _collectedImageMaterial0;
    private int _collected0Column;
    //1
    private GameObject _collected1;
    private GameObject _collected1Child0;
    private GameObject _collected1Child1;
    private Material _collectedImageMaterial1;
    private int _collected1Column;
    //2
    private GameObject _collected2;
    private GameObject _collected2Child0;
    private Material _collectedImageMaterial2;
    private int _collected2Column;
    private float[] _individualOutputData;

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

        _scanProgress = Camera.main.transform.GetChild(0).GetChild(2).GetComponent<Image>();
        _scanProgress.enabled = false;
        _scanDuration = 3.0f;
        _scanTimeLeft = _scanDuration;

        _texWidth = _playerLeftImageMaterial.mainTexture.width;
        _texHeight = _playerLeftImageMaterial.mainTexture.height;
        _playerLeftColumn = 0;

        _cameraBehaviour = Camera.main.transform.parent.GetComponent<CameraBehaviour>();
        SingleTons.GameController.onSceneLoadEvent += OnNewSceneLoad;
    }

    void Update()
    {
        _frameCount++;

        UpdateCollectedSoundWaves();
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
        _playerSoundWaveLeft = Camera.main.transform.GetChild(0).GetChild(3).gameObject;
        _playerLeftImageMaterial = _playerSoundWaveLeft.transform.GetChild(2).GetChild(0).GetComponent<Image>().material;
        ResetTexture(_playerLeftImageMaterial);
        _playerOutputDataLeft = new float[SpectrumSize];
        //Right
        _playerSoundWaveRight = Camera.main.transform.GetChild(0).GetChild(4).gameObject;
        _playerRightImageMaterial = _playerSoundWaveRight.transform.GetChild(2).GetChild(0).GetComponent<Image>().material;
        ResetTexture(_playerRightImageMaterial);
        _playerOutputDataRight = new float[SpectrumSize];

        //Collected
        GameObject collected = Camera.main.transform.GetChild(0).GetChild(6).gameObject;
        //0
        _collected0 = collected.transform.GetChild(0).gameObject;
        _collectedImageMaterial0 = _collected0.transform.GetChild(0).GetChild(1).GetComponent<Image>().material;
        _collected0Child0 = _collected0.transform.GetChild(0).gameObject;
        _collected0Child0.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial0;
        _collected0Child0.SetActive(false);
        _collected0Child1 = _collected0.transform.GetChild(1).gameObject;
        _collected0Child1.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial0;
        _collected0Child1.SetActive(false);
        _collected0Child2 = _collected0.transform.GetChild(2).gameObject;
        _collected0Child2.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial0;
        _collected0Child2.SetActive(false);
        ResetTexture(_collectedImageMaterial0);
        //1
        _collected1 = collected.transform.GetChild(1).gameObject;
        _collectedImageMaterial1 = _collected1.transform.GetChild(0).GetChild(1).GetComponent<Image>().material;
        _collected1Child0 = _collected1.transform.GetChild(0).gameObject;
        _collected1Child0.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial1;
        _collected1Child0.SetActive(false);
        _collected1Child1 = _collected1.transform.GetChild(1).gameObject;
        _collected1Child1.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial1;
        _collected1Child1.SetActive(false);
        ResetTexture(_collectedImageMaterial1);
        //2
        _collected2 = collected.transform.GetChild(2).gameObject;
        _collectedImageMaterial2 = _collected2.transform.GetChild(0).GetChild(1).GetComponent<Image>().material;
        _collected2Child0 = _collected2.transform.GetChild(0).gameObject;
        _collected2Child0.SetActive(false);
        ResetTexture(_collectedImageMaterial2);

        _individualOutputData = new float[SpectrumSize];
    }

    private void UpdateCollectedSoundWaves()
    {
        if (_listeningToCollected.Count == 0)
        {
            //0
            _collected0Child0.SetActive(false);
            _collected0Child1.SetActive(false);
            _collected0Child2.SetActive(false);
            //1
            _collected1Child0.SetActive(false);
            _collected1Child1.SetActive(false);
            //2
            _collected2Child0.SetActive(false);
            return;
        }
        if (_listeningToCollected.Count == 1)
        {
            //0
            if (!_collected0Child0.activeSelf)
            {
                ResetTexture(_collectedImageMaterial0);
                _collected0Column = _texWidth - 1;
            }
            _collected0Child0.SetActive(true);
            _collected0Child1.SetActive(false);
            _collected0Child2.SetActive(false);
            //1
            _collected1Child0.SetActive(false);
            _collected1Child1.SetActive(false);
            //2
            _collected2Child0.SetActive(false);
        }
        else if (_listeningToCollected.Count == 2)
        {
            //0
            _collected0Child0.SetActive(false);
            _collected0Child1.SetActive(true);
            _collected0Child2.SetActive(false);
            //1
            if (!_collected1Child0.activeSelf)
            {
                ResetTexture(_collectedImageMaterial1);
                _collected1Column = _texWidth - 1;
            }
            _collected1Child0.SetActive(true);
            _collected1Child1.SetActive(false);
            //2
            _collected2Child0.SetActive(false);
        }
        else if (_listeningToCollected.Count >= 3)
        {
            //0
            _collected0Child0.SetActive(false);
            _collected0Child1.SetActive(false);
            _collected0Child2.SetActive(true);
            //1
            _collected1Child0.SetActive(false);
            _collected1Child1.SetActive(true);
            //2
            if (!_collected2Child0.activeSelf)
            {
                ResetTexture(_collectedImageMaterial2);
                _collected2Column = _texWidth - 1;
            }
            _collected2Child0.SetActive(true);
        }

        //Update Collected
        if ((_frameCount + 2) % 3 == 0 || (_frameCount + 3) % 3 == 0)
        {
            Array.Clear(_subtractSpecturm, 0, _subtractSpecturm.Length - 1);
            for (int i = 0; i < _listeningToCollected.Count; i++)
            {
                _listeningToCollected[i].GetComponent<AudioSource>().GetSpectrumData(_individualOutputData, 0, FFTWindow.BlackmanHarris);

                if (i == 0)
                {
                    //0
                    DrawSpectrogram(_collectedImageMaterial0, _individualOutputData, _collected0Column);
                    _collected0Column--;
                    if (_collected0Column < 0) _collected0Column = _texWidth - 1;
                }
                else if (i == 1)
                {
                    //1
                    DrawSpectrogram(_collectedImageMaterial1, _individualOutputData, _collected1Column);
                    _collected1Column--;
                    if (_collected1Column < 0) _collected1Column = _texWidth - 1;
                }
                else if (i == 2)
                {
                    //2
                    DrawSpectrogram(_collectedImageMaterial2, _individualOutputData, _collected2Column);
                    _collected2Column--;
                    if (_collected2Column < 0) _collected2Column = _texWidth - 1;
                }

                for (int j = 0; j < SpectrumSize; j++)
                    _subtractSpecturm[j] += _individualOutputData[j];
            }
        }
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
        _targetSoundWave = Camera.main.transform.GetChild(0).GetChild(5).gameObject;
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
        Color color = _highIntensityColor * pValue + _lowIntensityColor * (1 - pValue);
        color.a = Mathf.Clamp(pValue * 10, 0, 1);
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

    public void ScanCreature(GameObject pScannedCreature)
    {
        Material mat = pScannedCreature.GetComponentInChildren<Renderer>().material;
        Renderer[] renderers = pScannedCreature.GetComponentsInChildren<Renderer>();
        List<Material> mats = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                mats.Add(material);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            int score = 100;

            _currentScan = pScannedCreature;
            _scanTimeLeft -= Time.deltaTime;

            foreach (Material material in mats)
            {
                material.SetFloat("_IsScanning", 1);
                material.SetFloat("_ScanLines", (_scanDuration - _scanTimeLeft) * 20);
                material.SetFloat("_ScanLineWidth", _scanDuration - _scanTimeLeft);
            }

            if (_scanTimeLeft <= 0)
            {
                SingleTons.CollectionsManager.AddToCollection(pScannedCreature);
                SingleTons.ScoreManager.AddScore(score);

                mat.SetFloat("_IsScanning", 0);
                mat.SetFloat("_ScanLines", 0);
                mat.SetFloat("_ScanLineWidth", 0);

                for (int i = 0; i < _listeningToAll.Count; i++)
                    if (_listeningToAll[i].name == pScannedCreature.name)
                        _listeningToCollected.Add(_listeningToAll[i]);

                _scanTimeLeft = _scanDuration;

                if (onFishScanEvent != null)
                    onFishScanEvent(pScannedCreature);
                foreach (KeyValuePair<GameObject, UnityEvent> unityEvent in scanEvents)
                {
                    if (unityEvent.Key == pScannedCreature)
                    {
                        unityEvent.Value.Invoke();
                    }
                }

                HideProgress(pScannedCreature);
            }
        }
        else
        {
            foreach (Material material in mats)
            {
                material.SetFloat("_IsScanning", 0);
                material.SetFloat("_ScanLines", 0);
                material.SetFloat("_ScanLineWidth", 0);
            }

            _scanTimeLeft = _scanDuration;
            _currentScan = null;
        }

        _scanProgress.fillAmount = (_scanDuration - _scanTimeLeft) / _scanDuration;
    }

    public void ScanTarget(GameObject pScannedTarget)
    {
        Material mat = pScannedTarget.GetComponentInChildren<Renderer>().material;
        Renderer[] renderers = pScannedTarget.GetComponentsInChildren<Renderer>();
        List<Material> mats = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                mats.Add(material);
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30.0f, ~(1 << 8));
                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.isTrigger) continue;

                    if (hits[i].transform.gameObject == pScannedTarget)
                    {
                        _currentScan = pScannedTarget;
                        _cameraBehaviour.StartScanningArtifact(pScannedTarget);
                        break;
                    }
                    else
                    {
                        _currentScan = null;
                    }
                }
            }

            if (_currentScan == null || _currentScan != pScannedTarget) return;

            int index = 0;
            int.TryParse(pScannedTarget.tag.Substring(6), out index);
            int score = (int)(50 + 50 * index * 0.5f);

            _scanTimeLeft -= Time.deltaTime;

            foreach (Material material in mats)
            {
                material.SetFloat("_IsScanning", 1);
                material.SetFloat("_ScanLines", (_scanDuration - _scanTimeLeft) * 20);
                material.SetFloat("_ScanLineWidth", _scanDuration - _scanTimeLeft);
            }

            if (_scanTimeLeft <= 0)
            {
                if (pScannedTarget.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex()))
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

                foreach (Material material in mats)
                {
                    material.SetFloat("_IsScanning", 0);
                    material.SetFloat("_ScanLines", 0);
                    material.SetFloat("_ScanLineWidth", 0);
                }

                _scanTimeLeft = _scanDuration;
                _cameraBehaviour.StopScanningArtifact();

                foreach (KeyValuePair<GameObject, UnityEvent> unityEvent in scanEvents)
                {
                    if (unityEvent.Key == pScannedTarget)
                    {
                        unityEvent.Value.Invoke();
                    }
                }

                if (onFishScanEvent != null)
                    onFishScanEvent(pScannedTarget);

                HideProgress(pScannedTarget);
            }
        }
        else
        {
            foreach (Material material in mats)
            {
                material.SetFloat("_IsScanning", 0);
                material.SetFloat("_ScanLines", 0);
                material.SetFloat("_ScanLineWidth", 0);
            }

            _scanTimeLeft = _scanDuration;
            _cameraBehaviour.StopScanningArtifact();
            _currentScan = null;
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
        //mat.SetFloat("_IsScanning", 0);
        //mat.SetFloat("_ScanLines", 0);
        //mat.SetFloat("_ScanLineWidth", 0);

        _scanTimeLeft = _scanDuration;
        _scanProgress.enabled = false;
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
}
