using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;
    public int backGroundLevel;
    
    private void Awake()
    {
        
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel01", 1);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        if (unlockedLevel <= 2)
        {
            for (int i = 0; i < unlockedLevel; i++)
            {
                buttons[i].interactable = true;
            }
        }
        Debug.Log("da duoc mo khoa: " + unlockedLevel);
    }
    public void openLevel(int levelID)
    {
        string levelName = "Level " + levelID;
        SceneManager.LoadScene(levelName);
    }
}
