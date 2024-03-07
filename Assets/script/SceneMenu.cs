using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneMenu : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void onClickPlay()
    {
        SceneManager.LoadScene(1);
    }

    public void onClickReturnInChoseBackGround()
    {
        SceneManager.LoadScene(0);
    }
    public void onClickReturnInChoseLevel()
    {
        SceneManager.LoadScene(1);
    }
    public void onClickInNextLevel()
    {
        SceneManager.LoadScene(2);
    }
    public void onClickReplay()
    {
        SceneManager.LoadScene(1);
    }
    public void onClickExit()
    {
        Application.Quit();
    }
    public void onClickMoveBackMenu()
    {
        SceneManager.LoadScene(0);    
    }



    public void onClickBG1()
    {
        SceneManager.LoadScene(2);
    }
    public void onClickBG2()
    {
        SceneManager.LoadScene(3);
    }
    public void onClickBG3()
    {
        SceneManager.LoadScene(4);
    }
}
