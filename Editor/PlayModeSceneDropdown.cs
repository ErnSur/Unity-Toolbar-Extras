namespace PlayModePlus.Editor
{
    using UnityEditor;
    using UnityEditor.Toolbars;
    using UnityEngine;

    internal static class PlayModeSceneDropdown
    {
        private const string SceneDropdownPath = "Play Mode/Play-Mode Scene Selector";

        private static readonly PlayModeSceneDropdownBrain PlayModeSceneDropdownBrain;

        static PlayModeSceneDropdown()
        {
            PlayModeSceneDropdownBrain = new PlayModeSceneDropdownBrain(SceneDropdownPath);
            SceneAssetPostprocessor.SceneAssetPathModified += () => MainToolbar.Refresh(SceneDropdownPath);
        }

        //TODO: create another one that lets you open the scene instead  
        [MainToolbarElement(SceneDropdownPath, defaultDockPosition = MainToolbarDockPosition.Middle,
            defaultDockIndex = 3)]
        public static MainToolbarElement CreateSceneDropdown()
        {
            var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
            var content = new MainToolbarContent(PlayModeSceneDropdownBrain.DisplayText, icon, "Select scene to play");
            var dropdown = new MainToolbarDropdown(content, PlayModeSceneDropdownBrain.ShowSceneDropdownMenu);
            return dropdown;
        }
    }
}