using UnityEngine;

namespace EventBus
{ 
    // Player Events
    public struct EV_PlayerSpawned : IEvent { }
    public struct EV_PlayerDied : IEvent { }
    public struct EV_PlayerDamaged : IEvent { public int value; }

    // Enemy Events
    public struct EV_EnemySpawned : IEvent { public EnemySO value; }
    public struct EV_EnemyDied : IEvent { public EnemySO value; }

    // GameManager Events 
    public struct EV_GameReachedFinishLine : IEvent { }
    public struct EV_GameStart : IEvent { }
    public struct EV_GameRestart : IEvent { }

    // MusicManager Events
    public struct EV_MusicChange : IEvent { public AudioClip value; }
    public struct EV_MusicTogglePause : IEvent { public bool value; }
    public struct EV_MusicTogglePlay : IEvent { public bool value; }

    // Sound Effects Manager
    public struct EV_SfxPlay : IEvent { public AudioClip value; }

    // Item Events
    public struct EV_ItemObtained : IEvent { public ItemSO value; }
    public struct EV_ItemUsed : IEvent { public ItemSO value; }

}
