using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using GameBase.Managers;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitBtn;

    private void Start()
    {
        StartCoroutine(Cor_TitleAnim(1.107f));
    }

    private IEnumerator Cor_TitleAnim(float duration)
    {
        Game_SoundManager.Instance.PlayMusic(GameBase.AudioPlayer.SoundID.GAMEPLAY, 0.3f);
        yield return new WaitForSeconds(duration);

        EnableButton();
        AddEventListener();
        EventSystem.current.SetSelectedGameObject(startBtn.gameObject);

    }

    private void EnableButton()
    {
        startBtn.gameObject.SetActive(true);
        settingBtn.gameObject.SetActive(true);
        exitBtn.gameObject.SetActive(true);
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
