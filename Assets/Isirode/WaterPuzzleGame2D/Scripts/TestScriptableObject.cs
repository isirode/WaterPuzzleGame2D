using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TestScriptableObject", order = 1)]
public class TestScriptableObject : ScriptableObject
{
    public string stringstuff;
    // not possible
    public Scene sceneReference;
    // only the name
    public SceneAsset sceneAsset;
}
 