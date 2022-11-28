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

    void Start()
    {
        RequireComponent.AssignIfNotSet(this, ref document);
        RequireComponent.RequireThrow(this, document);

        root = document.rootVisualElement;

        var playButton = root.QR<Button>(playButtonName);
        playButton.RegisterCallback<PointerUpEvent>(DoPlay);

        // FIXME : is this the better place for this
        var levelManager = LevelManager.getInstance();
        levelManager.levelListSceneName = playSceneName;
    }

    private void DoPlay(PointerUpEvent evt)
    {
        LevelManager.getInstance().GoToLevellListScene();
    }
}
