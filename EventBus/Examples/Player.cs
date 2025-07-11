using UnityEngine;
using EventBus; 

public class Player : MonoBehaviour
{
    [SerializeField] AudioClip _dieSound;

    private void Start()
    {
        Bus<EV_Player_Spawned>.Raise();
    }

    void Died()
    {
        Bus<EV_Player_Died>.Raise();
    }

    void TakeDamage()
    {
        Bus<EV_Player_Damaged>.Raise(new EV_Player_Damaged { value = 5 });
        Bus<EV_SFX_Play>.Raise(new EV_SFX_Play { value = _dieSound });
    }   
}