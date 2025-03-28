using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);      
    }

    public enum Scene
    {
        MainMenu,
        BattleScene
    }

    public void LoadScene(Scene scene)
    {
        //Debug.Log(scene.ToString());
        SceneManager.LoadScene(scene.ToString());
    }

    public void LoadMainMenu()
    {
        LoadScene(Scene.MainMenu);
    }

    public void LoadBattleScene()
    {
        LoadScene(Scene.BattleScene);
    }
}
