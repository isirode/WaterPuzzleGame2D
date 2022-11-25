using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelListController : MonoBehaviour
{
    public UIDocument document;
    private VisualElement root;

    public string firstLevelSceneName;

    public string firstLevelButtonName = "FirstLevelButton";

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
        var firstLevelButton = root.Q<Button>(firstLevelButtonName);
        if (firstLevelButton != null)
        {
            firstLevelButton.RegisterCallback<PointerUpEvent>(DoPlay);
        }
        else
        {
            Debug.LogWarning($"{nameof(firstLevelButton)} not found using '{firstLevelButtonName}' name.");
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
}
