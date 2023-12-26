using UnityEngine;

public class Test : MonoBehaviour
{
    public void OnTurnOn()
    {
        Flashlight.Enable();
    }
    
    public void OnTurnOff()
    {
        Flashlight.Disable();
    }
}