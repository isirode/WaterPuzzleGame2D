using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NormalSceneManager
{
    // FIXME : make it configurable
    private string scenePathPrefix = "Assets/Isirode/WaterPuzzleGame2D/Scenes/DistanceLimiter/";
    private Regex levelNumberRegex = new Regex(@"[\w\d\/\\]{0,}Scene(\d+).unity", RegexOptions.IgnoreCase);

    public void MoveToNextLevel(int currentLevel)
    {
        // FIXME : duplicated from LevelListController

        // FIXME : this assume levels are consecutive
        int requestedLevel = currentLevel + 1;

        string firstLevelPath = string.Empty;
        int firstLevelNumber = -1;

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

                    if (firstLevelPath == string.Empty)
                    {
                        firstLevelPath = scenePath;
                        firstLevelNumber = levelNumber;
                    }
                    
                    if (levelNumber == requestedLevel) 
                    {
                        var level = new LevelListController.Level()
                        {
                            levelNumber = levelNumber,
                            levelPath = scenePath
                        };
                        LoadLevel(level);
                        return;
                    }
                }
            }
        }
        var firstLevel = new LevelListController.Level()
        {
            levelNumber = firstLevelNumber,
            levelPath = firstLevelPath
        };
        LoadLevel(firstLevel);
    }

    private void LoadLevel(LevelListController.Level level)
    {
        LevelListController.currentLevel = level;

        SceneManager.LoadScene(level.levelPath, LoadSceneMode.Single);
    }
}
