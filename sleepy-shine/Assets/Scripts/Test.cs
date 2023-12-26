using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Button TimerButton;
    public Button FlashlightButton;
    public Button BackgroundChangeButton;
    public Button SettingsButton;
    
    private bool isFlashlightOff = true;
    
    private void Start()
    {
        FlashlightButton.onClick.AddListener(OnFlashlightButtonClick);
        TimerButton.onClick.AddListener(OnTimerButtonClick);
        BackgroundChangeButton.onClick.AddListener(OnBackgroundChangeButtonClick);
        SettingsButton.onClick.AddListener(OnSettingsButtonClick);
    }

    private void OnTimerButtonClick()
    {
        Debug.Log("Timer button clicked");
    }

    private void OnFlashlightButtonClick()
    {
        if (isFlashlightOff)
        {
#if PLATFORM_ANDROID
            Flashlight.Enable();
#endif
            Debug.Log("Flashlight enabled");
            isFlashlightOff = false;
        }
        else
        {
#if PLATFORM_ANDROID
            Flashlight.Disable();
#endif
            Debug.Log("Flashlight disabled");
            isFlashlightOff = true;
        }
    }

    private void OnBackgroundChangeButtonClick()
    {
        Debug.Log("Background change button clicked");
    }

    private void OnSettingsButtonClick()
    {
        Debug.Log("Settings button clicked");
    }

}