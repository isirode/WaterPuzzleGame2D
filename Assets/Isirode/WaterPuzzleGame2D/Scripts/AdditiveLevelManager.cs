using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveLevelManager : MonoBehaviour
{
    public string additiveRootGameObjectName = "";

    private Level currentLevel;
    private GameObject additiveRoot;
    private bool firstLevelWasLoaded = false;

    // Mechanism used to pass the levels to load
    public static int levelToUse = 1;
    public int sceneLevelToUseOverride = -1;

    public string levelPathPrefix = string.Empty;

    // FIXME : duplicated from LevelListController except for "Level"
    private Regex levelNumberRegex = new Regex(@"[\w\d\/\\]{0,}Level_(\d+).unity", RegexOptions.IgnoreCase);

    class Level
    {
        public int levelNumber;
        public string levelPath;
    }

    private List<Level> levels = new List<Level>();

    public SimpleWinManager simpleWinManager;
    public WaitForDrawing waitForDrawing;

    private void Start()
    {
        Debug.Log(nameof(Start));

        RequireComponent.RequireNotEmptyThrow(this, this.levelPathPrefix);
        RequireComponent.RequireNotEmptyThrow(this, this.additiveRootGameObjectName);
        RequireComponent.RequireThrow(this, this.simpleWinManager);
        RequireComponent.RequireThrow(this, this.waitForDrawing);

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];

        // TODO : check if this would work
        // bundle = AssetBundle.LoadFromFile("Assets/MyPath/scenes");
        // scenePaths = myLoadedAssetBundle.GetAllScenePaths();

        for (int i = 0; i < sceneCount; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            Debug.Log("Starts with " + scenePath.StartsWith(levelPathPrefix));
            if (scenePath.StartsWith(levelPathPrefix) && !scenePath.Contains("bis"))
            {
                Match match = levelNumberRegex.Match(scenePath);
                if (match.Groups.Count > 1 && match.Success)
                {
                    Debug.Log($"Groups {match.Groups.Count}");

                    var levelNumber = match.Groups[1].Value;

                    var levelNumberAsInt = int.Parse(levelNumber);

                    var level = new Level()
                    {
                        levelNumber = levelNumberAsInt,
                        levelPath = scenePath
                    };

                    levels.Add(level);
                }
            }
        }
        Debug.Log($"{levels.Count} levels added");

        if (sceneLevelToUseOverride > 0)
        {
            currentLevel = levels[sceneLevelToUseOverride - 1];
            if (levelToUse != 1)
            {
                Debug.LogWarning($"using {sceneLevelToUseOverride} ({sceneLevelToUseOverride}) but {nameof(levelToUse)} ({levelToUse}) is set");
            }
        }
        else
        {
            currentLevel = levels[levelToUse - 1];
        }

        simpleWinManager.NextLevelRequested += OnNextLevelRequested;

        StartCoroutine(LoadNextLevel(false));
    }

    private void OnNextLevelRequested()
    {
        StartCoroutine(LoadNextLevel(true));
    }

    public IEnumerator LoadNextLevel(bool doIncrement)
    {
        Destroy(additiveRoot);
        if (firstLevelWasLoaded)
        {
            SceneManager.UnloadSceneAsync(currentLevel.levelPath);
        }

        if (doIncrement)
        {
            var currentIndex = currentLevel.levelNumber - 1;
            currentIndex += 1;
            if (currentIndex >= levels.Count)
            {
                // TODO : make optional to quit the additive scenes
                currentIndex = 0;
            }

            currentLevel = levels[currentIndex];
        }
        
        /*var parameters = new LoadSceneParameters()
        {
            loadSceneMode = LoadSceneMode.Additive,
        };
        var scene = SceneManager.LoadScene(currentLevel.levelPath, parameters);
        additiveRoot = scene.GetRootGameObjects().Where(x => x.name == additiveRootGameObjectName).First();
        */

        Debug.Log("Additive root : " + additiveRoot);
        var asyncOperation = SceneManager.LoadSceneAsync(currentLevel.levelPath, LoadSceneMode.Additive);

        while (!asyncOperation.isDone)
        {
            yield return new WaitForSeconds(0.5f);
        }

        additiveRoot = GameObject.Find(additiveRootGameObjectName);
        Debug.Log("Additive root : " + additiveRoot);
        if (additiveRoot == null)
        {
            throw new Exception($"{nameof(additiveRoot)} is null");
        }

        var waterfall = GameObject.FindObjectOfType<SimpleWaterfall>();
        Debug.Log("waterfall : " + waterfall);
        if (additiveRoot == null)
        {
            throw new Exception($"{nameof(waterfall)} is null");
        }

        var objective = GameObject.FindObjectOfType<WaterEnterObjective>();
        Debug.Log("objective : " + objective);
        if (additiveRoot == null)
        {
            throw new Exception($"{nameof(objective)} is null");
        }

        simpleWinManager.simpleWaterfall = waterfall;
        simpleWinManager.waterEnterObjective = objective;

        waitForDrawing.simpleWaterfall = waterfall;

        simpleWinManager.Init();
        simpleWinManager.ListenObjective();
        simpleWinManager.ListenWaterfall();

        simpleWinManager.RestartLevel();

        firstLevelWasLoaded = true;
    }
}
