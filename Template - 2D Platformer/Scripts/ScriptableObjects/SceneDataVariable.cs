using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjectArchitecture
{
    [System.Serializable]
    public class SceneDataEvent : UnityEvent<SceneData> { }

    [CreateAssetMenu(
        fileName = "SceneDataVariable.asset",
        menuName = SOArchitecture_Utility.VARIABLE_SUBMENU + "SceneData",
        order = SOArchitecture_Utility.ASSET_MENU_ORDER_COLLECTIONS + 2)]
    public sealed class SceneDataVariable : BaseVariable<SceneData, SceneDataEvent>
    {
    } 
}