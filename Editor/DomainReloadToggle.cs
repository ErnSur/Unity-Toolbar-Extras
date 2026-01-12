namespace PlayModePlus.Editor
{
    using UnityEditor;
    using UnityEditor.Toolbars;
    using UnityEngine;

    public class DomainReloadToggle
    {
        [MainToolbarElement("Editor Utility/Domain Reload", defaultDockPosition = MainToolbarDockPosition.Right)]
        public static MainToolbarElement ToggleDomainReload()
        {
            var enabled = (EditorSettings.enterPlayModeOptions & (EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload)) == 0;
            var enabledIcon = EditorGUIUtility.IconContent("icons/d_preaudioautoplayoff@2x.png").image as Texture2D;
            var disabledIcon = EditorGUIUtility.IconContent("icons/animationvisibilitytoggleoff.png").image as Texture2D;
            var icon = enabled ? enabledIcon : disabledIcon;

            var content = new MainToolbarContent($"Reload {(enabled ? "Enabled" : "Disabled")}", icon, $"Toggle domain and scene reload on/off for faster Play Mode entry.\nCurrently {(enabled ? "Enabled" : "Disabled")}");
            return new MainToolbarToggle(content, enabled, ToggleDomainReload);
        }

        private static void ToggleDomainReload(bool e)
        {
            var options = EditorSettings.enterPlayModeOptions;
            if (e)
            {
                Debug.Log("Enabling Domain Reload and Scene Reload.");
                options = EnterPlayModeOptions.None;
            }
            else
            {
                Debug.Log("Disabling Domain Reload and Scene Reload for faster Play Mode entry.");
                options |= EnterPlayModeOptions.DisableDomainReload | EnterPlayModeOptions.DisableSceneReload;
            }

            EditorSettings.enterPlayModeOptions = options;
            MainToolbar.Refresh("EditorSettings/Toggle Domain Reload");
        }
    }
}