using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaitForDrawing : MonoBehaviour
{
    public UIDocument document;
    private VisualElement root;

    public string startDrawingSectionName = "StartDrawingSection";

    public BasicLineController basicLineController;

    public SimpleWaterfall simpleWaterfall;

    // TODO : add a property gameobject to disable
    // and a way to just top them

    public bool initOnStart = true;
    private bool wasInit = false;

    void Start()
    {
        if (initOnStart)
        {
            Init();
        }
    }

    public void Init()
    {
        if (wasInit)
        {
            return;
        }
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
        if (basicLineController == null)
        {
            basicLineController = GetComponent<BasicLineController>();
            if (basicLineController == null)
            {
                Debug.LogWarning($"{nameof(basicLineController)} is not set.");
                return;
            }
        }
        basicLineController.LineAdded += LineAdded;

        if (simpleWaterfall != null)
        {
            simpleWaterfall.enabled = false;
        }
        else
        {
            Debug.LogWarning($"{nameof(simpleWaterfall)} is not set.");
        }

        wasInit = true;
    }

    private void LineAdded(List<Vector3> points, GameObject gameObject)
    {
        basicLineController.enabled = false;

        StartWaterfall();
    }

    public void Restart()
    {
        basicLineController.enabled = true;

        StopWaterfall();
    }

    private void StartWaterfall()
    {
        if (simpleWaterfall != null)
        {
            simpleWaterfall.enabled = true;
            simpleWaterfall.Restart();
        }
        else
        {
            Debug.LogWarning($"{nameof(simpleWaterfall)} is not set.");
        }
    }

    private void StopWaterfall()
    {
        simpleWaterfall.enabled = false;
        simpleWaterfall.ResetState(true);
    }
}
