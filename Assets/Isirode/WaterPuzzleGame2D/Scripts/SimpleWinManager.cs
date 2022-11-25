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

        ListenObjective();

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
        
        // TODO : extension for this kind of things
        winSection = root.Q<VisualElement>(winSectionName);
        if (winSection != null)
        {
            // winSection.RegisterCallback<PointerUpEvent>(DoPlay);
        }
        else
        {
            Debug.LogWarning($"{nameof(winSection)} not found using '{winSectionName}' name.");
        }
        winSection.style.display = DisplayStyle.None;

        looseSection = root.Q<VisualElement>(looseSectionName);
        if (looseSection != null)
        {
            // winSection.RegisterCallback<PointerUpEvent>(DoPlay);
        }
        else
        {
            Debug.LogWarning($"{nameof(looseSection)} not found using '{looseSectionName}' name.");
        }
        looseSection.style.display = DisplayStyle.None;

        var returnToLevelListButton = winSection.Q<Button>(returnToLevelListButtonName);
        if (returnToLevelListButton != null)
        {
            returnToLevelListButton.RegisterCallback<PointerUpEvent>(ReturnToList);
        }
        else
        {
            Debug.LogWarning($"{nameof(returnToLevelListButton)} not found using '{returnToLevelListButtonName}' name.");
        }

        var restartLevelButton = looseSection.Q<Button>(restartLevelButtonName);
        if (restartLevelButton != null)
        {
            restartLevelButton.RegisterCallback<PointerUpEvent>(RestartLevel);
        }
        else
        {
            Debug.LogWarning($"{nameof(restartLevelButton)} not found using '{restartLevelButtonName}' name.");
        }

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

        var globalRestartLevelButton = root.Q<Button>(globalRestartLevelButtonName);
        if (globalRestartLevelButton != null)
        {
            globalRestartLevelButton.RegisterCallback<PointerUpEvent>(RestartLevel);
        }
        else
        {
            Debug.LogWarning($"{nameof(globalRestartLevelButton)} not found using '{globalRestartLevelButtonName}' name.");
        }

        var globalReturnToLevelListButton = root.Q<Button>(globalReturnToLevelListButtonName);
        // TODO : automate this using an extension, specify the container used
        if (globalReturnToLevelListButton != null)
        {
            returnToLevelListButton.RegisterCallback<PointerUpEvent>(ReturnToList);
        }
        else
        {
            Debug.LogWarning($"{nameof(globalReturnToLevelListButton)} not found using '{globalReturnToLevelListButtonName}' name.");
        }

        RequireComponent.RequireThrow(this, () => this.lineOwner);

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
    }

    private void RestartLevel(PointerUpEvent evt)
    {
        Debug.Log(nameof(RestartLevel));

        if (awaitingForLooseCoroutine != null)
        {
            StopCoroutine(awaitingForLooseCoroutine);
            awaitingForLooseCoroutine = null;
        }

        // TODO : do an extension which do this
        winSection.style.display = DisplayStyle.None;
        looseSection.style.display = DisplayStyle.None;

        legacyInputController.enabled = true;

        // TODO : move it the line library ; possibility to have an owner should be added
        foreach (Transform child in lineOwner.transform)
        {
            Destroy(child.gameObject);
        }

        waitForDrawing.Restart();

        waterEnterObjective.ResetState();
        simpleWaterfall.ResetState(true);

        hasStarted = false;
        hasWon = false;
    }

    private void ReturnToList(PointerUpEvent evt)
    {
        LevelManager.getInstance().ReturnToLevellListScene();
    }

    private void ObjectiveReached(GameObject gameObject)
    {
        hasWon = true;

        winSection.style.display = DisplayStyle.Flex;

        legacyInputController.enabled = false;

        // TODO : optionally disable the waterfall or stop the time
    }

    private void SpawningFinished(int spawnCount)
    {
        Debug.Log(nameof(SpawningFinished));

        awaitingForLooseCoroutine = StartCoroutine(DoLooseAfterSeconds());
    }

    private IEnumerator DoLooseAfterSeconds()
    {
        yield return new WaitForSeconds(waitTimeBeforeDeclaringLostSeconds);

        // TODO : find a way to flatten the condition
        if (!hasWon && hasStarted)
        {
            looseSection.style.display = DisplayStyle.Flex;

            legacyInputController.enabled = false;
        }
        
    }

}
