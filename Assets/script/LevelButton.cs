using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour
{
    public void goToNextLevel()
    {
        int completedLevel = PlayerPrefs.GetInt("inLevel", 1);
        int nextLevel = completedLevel + 1;

        if (nextLevel <= 9 )
        {
            string nextSceneName = "Level " + nextLevel;
            SceneManager.LoadScene(nextSceneName);
            Debug.Log(nextLevel);
        }
    }

    public void rePlayLevel()
    {
        int LevelAgain = PlayerPrefs.GetInt("inLevel", 1);
        string Levelback = "Level " + LevelAgain;
        SceneManager.LoadScene(Levelback);
        Debug.Log(LevelAgain);
        
    }
}
