using UnityEngine;
using EventBus;

public class GameManager : MonoBehaviour
{
    private void OnEnable()
    {
        Bus<EV_Player_Died>.Add(RestartGame);
        Bus<EV_Player_Damaged>.Add(FlashScreenToRed);
    }
    private void OnDisable()
    {
        Bus<EV_Player_Died>.Remove(RestartGame);
        Bus<EV_Player_Damaged>.Remove(FlashScreenToRed);
    }

    void RestartGame() 
    {
        // Restart logic here...
    }

    void FlashScreenToRed(EV_Player_Damaged message)
    {
        Debug.Log(message.value);
        // Flash screen to red based on the amount of damage taken
    }
}
