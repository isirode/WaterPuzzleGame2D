using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelListController : MonoBehaviour
{
    public UIDocument document;
    private VisualElement root;

    public string firstLevelSceneName;

    public string firstLevelButtonName = "FirstLevelButton";

    public string sceneListContainerName = "SceneListContainer";

    public string scenePathPrefix = string.Empty;

    private Regex levelNumberRegex = new Regex(@"[\w\d\/\\]{0,}Scene(\d+).unity", RegexOptions.IgnoreCase);

    public static Level currentLevel;

    public class Level
    {
        public int levelNumber;
        public string levelPath;
    }

    // Start is called before the first frame update
    void Start()
    {
        RequireComponent.AssignIfNotSet(this, ref document);
        RequireComponent.RequireThrow(this, document);

        root = document.rootVisualElement;

        var sceneListContainer = root.QR<VisualElement>(sceneListContainerName);

        // Info : we clear it because the document has children so that we can test it in the UI Builder
        sceneListContainer.Clear();

        RequireComponent.RequireNotEmptyThrow(this, scenePathPrefix);

        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (scenePath.StartsWith(scenePathPrefix) && !scenePath.Contains("bis"))
            {
                Match match = levelNumberRegex.Match(scenePath);
                if (match.Groups.Count > 1 && match.Success)
                {
                    // Info : the captured group is the second group
                    var levelNumberAsString = match.Groups[1].Value;
                    var levelNumber = int.Parse(levelNumberAsString);

                    var button = new Button();
                    button.text = levelNumberAsString;
                    button.AddToClassList("level");
                    button.AddToClassList("level_button");

                    sceneListContainer.Add(button);

                    var level = new Level()
                    {
                        levelNumber = levelNumber,
                        levelPath = scenePath
                    };

                    button.RegisterCallback<PointerUpEvent, Level>(LoadLevel, level);
                }
            }
        }
    }

    private void DoPlay(PointerUpEvent evt)
    {
        Debug.Log("Play");
        if (string.IsNullOrEmpty(firstLevelSceneName))
        {
            // TODO : do the checks in the Start method
            Debug.LogWarning($"{nameof(firstLevelSceneName)} is null or empy.");
            return;
        }

        SceneManager.LoadScene(firstLevelSceneName, LoadSceneMode.Single);
    }

    private void LoadLevel(PointerUpEvent evt, Level level)
    {
        Debug.Log(nameof(LoadLevel));

        currentLevel = level;

        SceneManager.LoadScene(level.levelPath, LoadSceneMode.Single);
    }
}
