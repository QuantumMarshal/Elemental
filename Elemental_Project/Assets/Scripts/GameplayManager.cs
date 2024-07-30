using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [SerializeField] private GameObject player;
    private GameObject playerObject;
    [SerializeField] private PlayerController playerController;
    private PlayerInputAction playerInputAction;

    [SerializeField] private UIGameplayManager uIGameplayManager;
    public bool showPauseUI = false;

    [SerializeField] private ProceduralGenerator proceduralGenerator;
    private List<Room> rooms;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public void ChangeFollowTarget(Transform target)
    {
        virtualCamera.Follow = target;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Setup();
    }

    private void OnDestroy()
    {
        playerInputAction.PlayerUI.Pause.started -= Pause;
        Destroy(playerObject);
    }

    private void Setup()
    {
        proceduralGenerator.Generate();

        rooms = proceduralGenerator.finalRooms;

        playerObject = Instantiate(player, rooms[0].centerPos, Quaternion.identity);
        ChangeFollowTarget(playerObject.transform);

        PlayerController playerController = playerObject.GetComponentInChildren<PlayerController>();

        playerInputAction = playerController.PlayerInputAction;
        playerInputAction.PlayerUI.Pause.started += Pause;

        uIGameplayManager.ShowHUD();
        uIGameplayManager.SetHUD(playerController.config);

    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (!showPauseUI) uIGameplayManager.Show();
        else uIGameplayManager.ContinueGame();
    }
}
