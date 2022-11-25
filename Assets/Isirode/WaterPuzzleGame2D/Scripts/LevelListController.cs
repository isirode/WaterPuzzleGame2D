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

    // Start is called before the first frame update
    void Start()
    {
        if (document == null)
        {
            document = GetComponent<UIDocument>();
            if (document == null)
            {
                Debug.LogWarning($"{nameof(document)} is not set.");
                return;
            }
            root = document.rootVisualElement;
        }
        // menu.RegisterCallback<PointerLeaveEvent>(HideMenu);
        /*
        var firstLevelButton = root.Q<Button>(firstLevelButtonName);
        if (firstLevelButton != null)
        {
            firstLevelButton.RegisterCallback<PointerUpEvent>(DoPlay);
        }
        else
        {
            Debug.LogWarning($"{nameof(firstLevelButton)} not found using '{firstLevelButtonName}' name.");
        }
        */

        var sceneListContainer = root.Q<VisualElement>(sceneListContainerName);
        if (sceneListContainer == null)
        {
            Debug.LogWarning($"{nameof(sceneListContainer)} not found using '{sceneListContainerName}' name.");
            return;
        }

        // Info : we clear it because the document has children so that we can test it in the UI Builder
        sceneListContainer.Clear();

        if (scenePathPrefix == string.Empty)
        {
            Debug.LogWarning($"{nameof(scenePathPrefix)} is empty.");
            return;
        }

        Debug.Log("Prefix : " + scenePathPrefix);

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];

        for (int i = 0; i < sceneCount; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            Debug.Log("Starts with " + scenePath.StartsWith(scenePathPrefix));
            if (scenePath.StartsWith(scenePathPrefix) && !scenePath.Contains("bis"))
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

                    var button = new Button();
                    button.text = levelNumber;
                    button.AddToClassList("level");
                    button.AddToClassList("level_button");

                    sceneListContainer.Add(button);

                    button.RegisterCallback<PointerUpEvent, string>(LoadLevel, scenePath);
                }
            }
            Debug.Log(scenePath);
            Debug.Log(scenePathPrefix);
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

    private void LoadLevel(PointerUpEvent evt, string scenePath)
    {
        Debug.Log(nameof(LoadLevel));
        if (string.IsNullOrEmpty(scenePath))
        {
            // TODO : do the checks in the Start method
            Debug.LogWarning($"{nameof(scenePath)} is null or empy.");
            return;
        }

        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }
}
