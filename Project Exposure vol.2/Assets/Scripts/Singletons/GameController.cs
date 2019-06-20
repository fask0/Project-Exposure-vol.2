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

    public List<Scene> LoadedScenes = new List<Scene>();

    //[HideInInspector]
    public GameObject Player;

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
        else if (SceneManager.GetActiveScene().name == "DemoMainScene" || SceneManager.GetActiveScene().name == "MainMenu")
        {
            Initialize();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int index = 0;
        while (index == 0)
            index = UnityEngine.Random.RandomRange(-1, 1);
        UnityEngine.Random.InitState(DateTime.Now.Millisecond * DateTime.Now.Second * DateTime.Now.Minute * DateTime.Now.Hour * index);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
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
        {
            SceneManager.LoadScene(pSceneName, LoadSceneMode.Additive);
            LoadedScenes.Add(SceneManager.GetSceneByName(pSceneName));
        }
    }

    public void Unload(string pSceneName)
    {
        if (SceneManager.GetSceneByName(pSceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(pSceneName);
            LoadedScenes.Remove(SceneManager.GetSceneByName(pSceneName));
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
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
}
