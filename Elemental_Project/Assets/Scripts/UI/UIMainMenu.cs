using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using GameBase.Managers;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitBtn;

    private void Awake()
    {
        AddEventListener();
        EventSystem.current.SetSelectedGameObject(startBtn.gameObject);
    }

    private void AddEventListener()
    {
        startBtn.onClick.AddListener(StartGame);
        settingBtn.onClick.AddListener(Settings);
        exitBtn.onClick.AddListener(ExitGame);
    }

    private void StartGame()
    {
        Game_Manager.Instance.LoadSceneManually(
            GameBase.Enums.SceneType.LOBBY, 
            GameBase.Enums.TransitionType.IN);
    }

    private void Settings()
    {

    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
