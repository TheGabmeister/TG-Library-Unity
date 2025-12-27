using UnityEditor;

[InitializeOnLoad]
public static class DisableDomainReload
{
    const string MenuPath = "Tools/Disable Domain Reload";

    static bool m_enabled;

    static DisableDomainReload()
    {
        // Initialize from current Editor settings (so the menu state matches on load).
        m_enabled = EditorSettings.enterPlayModeOptionsEnabled
            && (EditorSettings.enterPlayModeOptions & EnterPlayModeOptions.DisableDomainReload) != 0;

        Apply(m_enabled);
    }

    [MenuItem(MenuPath, priority = 2000)]
    static void ToggleMenu()
    {
        m_enabled = !m_enabled;
        Apply(m_enabled);
    }

    // Validation method: runs to draw/enable the menu item. Returning true keeps it enabled.
    [MenuItem(MenuPath, validate = true)]
    static bool ToggleMenuValidate()
    {
        Menu.SetChecked(MenuPath, m_enabled);
        return true;
    }

    static void Apply(bool enabled)
    {
        EditorSettings.enterPlayModeOptionsEnabled = enabled;

        if (enabled)
        {
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
        }
    }
}