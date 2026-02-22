using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveLoadManager : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject saveLoadPanel;
    public TextMeshProUGUI panelTitle;
    public Button[] saveLoadButton;
/*    public Button prevPageButton;
    public Button NextPageButton;*/
    public Button returnButton;

    private int day;
    private bool isSave;
    private int currentPage = Constants.DEFAULT_START_INDEX;
    private readonly int slotsPerPage = Constants.SLOTS_PER_PAGE;
    private readonly int totalSlots = Constants.TOTAL_SLOTS;
    public bool isMenu = false;

    public static SaveLoadManager Instance { get; private set; }
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
        //prevPageButton.onClick.AddListener(prevPage);
        //NextPageButton.onClick.AddListener(nextPage);
        returnButton.onClick.AddListener(GoBack);
        saveLoadPanel.SetActive(false);
        
    }

    public void ShowSaveLoadUI(bool save)
    {
        isSave = save;
        panelTitle.text = "Load Day";
        //UpdateSaveLoadUI();
        saveLoadPanel.SetActive(true);
        VNManager.Instance.gamePanel.SetActive(false);
        LoadStorylineAndScreenshots();
    }

    private void UpdateSaveLoadUI()
    {
        for (int i = 0; i < slotsPerPage; i++)
        {
            int slotIndex = currentPage * slotsPerPage + i;
            if (slotIndex < totalSlots)
            {
                saveLoadButton[i].gameObject.SetActive(true);
                saveLoadButton[i].interactable = true;

                var slotText = (slotIndex + 1) + Constants.COLON + Constants.EMPTY_SOLT;
                var textComponents = saveLoadButton[i].GetComponentsInChildren<TextMeshProUGUI>();
                textComponents[0].text = null;
                textComponents[1].text = slotText;
                saveLoadButton[i].GetComponentInChildren<RawImage>().texture = null;
            }
            else
            {
                saveLoadButton[i].gameObject.SetActive(false);
            }
        }
    }

    private void prevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateSaveLoadUI();
            LoadStorylineAndScreenshots();
        }
    }

    private void nextPage()
    {
        if ((currentPage + 1) * slotsPerPage < totalSlots)
        {
            currentPage++;
            UpdateSaveLoadUI();
            LoadStorylineAndScreenshots();
        }
    }

    private void GoBack()
    {
        saveLoadPanel.SetActive(false);
        if (isMenu)
        {
            menuPanel.SetActive(true);
        }
        else
        {
            VNManager.Instance.gamePanel.SetActive(true);
        }
    }

    private void LoadStorylineAndScreenshots()
    {
        for (int i = 0; i < saveLoadButton.Length; i++)
        {
            Debug.Log("i: " + VNManager.Instance.dayActive[i]);
            
            if (VNManager.Instance.dayActive[i])
            {
                int index = i;
                Debug.Log(index);
                saveLoadButton[index].onClick.AddListener(()=>LoadStory(index));
            }
        }
    }

    void LoadStory(int day)
    {
        MenuManager.Instance.hasStarted = true;
        saveLoadPanel.SetActive(false);
        VNManager.Instance.gamePanel.SetActive(true);
        VNManager.Instance.LoadDay(day);
    }

}
