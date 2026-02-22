using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject menuPanel;
    public Button startButton;
    public Button Continue;
    public Button loadButton;
    public Button settingsButton;
    public Button quitButton;

    public bool hasStarted = false;
    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        menuButtonAddListener();
        VNManager.Instance.ReadLoad();
    }

    private void menuButtonAddListener()
    {
        startButton.onClick.AddListener(StartGame);
        Continue.onClick.AddListener(ContinueGame);
        loadButton.onClick.AddListener(LoadGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        hasStarted = true;
        menuPanel.SetActive(false);
        VNManager.Instance.gamePanel.SetActive(true);
        //VNManager.Instance.ReadLoad();
        VNManager.Instance.StartGame();
    }

    private void ContinueGame()
    {
        if (hasStarted)
        {
            Debug.Log("Continue");
            menuPanel.SetActive(false);
            VNManager.Instance.gamePanel.SetActive(true);
        }
    }
    private void LoadGame()
    {
        menuPanel.SetActive(false);
        SaveLoadManager.Instance.ShowSaveLoadUI(false);
        SaveLoadManager.Instance.isMenu = true;
    }

    private void QuitGame()
    {
        GameQuit.ExitGame();
    }
}
