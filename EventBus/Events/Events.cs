using UnityEngine;

// Define your events here. For larger projects, create separate files that group the events according 
// to the system they manage: PlayerEvents.cs, GameStateEvents.cs, UIEvents.cs...

namespace EventBus
{
    public struct EV_PlayerPossess : IEvent { public bool value; }
    public struct EV_PlayerDied : IEvent { }


    public struct EV_GameStart : IEvent { }
    public struct EV_GameRestart : IEvent { }
    public struct EV_CoinCollected : IEvent { }

    public struct EV_UiStatsUpdate : IEvent { public int score, distance, coins; }
    public struct EV_UiStateChange : IEvent { public UiState state; }

    public struct EV_MusicToggle : IEvent { public bool value; }

    public struct EV_SfxPlay : IEvent { public AudioClip clip; }


}
