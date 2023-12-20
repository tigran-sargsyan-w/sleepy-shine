using UnityEngine;

public class Test : MonoBehaviour
{
    public void OnTurnOn()
    {
        FlashLightManager.Enable();
    }
    
    public void OnTurnOff()
    {
        FlashLightManager.Disable();
    }
}