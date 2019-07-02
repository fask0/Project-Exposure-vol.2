using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public delegate void OnSceneLoad(string SceneName);
    public event OnSceneLoad onSceneLoadEvent;

    public delegate void OnAllSceneLoad(string SceneName);
    public event OnAllSceneLoad onAllSceneLoadEvent;

    public bool isAnySceneLoading = false;
    public List<Scene> LoadedScenes = new List<Scene>();

    [SerializeField]
    private bool _shouldLoadLevels = true;

    private float _originalTimescale;
    private float _originalFixedDeltaTime;

    //[HideInInspector]
    public GameObject Player;
    private DateTime _timeIdle;
    private bool _shouldUpdate = true;

    private void Awake()
    {
        SingleTons.GameController = this;
        if (SceneManager.GetActiveScene().name == "MainGameScene")
        {
            Load("Level0A");
            Load("Level0B");
            Load("Level0C");
            Load("Level0D Last");
            Load("Level0Transition");
        }
        else if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (_shouldLoadLevels)
                Initialize();

            _shouldUpdate = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Random.InitState(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour);
        int index = 0;
        while (index == 0)
            index = UnityEngine.Random.Range(-2, 3);
        UnityEngine.Random.InitState(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour * index);

        _originalTimescale = Time.timeScale;
        _originalFixedDeltaTime = Time.fixedDeltaTime;
        _timeIdle = DateTime.Now.AddSeconds(60);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_shouldUpdate) return;

        if (Input.anyKey)
            _timeIdle = DateTime.Now.AddSeconds(30);

        if (DateTime.Now >= _timeIdle)
            ResetGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public float GetRandomRange(float min, float max)
    {
        float rand = UnityEngine.Random.Range(min, max);
        return rand;
    }

    public void Load(string pSceneName)
    {
        if (!SceneManager.GetSceneByName(pSceneName).isLoaded)
            StartCoroutine("AsyncLoad", pSceneName);
    }

    private IEnumerator AsyncLoad(string pSceneName)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(pSceneName, LoadSceneMode.Additive);
        Rigidbody rb = null;
        if (Player != null)
            rb = Player.GetComponent<Rigidbody>();

        while (!async.isDone)
        {
            if (rb != null)
                rb.velocity *= 0.25f;
            yield return null;
            LoadedScenes.Add(SceneManager.GetSceneByName(pSceneName));
        }
    }

    public void Unload(string pSceneName)
    {
        if (SceneManager.GetSceneByName(pSceneName).isLoaded)
            StartCoroutine("AsyncUnload", pSceneName);
    }

    private IEnumerator AsyncUnload(string pSceneName)
    {
        AsyncOperation async = SceneManager.UnloadSceneAsync(pSceneName);

        while (!async.isDone)
        {
            yield return null;
            LoadedScenes.Remove(SceneManager.GetSceneByName(pSceneName));
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0.05f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = _originalTimescale;
        Time.fixedDeltaTime = _originalFixedDeltaTime;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        UnpauseGame();
    }

    public void Initialize()
    {
        StartCoroutine("UnloadMainMenu");
        Load("Level0A");
        Load("Level0B");
        Load("Level0C");
        Load("Level0D Last");
        Load("Level0Transition");
    }

    private IEnumerator UnloadMainMenu()
    {
        yield return new WaitForSeconds(0.01f);
        Unload("MainMenu");
    }

    private void OnLevelFinishedLoading(Scene pScene, LoadSceneMode pLoadMode)
    {
        if (pScene.name.Contains("Last"))
            if (onSceneLoadEvent != null)
                onSceneLoadEvent(pScene.name);

        if (onAllSceneLoadEvent != null)
            onAllSceneLoadEvent(pScene.name);
    }

    public void ShowResolutionScreen()
    {
        Transform canvas = Camera.main.transform.GetChild(0);
        for (int i = 1; i < canvas.childCount; i++)
        {
            Transform transform = canvas.GetChild(i);
            if (transform.gameObject.name == "ResolutionScreen")
                transform.gameObject.SetActive(true);
            else
                transform.gameObject.SetActive(false);
        }

        SingleTons.CollectionsManager.ReduceAllVolume();
        PauseGame();
    }

    private void OnDestroy()
    {

    }
}

public static class SingleTons
{
    public static GameController GameController;
    public static FishManager FishManager;
    public static QuestManager QuestManager;
    public static SoundWaveManager SoundWaveManager;
    public static CollectionsManager CollectionsManager;
    public static ScoreManager ScoreManager;
    public static MinimapManager MinimapManager;
    public static MainCanavasManager MainCanavasManager;

    public static GameObject FindChild(GameObject pParent, string pChildName)
    {
        for (int i = 0; i < pParent.transform.childCount; i++)
            if (pParent.transform.GetChild(i).name.ToLower() == pChildName.ToLower())
                return pParent.transform.GetChild(i).gameObject;

        return null;
    }
}
