using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;

/* 
 * This script adds a button to the right of the Play Mode button in the toolbar. 
 * When pressed, the button disables domain reload.
 * Depends on https://github.com/marijnz/unity-toolbar-extender
 */

[InitializeOnLoad]
public static class DisableDomainReload
{
    static bool m_enabled;

    static bool Enabled
    {
        get { return m_enabled; }
        set
        {
            m_enabled = value;
        }
    }

    static DisableDomainReload()
    {
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
    }

    static void OnToolbarGUI()
    {
        var tex = EditorGUIUtility.IconContent(@"UnityEditor.SceneView").image;

        GUI.changed = false;
        GUILayout.Toggle(m_enabled, new GUIContent(null, tex, "Disable Domain Reload"), "Command");
        if (GUI.changed)
        {
            Enabled = !Enabled;

            if (m_enabled)
            {
                EditorSettings.enterPlayModeOptionsEnabled = m_enabled;
                EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.DisableDomainReload;
            }
            else
            {
                EditorSettings.enterPlayModeOptionsEnabled = m_enabled;
            }
        }
    }
}