using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    #region Fields

    [SerializeField] private TextMeshProUGUI timerText;
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button minusButton;
    [SerializeField] private Button plusButton;
    [Header("Play/Pause sprites")]
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    private float timeLeft = 60;
    private bool isTimerOff = true;
    private Coroutine timer;
    private Image playImage;

    #endregion
    
    #region Unity Lifecycle

    private void Start()
    {
        SubscribeOnEvents();
        playImage = playButton.GetComponent<Image>();
        UpdateTimeText();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    #region Event Registry

    private void SubscribeOnEvents()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        minusButton.onClick.AddListener(OnMinusButtonClick);
        plusButton.onClick.AddListener(OnPlusButtonClick);
    }

    private void UnsubscribeFromEvents()
    {
        playButton.onClick.RemoveAllListeners();
        minusButton.onClick.RemoveAllListeners();
        plusButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Methods

    private void OnPlayButtonClick()
    {
        if (isTimerOff)
        {
            timer = StartCoroutine(StartTimer());
            isTimerOff = false;
            playImage.sprite = pauseSprite;
        }
        else
        {
            StopCoroutine(timer);
            isTimerOff = true;
            playImage.sprite = playSprite;
        }
    }

    private void OnPlusButtonClick()
    {
        timeLeft += 60f;
        UpdateTimeText();
    }

    private void OnMinusButtonClick()
    {
        timeLeft -= 60f;
        UpdateTimeText();
    }
    
    private IEnumerator StartTimer()
    {
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimeText();
            yield return null;
        }
        isTimerOff = true;
        playImage.sprite = playSprite;
        //TODO: add flashlights disabling
    }
 
    private void UpdateTimeText()
    {
        if (timeLeft < 0) timeLeft = 0;
 
        float minutes = Mathf.FloorToInt(timeLeft / 60);
        float seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    #endregion
}