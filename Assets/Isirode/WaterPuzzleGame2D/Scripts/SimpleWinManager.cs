using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// FIXME : rename it, I was not inspired for the name
public class SimpleWinManager : MonoBehaviour
{
    public WaterEnterObjective waterEnterObjective;

    public UIDocument document;
    private VisualElement root;

    VisualElement winSection;
    public string winSectionName = "WinSection";

    private bool hasStarted = false;
    private bool hasWon = false;

    VisualElement looseSection;
    public string looseSectionName = "LooseSection";

    public float waitTimeBeforeDeclaringLostSeconds = 5f;

    public string returnToLevelListButtonName = "ReturnToLevelListButton";
    public string nextLevelButtonName = "NextLevelButton";

    public string restartLevelButtonName = "RestartLevelButton";

    public string globalRestartLevelButtonName = "GRestartLevel";
    public string globalReturnToLevelListButtonName = "GReturnToLevelList";

    public LegacyInputController legacyInputController;

    public SimpleWaterfall simpleWaterfall;

    public WaitForDrawing waitForDrawing;

    public GameObject lineOwner;

    public bool startWithInit = true;
    private bool wasInit = false;

    private Coroutine awaitingForLooseCoroutine;

    public delegate void LevelWonDelegate();
    public event LevelWonDelegate LevelWon;

    public delegate void NextLevelRequestDelegate();
    public event NextLevelRequestDelegate NextLevelRequested;

    private NormalSceneManager normalSceneManager = new NormalSceneManager();

    private void Start()
    {
        if (startWithInit)
        {
            Init();
        }
    }

    public void Init()
    {
        Debug.Log(nameof(Init));

        if (wasInit)
        {
            return;
        }

        RequireComponent.AssignIfNotSet(this, ref document);
        RequireComponent.RequireThrow(this, document);

        RequireComponent.RequireNotEmptyThrow(this, winSectionName);
        RequireComponent.RequireNotEmptyThrow(this, looseSectionName);
        RequireComponent.RequireNotEmptyThrow(this, returnToLevelListButtonName);
        RequireComponent.RequireNotEmptyThrow(this, nextLevelButtonName);
        RequireComponent.RequireNotEmptyThrow(this, restartLevelButtonName);
        RequireComponent.RequireNotEmptyThrow(this, globalRestartLevelButtonName);
        RequireComponent.RequireNotEmptyThrow(this, globalReturnToLevelListButtonName);

        ListenObjective();

        root = document.rootVisualElement;

        // TODO : extension for this kind of things
        winSection = root.QR<VisualElement>(winSectionName);
        winSection.style.display = DisplayStyle.None;

        looseSection = root.QR<VisualElement>(looseSectionName);
        looseSection.style.display = DisplayStyle.None;

        var returnToLevelListButton = winSection.QR<Button>(returnToLevelListButtonName);
        returnToLevelListButton.RegisterCallback<PointerUpEvent>(ReturnToLevelList);

        var nextLeveLButton = winSection.QR<Button>(nextLevelButtonName);
        nextLeveLButton.RegisterCallback<PointerUpEvent>(NextLevel);

        var restartLevelButton = looseSection.QR<Button>(restartLevelButtonName);
        restartLevelButton.RegisterCallback<PointerUpEvent>(RestartLevel);

        if (legacyInputController == null)
        {
            Debug.LogWarning($"{nameof(legacyInputController)} is not set.");
        }
        legacyInputController.LineFinished += LineFinished;

        ListenWaterfall();

        if (waitForDrawing == null)
        {
            Debug.LogWarning($"{nameof(waitForDrawing)} is not set.");
        }

        var globalRestartLevelButton = root.QR<Button>(globalRestartLevelButtonName);
        globalRestartLevelButton.RegisterCallback<PointerUpEvent>(RestartLevel);

        var globalReturnToLevelListButton = root.QR<Button>(globalReturnToLevelListButtonName);
        globalReturnToLevelListButton.RegisterCallback<PointerUpEvent>(ReturnToLevelList);

        RequireComponent.RequireThrow(this, this.lineOwner);

        wasInit = true;
    }

    public void ListenWaterfall()
    {
        if (simpleWaterfall == null)
        {
            Debug.LogWarning($"{nameof(simpleWaterfall)} should not be null for the {nameof(SimpleWinManager)} to work");
        }
        else
        {
            simpleWaterfall.SpawningFinished += SpawningFinished;
        }
    }

    public void ListenObjective()
    {
        if (waterEnterObjective == null)
        {
            Debug.LogWarning($"{nameof(waterEnterObjective)} should not be null for the {nameof(SimpleWinManager)} to work");
        }
        else
        {
            waterEnterObjective.ObjectiveReached += ObjectiveReached;
        }
    }

    // TODO : use another class than legacyInputController for this maybe
    private void LineFinished(List<Vector3> points)
    {
        hasStarted = true;

        legacyInputController.enabled = false;
    }

    private void RestartLevel(PointerUpEvent evt)
    {
        Debug.Log(nameof(RestartLevel));

        RestartLevel();
    }

    public void RestartLevel()
    {
        StopCoroutines();

        HidePopups();

        legacyInputController.enabled = true;

        ClearDrawings();

        waitForDrawing.Restart();

        ResetObjectives();
        ResetWaterfalls();

        hasStarted = false;
        hasWon = false;

        Debug.Log($"coroutine : {awaitingForLooseCoroutine}");
    }

    private void ReturnToLevelList(PointerUpEvent evt)
    {
        Debug.Log(nameof(ReturnToLevelList));
        LevelManager.getInstance().GoToLevellListScene();
    }

    // FIXME : it is done this way for now so that there are not circular references between the additive level manager and this class
    // And we need an interface system 
    private void NextLevel(PointerUpEvent evt)
    {
        // Clearing the scene
        ClearDrawings();
        ResetObjectives();
        ResetWaterfalls();
        HidePopups();
        StopCoroutines();

        NextLevelRequested?.Invoke();

        // FIXME : this is a quickfix
        if (NextLevelRequested == null)
        {
            normalSceneManager.MoveToNextLevel(LevelListController.currentLevel.levelNumber);
        }
    }

    private void ObjectiveReached(GameObject gameObject)
    {
        hasWon = true;

        winSection.style.display = DisplayStyle.Flex;

        legacyInputController.enabled = false;

        LevelWon?.Invoke();

        // TODO : optionally disable the waterfall or stop the time
        ResetObjectives();
        StopCoroutines();
    }

    private void SpawningFinished(int spawnCount)
    {
        Debug.Log(nameof(SpawningFinished));

        if (awaitingForLooseCoroutine != null)
        {
            Debug.LogWarning($"{nameof(awaitingForLooseCoroutine)} is not null but should be ({awaitingForLooseCoroutine})");
            return;
        }

        awaitingForLooseCoroutine = StartCoroutine(DoLooseAfterSeconds());
    }

    private IEnumerator DoLooseAfterSeconds()
    {
        Debug.Log(nameof(DoLooseAfterSeconds) + "1");
        yield return new WaitForSeconds(waitTimeBeforeDeclaringLostSeconds);
        Debug.Log(nameof(DoLooseAfterSeconds) + "2");

        // TODO : find a way to flatten the condition
        if (!hasWon && hasStarted)
        {
            looseSection.style.display = DisplayStyle.Flex;

            legacyInputController.enabled = false;
        }
    }

    // TODO : move it the line library ; possibility to have an owner should be added
    private void ClearDrawings()
    {
        foreach (Transform child in lineOwner.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void ResetWaterfalls()
    {
        simpleWaterfall.ResetState(true);
    }

    private void ResetObjectives()
    {
        waterEnterObjective.ResetState();
    }

    private void HidePopups()
    {
        // TODO : do an extension which do this
        winSection.style.display = DisplayStyle.None;
        looseSection.style.display = DisplayStyle.None;
    }

    private void StopCoroutines()
    {
        if (awaitingForLooseCoroutine != null)
        {
            StopCoroutine(awaitingForLooseCoroutine);
            awaitingForLooseCoroutine = null;
        }
    }

}
