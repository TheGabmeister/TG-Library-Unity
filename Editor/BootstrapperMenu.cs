#if UNITY_EDITOR
using UnityEditor;

// To be used together with Bootstrapper.cs. Adds a button in the toolbar
// to enable bootstrapper.

public class BootstrapperMenu
{
    private const string MenuPath = "Bootstrapper/Enabled";
    
    [MenuItem(MenuPath, false)]
    private static void ToggleBootstrapper()
    {
        Bootstrapper.Enabled = !Bootstrapper.Enabled;
        Menu.SetChecked(MenuPath, Bootstrapper.Enabled);
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleBootstrapperValidate()
    {
        Menu.SetChecked(MenuPath, Bootstrapper.Enabled);
        return true;
    }
}
#endif