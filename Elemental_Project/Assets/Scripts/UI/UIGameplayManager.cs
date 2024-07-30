using GameBase.Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayManager : MonoBehaviour
{
    [SerializeField] private GameObject UIPauseMenu;
    [SerializeField] private GameObject HUD;
    [SerializeField] private Button continueBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitBtn;

    private void Start()
    {
        AddEventListener();
        
    }

    private void AddEventListener()
    {
        continueBtn.onClick.AddListener(ContinueGame);
        settingBtn.onClick.AddListener(Settings);
        exitBtn.onClick.AddListener(ExitGame);
    }

    public void ContinueGame()
    {
        GameplayManager.Instance.showPauseUI = false;
        UIPauseMenu.SetActive(false);
        HUD.SetActive(true);
        Time.timeScale = 1f;
    }

    private void Settings()
    {

    }

    private void ExitGame()
    {
        GameplayManager.Instance.showPauseUI = false;
        HUD.SetActive(false);
        Game_Manager.Instance.LoadSceneManually(GameBase.Enums.SceneType.MAINMENU, GameBase.Enums.TransitionType.IN);
    }

    public void Show()
    {
        GameplayManager.Instance.showPauseUI = true;
        UIPauseMenu.SetActive(true);
        HUD.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowHUD()
    {
        HUD.SetActive(true);
    }

    public void SetHUD(PlayerConfig playerConfig)
    {
        HUD.GetComponent<UIHUDManager>().SetHUD(playerConfig);
    }
}
