using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Info : we do not SceneManager.LoadScene directly so that the scene's name is set only once
// Note : there is no reason to not have a factory scene names in the C# code
/// <summary>
/// Level manager for the common scenes (Settings, Home page, levels list etc)
/// </summary>
public class LevelManager
{
    private static LevelManager instance;

    public static LevelManager getInstance()
    {
        if (instance == null)
        {
            instance = new LevelManager();
        }
        return instance;
    }

    // TODO : make it private
    public string levelListSceneName = string.Empty;

    public void ReturnToLevellListScene()
    {
        if (levelListSceneName == string.Empty)
        {
            // TODO : add a log here
            // we cannot use Debug.Log
            return;
        }
        SceneManager.LoadScene(levelListSceneName, LoadSceneMode.Single);
    }

}
