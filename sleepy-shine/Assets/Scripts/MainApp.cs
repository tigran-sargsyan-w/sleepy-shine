using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainApp : MonoBehaviour
{
    #region Fields

    [SerializeField] private Timer timerPanel;
    
    [Header("Images")]
    [SerializeField] private Image background;
    [SerializeField] private Image mainIconImage;
    [Header("Buttons")]
    [SerializeField] private Button timerButton;
    [SerializeField] private Button flashlightButton;
    [SerializeField] private Button backgroundChangeButton;
    [SerializeField] private Button settingsButton;

    [Header("Flashlight sprites")]
    [SerializeField] private Sprite flashlightOnSprite;
    [SerializeField] private Sprite flashlightOffSprite;

    [SerializeField] private float colorChangeTime = 1f;

    private bool isFlashlightOff = true;
    private bool isBackgroundColorChanging = true;
    private bool isTimerDeactivated = true;
    
    private Sequence backgroundSequence;
    private Sequence iconSequence;
    private Image flashlightImage;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        AnimateBackground();

        AnimateMainIcon();

        SubscribeOnEvents();
        flashlightImage = flashlightButton.GetComponent<Image>();
        
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    #endregion

    #region Event Registry

    private void SubscribeOnEvents()
    {
        flashlightButton.onClick.AddListener(OnFlashlightButtonClick);
        timerButton.onClick.AddListener(OnTimerButtonClick);
        backgroundChangeButton.onClick.AddListener(OnBackgroundChangeButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
    }

    private void UnsubscribeFromEvents()
    {
        flashlightButton.onClick.RemoveAllListeners();
        timerButton.onClick.RemoveAllListeners();
        backgroundChangeButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
    }

    #endregion

    #region Methods

    private void AnimateMainIcon()
    {
        iconSequence = DOTween.Sequence();
        iconSequence.Append(mainIconImage.transform.DOScale(1.2f, colorChangeTime))
            .Join(mainIconImage.transform.DORotate(new Vector3(0, 0, 360), colorChangeTime, RotateMode.FastBeyond360))
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void AnimateBackground()
    {
        backgroundSequence = DOTween.Sequence();
        backgroundSequence.Append(background.DOColor(BaseColors.color1, colorChangeTime))
            .Append(background.DOColor(BaseColors.color2, colorChangeTime))
            .Append(background.DOColor(BaseColors.color3, colorChangeTime))
            .Append(background.DOColor(BaseColors.color4, colorChangeTime))
            .Append(background.DOColor(BaseColors.color5, colorChangeTime))
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnTimerButtonClick()
    {
        if (isTimerDeactivated)
        {
            timerPanel.gameObject.SetActive(true);
            isTimerDeactivated = false;
        }
        else
        {
            timerPanel.gameObject.SetActive(false);
            isTimerDeactivated = true;
        }
    }

    private void OnFlashlightButtonClick()
    {
        if (isFlashlightOff)
        {
#if PLATFORM_ANDROID
            Flashlight.Enable();
#endif
            flashlightImage.sprite = flashlightOnSprite;
            isFlashlightOff = false;
        }
        else
        {
#if PLATFORM_ANDROID
            Flashlight.Disable();
#endif
            flashlightImage.sprite = flashlightOffSprite;
            isFlashlightOff = true;
        }
    }

    private void OnBackgroundChangeButtonClick()
    {
        if (isBackgroundColorChanging)
        {
            backgroundSequence.Pause();
            isBackgroundColorChanging = false;
        }
        else
        {
            backgroundSequence.Play();
            isBackgroundColorChanging = true;
        }
    }

    private void OnSettingsButtonClick()
    {
        Debug.Log("Settings button clicked");
    }

    #endregion
}