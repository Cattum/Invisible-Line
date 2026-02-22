using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class VNManager : MonoBehaviour
{
    public GameObject gamePanel;
    public GameObject dialogueBox;
    //public GameObject historyPanel;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI speakingContent;
    public TypewriterEffector typewriterEffector;
    public Image image;
    public AudioSource vocalAudio;
    public Image backgroundImage;
    public AudioSource backgroundMusic;
    public Image CharacterImage1;
    public Image CharacterImage2;
    public RectTransform uiRoot;
    public Image flashPanel;
    public Image blackPanel;

    public List<GameObject> choicePanels;
    public List<bool> dayActive;

    public GameObject bottomButtons;
    public Button autoButton;
    public Button Skip;
    //public Button saveButton;
    public Button loadButton;
    public Button history;
    //public Button settingsButton;
    public Button homeButton;
    //public Button closeButton;
    private bool isAutoPlay = false;
    private bool isSkip = false;
    private Vector2 originalPivot;
    private Vector3 originalTrans;
   /* public GameObject choicePanel;
    public Button choiceButton1;
    public Button choiceButton2;*/

    private string storyPath = Constants.STORY_PATH;
    private string defaultStoryFileName = Constants.DEFAULT_STORY_FILE_NAME;
    private string excelFileExtention = Constants.EXCEL_FILE_EXTENSION;

    private string currentSpeakingContent;
    private List<ExcelReader.ExcelData> storyData;
    private int currentLine = Constants.DEFAULT_START_LINE;
    private LinkedList<string> historyRecords = new LinkedList<string>();

    private string[] bgSequence;
    private int bgIndex;
    private float bgTimer;
    private float bgInterval = 0.7f;
    private bool isPlayingBgSequence;

    public static VNManager Instance { get; private set; }
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
        originalPivot = uiRoot.pivot;
        originalTrans = uiRoot.transform.localPosition;
    }
    // Start is called before the first frame update
    void Start()
    {
        bottomButtonAddListener();
        
    }

    private void InitDay()
    {
        for(int i = 0; i < dayActive.Count; i++)
        {
            if (i == 0)
            {
                dayActive[i] = true;
            }
            else
            {
                dayActive[i] = false;
            }
        }
    }

    public void ReadLoad()
    {
        var loadedDays = DaySaveSystem.Load();
        if (loadedDays != null && loadedDays.Count > 0)
        {
            dayActive.Clear();
            dayActive = loadedDays;
        }
        else
        {
            InitDay();
        }
    }

    public void StartGame()
    {
        InitializeAndLoadStory(defaultStoryFileName);
        //InitDay();
    }

    private void InitializeAndLoadStory(string fileName)
    {
        Initialize();
        LoadStoryFromFile(fileName);
        //DisplayNextLine();
        if (storyData != null && storyData.Count > 0)
            DisplayNextLine();
        else
            Debug.LogError("Story not loaded correctly.");
    }

    void Initialize()
    {
        currentLine = Constants.DEFAULT_START_LINE;
        image.gameObject.SetActive(false);
        //backgroundImage.gameObject.SetActive(false);
        /*if (vocalAudio.isPlaying)
        {
            vocalAudio.Stop();
        }*/
/*        if (backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }*/
        CharacterImage1.gameObject.SetActive(false);
        CharacterImage2.gameObject.SetActive(false);
        for(int i = 0; i < choicePanels.Count; i++)
        {
            choicePanels[i].SetActive(false);
        }
        
    }

    void bottomButtonAddListener()
    {
        autoButton.onClick.AddListener(OnAutoButtonCilck);
        Skip.onClick.AddListener(OnSkipButtonCilck);
        //saveButton.onClick.AddListener(OnSaveButtonCilck);
        loadButton.onClick.AddListener(OnLoadButtonCilck);
        homeButton.onClick.AddListener(OnHomeButtonCilck);
        history.onClick.AddListener(OnHistoryButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePanel.activeSelf && Input.GetMouseButtonDown(0) && !HistoryManager.Instance.historyScrollView.activeSelf && !MenuManager.Instance.menuPanel.activeSelf)
        {
            if (!dialogueBox.activeSelf)
            {
                OpenUI();
            }
            else if (!IsHittingBottomButtons())
            {
                if (!SaveLoadManager.Instance.saveLoadPanel.activeSelf)
                {
                    DisplayNextLine();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            ResetGameProgress();
        }
        /*        if (Input.GetMouseButtonDown(0))
                {
                    if (!IsHittingBottomButtons())
                    {
                        DisplayNextLine();
                    }

                }*/
        if (isPlayingBgSequence)
        {
            bgTimer += Time.deltaTime;

            if (bgTimer >= bgInterval)
            {
                bgTimer = 0f;
                bgIndex++;

                if (bgIndex >= bgSequence.Length)
                {
                    isPlayingBgSequence = false;
                    return;
                }

                UpdateBackgroundImage(bgSequence[bgIndex]);
            }
        }
    }
    public void ResetGameProgress()
    {
        DaySaveSystem.ResetSave(dayActive.Count);
        dayActive = DaySaveSystem.Load();
    }
    void OpenUI()
    {
        dialogueBox.SetActive(true);
        bottomButtons.SetActive(true);
    }

    void CloseUI()
    {
        dialogueBox.SetActive(false);
        bottomButtons.SetActive(false);
    }

    void LoadStoryFromFile(string fileName)
    {
        Debug.Log(Constants.JSON_PATH + fileName);
        TextAsset jsonFile = Resources.Load<TextAsset>(Constants.JSON_PATH + fileName);
        if (jsonFile == null)
        {
            Debug.LogError("Story not found: " + fileName);
            return;
        }

        StoryWrapper wrapper = JsonUtility.FromJson<StoryWrapper>(jsonFile.text);
        storyData = wrapper.items;
        if (storyData == null || storyData.Count == 0)
            Debug.LogError(Constants.NO_DATA_FOUND);
        /*var path = storyPath + fileName + excelFileExtention;
        storyData = ExcelReader.ReadExcel(path);
        if (storyData == null || storyData.Count == 0)
        {
            Debug.LogError(Constants.NO_DATA_FOUND);
        }*/
    }

    [System.Serializable]
    public class StoryWrapper
    {
        public List<ExcelReader.ExcelData> items;
    }

    void DisplayNextLine()
    {
        if (storyData == null || storyData.Count == 0)
        {
            Debug.LogWarning("Story data not loaded yet.");
            return;
        }
        if (currentLine == storyData.Count - 1) 
        {
            if (storyData[currentLine].speaker == Constants.CHOICE)
            {
                ShowChoice();
                return;
            }
            if (storyData[currentLine].speaker == Constants.CONTINUE)
            {
                Continue();
                return;
            }
            if (storyData[currentLine].speaker == Constants.DAY_END)
            {
                EndDay();
            }
            /*if (storyData[currentLine].speaker == Constants.END_OF_STORY)
            {
                Debug.Log(Constants.END_OF_STORY);
                return;
            }*/
            if(storyData[currentLine].speaker== Constants.END)
            {
                MenuManager.Instance.menuPanel.SetActive(true);
                Debug.Log("END OF STORY");
                return;
            }
        }
        if (typewriterEffector.IsTyping())
        {
            
            typewriterEffector.CompleteLine();
        }
        else
        {
            DisplayThisLine();
        }
        
    }

    private void EndDay()
    {
        var data = storyData[currentLine];
        if (data.content != "")
        {
            dayActive[int.Parse(data.content) + 1] = true;
        }
        //InitializeAndLoadStory(data.avatarImageFileName);

        DaySaveSystem.Save(dayActive);

        InitializeAndLoadStory(data.avatarImageFileName);
    }

    private void Continue()
    {
        var data = storyData[currentLine];
        InitializeAndLoadStory(data.content);
    }

    private void ShowChoice()
    {
        var data = storyData[currentLine];
        int numChoice = int.Parse(data.content)-1;
        GameObject Panel = choicePanels[numChoice];
        Button[] buttons = Panel.GetComponentsInChildren<Button>();
        Debug.Log(buttons.Length);
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }
        Panel.SetActive(true);
        if (numChoice == 0)
        {
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = data.avatarImageFileName;
            buttons[0].onClick.AddListener(() => InitializeAndLoadStory(data.vocalAudioFileName));
        }
        if (numChoice == 1)
        {
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = data.avatarImageFileName;
            buttons[0].onClick.AddListener(() => InitializeAndLoadStory(data.vocalAudioFileName));
            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = data.backgroundImageFileName;
            buttons[1].onClick.AddListener(() => InitializeAndLoadStory(data.backgroundMusicFileName));
        }
        if (numChoice == 2)
        {
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = data.avatarImageFileName;
            buttons[0].onClick.AddListener(() => InitializeAndLoadStory(data.vocalAudioFileName));
            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = data.backgroundImageFileName;
            buttons[1].onClick.AddListener(() => InitializeAndLoadStory(data.backgroundMusicFileName));
            buttons[2].GetComponentInChildren<TextMeshProUGUI>().text = data.character1Action;
            buttons[2].onClick.AddListener(() => InitializeAndLoadStory(data.CoordinateX1));
        }

       /* choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();
        choicePanel.SetActive(true);
        choiceButton1.GetComponentInChildren<TextMeshProUGUI>().text = data.content;
        choiceButton1.onClick.AddListener(() => InitializeAndLoadStory(data.avatarImageFileName));
        choiceButton2.GetComponentInChildren<TextMeshProUGUI>().text = data.vocalAudioFileName;
        choiceButton2.onClick.AddListener(() => InitializeAndLoadStory(data.backgroundImageFileName));*/
    }

    void DisplayThisLine()
    {
        backgroundImage.rectTransform.offsetMax = Vector2.zero;
        backgroundImage.rectTransform.offsetMin = Vector2.zero;
        var data = storyData[currentLine];
        speakerName.text = data.speaker;
        currentSpeakingContent = data.content;
        typewriterEffector.StartTyping(data.content);
        Debug.Log(speakerName.text);
        RecordHistory(speakerName.text, currentSpeakingContent);
        if (NotNullNorEmpty(data.avatarImageFileName))
        {
            UpdateAvatarImage(data.avatarImageFileName);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
        if (NotNullNorEmpty(data.vocalAudioFileName))
        {
            PlayVocalAudio(data.vocalAudioFileName);
        }
        if (NotNullNorEmpty(data.backgroundImageFileName))
        {
            string backgroundImageFileName;
            backgroundImageFileName = data.backgroundImageFileName.Trim().Replace("£»", ";");
            Debug.Log("backgroudchange"+ backgroundImageFileName);
            if (backgroundImageFileName.Contains(";"))
            {
                Debug.Log("multi-background");
                bgSequence = data.backgroundImageFileName.Split(';');
                bgIndex = 0;
                bgTimer = 0f;
                isPlayingBgSequence = true;
                UpdateBackgroundImage(bgSequence[0]);
            }
            else
            {
                isPlayingBgSequence = false;
                UpdateBackgroundImage(backgroundImageFileName);
            }
        }
        if (NotNullNorEmpty(data.backgroundMusicFileName))
        {
            PlayBackgroundMusic(data.backgroundMusicFileName);
        }
        if (NotNullNorEmpty(data.character1Action))
        {
            UpdateCharacterImage(data.character1Action, data.character1Image, CharacterImage1, data.CoordinateX1);
        }
        if (NotNullNorEmpty(data.character2Action))
        {
            UpdateCharacterImage(data.character2Action, data.character2Image, CharacterImage2, data.CoordinateX2);
        }
        if (NotNullNorEmpty(data.backgroundEffect))
        {
            BackgroundAnim(data.backgroundEffect, data.effectValue);
        }
        //speakingContent.text = data.content;
        currentLine++;
    }

    private void BackgroundAnim(string effect, string pos)
    {
        if (effect.StartsWith(Constants.backgroundShake))
        {
            Debug.Log("shake");
            if (pos == "slow")
            {
                ShakeWalk(Constants.slowShakeDuration, Constants.shakeStrength);
            }
            else
            {
                StartCoroutine(Shake(Constants.shakeTimes, Constants.shakeDuration, Constants.shakeStrength));
            }
            backgroundImage.rectTransform.offsetMax = Vector2.zero;
            backgroundImage.rectTransform.offsetMin = Vector2.zero;
            uiRoot.pivot = originalPivot;
        }
        if (effect.StartsWith(Constants.backgroundZoom))
        {

            var values = pos.Split(',');
            Debug.Log("zoom" + values[0]);
            float x = float.Parse(values[0]);
            float y = float.Parse(values[1]);
            zoomTo(new Vector2(x / Screen.width, y / Screen.height),Constants.zoomAmount,Constants.zoomDuration);
        }
        if (effect.StartsWith(Constants.backgroundFlash))
        {
            flashPanel.color = new Color(1, 1, 1, 0);
            flashPanel.gameObject.SetActive(true);

            Sequence s = DOTween.Sequence();
            s.Append(flashPanel.DOFade(1, Constants.flashDuration * 0.3f));
            s.Append(flashPanel.DOFade(0, Constants.flashDuration * 0.7f));
            s.OnComplete(() => flashPanel.gameObject.SetActive(false));
            uiRoot.pivot = originalPivot;
        }
        if (effect.StartsWith(Constants.backgroundFadeOut))
        {
            blackPanel.DOFade(1f, Constants.fadeDuration);
            uiRoot.pivot = originalPivot;
        }
        if (effect.StartsWith(Constants.backgroundFadeIn))
        {
            blackPanel.DOFade(0f, Constants.fadeDuration);
            uiRoot.pivot = originalPivot;
        }
        if (effect.StartsWith(Constants.backgroundResetZoom))
        {
            ResetZoom(Constants.zoomDuration);
            uiRoot.pivot = originalPivot;
        }
        uiRoot.transform.localPosition = originalTrans;
    }

    private void ResetZoom(float duration)
    {
        uiRoot.DOScale(1f, duration);
        uiRoot.pivot = originalPivot;
    }

    public IEnumerator Shake(int times, float duration, float strength)
    {
        Vector3 originalPos = transform.localPosition;

        for (int i = 0; i < times; i++)
        {
            uiRoot.DOShakeAnchorPos(duration, strength, 20, 90, false, true);

            transform.localPosition = originalPos;
            backgroundImage.rectTransform.offsetMax = Vector2.zero;
            backgroundImage.rectTransform.offsetMin = Vector2.zero;
        }
        yield return null;
        transform.localPosition = originalPos;
        backgroundImage.rectTransform.offsetMax = Vector2.zero;
        backgroundImage.rectTransform.offsetMin = Vector2.zero;
    }

    public void ShakeWalk(float duration, float strength)
    {
        Vector2 originalPos = uiRoot.anchoredPosition;

        uiRoot.DOShakeAnchorPos(duration, 10, 5, 180, false, true)
            .OnComplete(() => {
            uiRoot.anchoredPosition = originalPos;

            backgroundImage.rectTransform.offsetMin = Vector2.zero;
                backgroundImage.rectTransform.offsetMax = Vector2.zero;
            });
    }

    private void zoomTo(Vector2 screenPos, float zoomAmount, float duration)
    {
        uiRoot.DOScale(zoomAmount, duration);

        Vector2 pivot = new Vector2(screenPos.x, screenPos.y);
        uiRoot.pivot = pivot;
    }

    private void UpdateCharacterImage(string character1Action, string character1Image, Image characterImage, string x)
    {
        if (character1Action.StartsWith(Constants.characterActionAppearAt))
        {
            string imagePath = Constants.CHARACTER_PATH + character1Image;
            if (Input.GetMouseButton(0))
            {
                UpdateImage(imagePath, characterImage);
            }
            if (NotNullNorEmpty(x))
            {
                UpdateImage(imagePath, characterImage);
                var newPosition = new Vector2(float.Parse(x), characterImage.rectTransform.anchoredPosition.y);
                characterImage.rectTransform.anchoredPosition = newPosition;
                characterImage.DOFade(1, Constants.DURATION_TIME).From(0);
            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }
        }
        else if (character1Action == Constants.characteraActionDisappear)
        {
            Debug.Log("Disappear: " + characterImage.name);
            characterImage.DOFade(0,Constants.DURATION_TIME).OnComplete(()=>characterImage.gameObject.SetActive(false));
        }
        else if (character1Action.StartsWith(Constants.characyerActionMoveTo))
        {
            if (NotNullNorEmpty(x))
            {
                characterImage.rectTransform.DOAnchorPosX(float.Parse(x), Constants.DURATION_TIME);
            }
            else
            {
                Debug.LogError(Constants.COORDINATE_MISSING);
            }
        }
    }

    private void UpdateImage(string imagePath, Image characterImage)
    {
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            characterImage.sprite = sprite;
            if (!characterImage.gameObject.activeSelf)
            {
                characterImage.gameObject.SetActive(true);
            }
            Debug.Log("Appear: " + sprite.name);
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
        }
    }

    void RecordHistory(string speaker, string content)
    {
        string historyRecord = "";
        if (speaker!="")
        {
            speaker = $"<b>{speaker}</b>";
            Debug.Log("Speaker"+speaker);
            historyRecord = speaker + Constants.COLON + content;
        }
        else if (content == "")
        {
            historyRecord = "";
        }
        else
        {
            historyRecord= content;
        }
        //Debug.Log(historyRecord);
        if (historyRecord != "")
        {
            if (historyRecords.Count >= Constants.MAX_LENGTH)
            {
                historyRecords.RemoveFirst();
            }
            if (historyRecord != null)
            {
                historyRecords.AddLast(historyRecord);
            }
        }
    }

    private void PlayBackgroundMusic(string backgroundMusicFileName)
    {
        if (backgroundMusicFileName == "stop")
        {
            if (backgroundMusic.isPlaying)
            {
                backgroundMusic.Stop();
            }
        }
        else
        {
            string audioPath = Constants.MUSIC_PATH + backgroundMusicFileName;
            AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
            Debug.Log("audio play:" + audioClip.name);
            if (audioClip != null)
            {
                backgroundMusic.clip = audioClip;
                backgroundMusic.Play();
                backgroundMusic.loop = true;
            }
            else
            {
                Debug.LogError(Constants.MUSIC_LOAD_FAILED + audioPath);
            }
        }
    }

    private void UpdateBackgroundImage(string backgroundImageFileName)
    {
        string imagePath = Constants.BACKGROUND_PATH + backgroundImageFileName;
        Debug.Log(backgroundImageFileName);
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            backgroundImage.sprite = sprite;
            backgroundImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
        }
    }

    private void PlayVocalAudio(string vocalAudioFileName)
    {
        string audioPath = Constants.VOCAL_PATH + vocalAudioFileName;
        AudioClip audioClip = Resources.Load<AudioClip>(audioPath);
        if (audioClip != null)
        {
            vocalAudio.clip = audioClip;
            vocalAudio.Play();
        }
        else
        {
            Debug.LogError(Constants.AUDIO_LOAD_FAILED + audioPath);
        }
    }

   

    private void UpdateAvatarImage(string avatarImageFileName)
    {
        string imagePath = Constants.AVATAR_PATH + avatarImageFileName;
        Sprite sprite = Resources.Load<Sprite>(imagePath);
        if (sprite != null)
        {
            image.sprite = sprite;
            image.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError(Constants.IMAGE_LOAD_FAILED + imagePath);
        }
    }

    private bool NotNullNorEmpty(string str)
    {
        return !string.IsNullOrEmpty(str);
    }

    bool IsHittingBottomButtons()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            bottomButtons.GetComponent<RectTransform>(),
            Input.mousePosition,
            null);
    }

    void OnHistoryButtonClick()
    {
        HistoryManager.Instance.ShowHistory(historyRecords);
    }

    void OnSkipButtonCilck()
    {
        isSkip = !isSkip;
        UpdateBottonImage((isSkip ? Constants.SKIP_ON : Constants.SKIP_OFF), Skip);
        if (isSkip)
        {
            typewriterEffector.waitingSeconds = Constants.SKIP_MODE_WAITING_SECONDS;
            Debug.Log("Skip");
            StartCoroutine(StartSkipPlay());
        }
        else
        {
            Debug.Log("Stop Skip");
            typewriterEffector.waitingSeconds = Constants.DEFAULT_WAITING_SECONDS;
        }
    }

    private IEnumerator StartSkipPlay()
    {
        while (isSkip)
        {
            if (!typewriterEffector.IsTyping())
            {
                DisplayNextLine();
            }
            yield return new WaitForSeconds(Constants.SKIP_MODE_WAITING_SECONDS);
        }


    }
    void OnAutoButtonCilck()
    {
        isAutoPlay = !isAutoPlay;
        Debug.Log("Cilck Auto: "+isAutoPlay);
        UpdateBottonImage((isAutoPlay ? Constants.AUTO_ON : Constants.AUTO_OFF), autoButton);
        if (isAutoPlay)
        {
            StartCoroutine(StartAutoPlay());
        }
    }

    void UpdateBottonImage(string imageFileName, Button button)
    {
        string imagePath = Constants.BUTTON_PATH + imageFileName;
        UpdateImage(imagePath, button.image);
    }

    private IEnumerator StartAutoPlay()
    {
        while (isAutoPlay)
        {
            if (!typewriterEffector.IsTyping())
            {
                DisplayNextLine();
            }
            yield return new WaitForSeconds(Constants.AUTO_MODE_WAITING_SECONDS);
        }


    }

    void OnHomeButtonCilck()
    {
        gamePanel.SetActive(false);
        MenuManager.Instance.menuPanel.SetActive(true);
    }

    void OnSaveButtonCilck()
    {
        SaveLoadManager.Instance.ShowSaveLoadUI(true);
    }

    void OnLoadButtonCilck()
    {
        SaveLoadManager.Instance.isMenu = false;
        SaveLoadManager.Instance.ShowSaveLoadUI(false);
    }

    public void LoadDay(int day)
    {
        if (day == 0)
        {
            Debug.Log("Load Day1");
            InitializeAndLoadStory(Constants.DAY_1);
        }
        if (day == 1)
        {
            Debug.Log("Load Day2");
            InitializeAndLoadStory(Constants.DAY_2);
        }
        if (day == 2)
        {
            InitializeAndLoadStory(Constants.DAY_3);
        }
        if (day == 3)
        {
            InitializeAndLoadStory(Constants.DAY_4);
        }
        if (day == 4)
        {
            InitializeAndLoadStory(Constants.DAY_5);
        }
        if (day == 5)
        {
            InitializeAndLoadStory(Constants.DAY_6);
        }
        if (day == 6)
        {
            InitializeAndLoadStory(Constants.DAY_7);
        }

    }
}
