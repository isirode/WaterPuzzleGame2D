using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartSceneController : MonoBehaviour
{
    public UIDocument document;
    private VisualElement root;

    // FIXME : rename it to LevelList ?
    public string playSceneName;

    public string playButtonName = "PlayButton";
    public string settingsButtonName = "SettingsButton";

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
        var playButton = root.Q<Button>(playButtonName);
        if (playButton != null) 
        {
            playButton.RegisterCallback<PointerUpEvent>(DoPlay);
        }
        else
        {
            Debug.LogWarning($"{nameof(playButton)} not found using '{playButtonName}' name.");
        }

        // FIXME : is this the better place for this
        var levelManager = LevelManager.getInstance();
        levelManager.levelListSceneName = playSceneName;
    }

    private void DoPlay(PointerUpEvent evt)
    {
        Debug.Log("Play");
        if (string.IsNullOrEmpty(playSceneName))
        {
            // TODO : do the checks in the Start method
            Debug.LogWarning($"{nameof(playSceneName)} is null or empy.");
            return;
        }

        SceneManager.LoadScene(playSceneName, LoadSceneMode.Single);
    }
}
