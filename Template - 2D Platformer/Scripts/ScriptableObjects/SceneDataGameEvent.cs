using UnityEngine;

namespace ScriptableObjectArchitecture
{
    [System.Serializable]
    [CreateAssetMenu(
        fileName = "SceneGameEvent.asset",
        menuName = SOArchitecture_Utility.ADVANCED_GAME_EVENT + "SceneData",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_EVENTS + 5)]
    public sealed class SceneDataGameEvent : GameEventBase<SceneData>
    {

    }
}
