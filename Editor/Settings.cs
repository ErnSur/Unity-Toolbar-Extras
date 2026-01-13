namespace QuickEye.ToolbarExtras
{
    using UnityEditor;
    using UnityEditor.Toolbars;
    using UnityEngine;

    public class Settings
    {
        [MainToolbarElement("Editor Utility/Project Settings", defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement ProjectSettingsButton()
        {
            var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
            var content = new MainToolbarContent("Project", icon, "Project Settings");
            return new MainToolbarButton(content, () => { SettingsService.OpenProjectSettings(); });
        }

        [MainToolbarElement("Editor Utility/Preferences", defaultDockPosition = MainToolbarDockPosition.Left)]
        public static MainToolbarElement PreferencesButton()
        {
            var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
            var content = new MainToolbarContent("User", icon, "User Preferences");
            return new MainToolbarButton(content, () => { SettingsService.OpenUserPreferences(); });
        }

        private const string AssemblyReloadLockPath = "Editor Utility/Assembly Reload Lock";

        [MainToolbarElement(AssemblyReloadLockPath, defaultDockPosition = MainToolbarDockPosition.Right)]
        public static MainToolbarElement AssemblyReloadToggle()
        {
            var enabled = AssemblyReloadLock.IsLocked;
            var iconEnabled = EditorGUIUtility.IconContent("Locked").image as Texture2D;
            var iconDisabled = EditorGUIUtility.IconContent("Unlocked").image as Texture2D;
            var icon = enabled ? iconEnabled : iconDisabled;
            var tooltip = enabled ? "Assembly Reload Disabled" : "Assembly Reload Enabled";
            var content = new MainToolbarContent("Assembly Reload", icon, tooltip);

            return new MainToolbarToggle(content, enabled, v =>
            {
                AssemblyReloadLock.IsLocked = v;
                MainToolbar.Refresh(AssemblyReloadLockPath);
            });
        }
    }
}