using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AdditiveLevelListController : MonoBehaviour
{
    public UIDocument document;
    private VisualElement root;

    public string levelListContainerName = "LevelListContainer";
    public string levelPathPrefix = "";

    public string levelNumberRegexString;
    private Regex levelNumberRegex;

    public string mainAdditiveScenePath;

    class Level
    {
        public int levelNumber;
        public string levelPath;
    }

    void Start()
    {
        RequireComponent.AssignIfNotSet(this, ref document);
        RequireComponent.RequireThrow(this, document);

        RequireComponent.RequireNotEmptyThrow(this, levelListContainerName);
        RequireComponent.RequireNotEmptyThrow(this, levelPathPrefix);
        RequireComponent.RequireNotEmptyThrow(this, levelNumberRegexString);
        RequireComponent.RequireNotEmptyThrow(this, mainAdditiveScenePath);

        levelNumberRegex = new Regex(levelNumberRegexString, RegexOptions.IgnoreCase);

        root = document.rootVisualElement;

        var levelListContainer = root.Q<VisualElement>(levelListContainerName);
        if (levelListContainer == null)
        {
            throw new Exception($"{nameof(levelListContainer)} not found using '{levelListContainerName}' name.");
        }
        // Info : we clear it because the document has children so that we can test it in the UI Builder
        levelListContainer.Clear();

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            Debug.Log("scenePath " + scenePath);
            Debug.Log("Starts with " + scenePath.StartsWith(levelPathPrefix));
            if (scenePath.StartsWith(levelPathPrefix) && !scenePath.Contains("bis"))
            {
                Match match = levelNumberRegex.Match(scenePath);
                if (match.Groups.Count > 1 && match.Success)
                {
                    Debug.Log($"Groups {match.Groups.Count}");

                    var levelNumber = match.Groups[1].Value;

                    Debug.Log("value : " + levelNumber);
                    Debug.Log("value : " + match.Groups[0]);
                    Debug.Log("value : " + match.Groups[1]);
                    Debug.Log("value : " + match);

                    var levelNumberAsInt = int.Parse(levelNumber);

                    var level = new Level()
                    {
                        levelNumber = levelNumberAsInt,
                        levelPath = scenePath
                    };

                    var button = new Button();
                    button.text = levelNumber;
                    button.AddToClassList("level");
                    button.AddToClassList("level_button");

                    levelListContainer.Add(button);

                    button.RegisterCallback<PointerUpEvent, Level>(LoadLevel, level);
                }
            }
        }
    }

    private void LoadLevel(PointerUpEvent evt, Level level)
    {
        // FIXME : it is probably more natural to use the actual numbre
        AdditiveLevelManager.levelToUse = level.levelNumber;

        SceneManager.LoadScene(mainAdditiveScenePath, LoadSceneMode.Single);
    }
}
