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
    private int currentIndex = -1;
    private GameObject additiveRoot;

    public string levelPathPrefix = string.Empty;

    // FIXME : duplicated from LevelListController except for "Level"
    private Regex levelNumberRegex = new Regex(@"[\w\d\/\\]{0,}Level_(\d+).unity", RegexOptions.IgnoreCase);

    class Level
    {
        public string levelNumber;
        public string levelPath;
    }

    private List<Level> levels = new List<Level>();

    public SimpleWinManager simpleWinManager;
    public WaitForDrawing waitForDrawing;

    private void Start()
    {
        Debug.Log(nameof(Start));

        RequireComponent.RequireNotEmptyThrow(this, () => this.levelPathPrefix);
        RequireComponent.RequireNotEmptyThrow(this, () => this.additiveRootGameObjectName);
        RequireComponent.RequireThrow(this, () => this.simpleWinManager);
        RequireComponent.RequireThrow(this, () => this.waitForDrawing);

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

                    var level = new Level()
                    {
                        levelNumber = levelNumber,
                        levelPath = scenePath
                    };

                    levels.Add(level);
                }
            }
        }
        Debug.Log($"{levels.Count} levels added");

        StartCoroutine(LoadNextLevel());
    }

    public IEnumerator LoadNextLevel()
    {
        Destroy(additiveRoot);
        currentIndex += 1;
        if (currentIndex >= levels.Count)
        {
            // TODO : make optional to quit the additive scenes
            currentIndex = 0;
        }

        currentLevel = levels[currentIndex];
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
    }
}
