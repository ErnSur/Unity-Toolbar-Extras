namespace QuickEye.Editor
{
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEditor.Toolbars;
    using UnityEngine.SceneManagement;

    public class OpenSceneDropdown
    {
        private const string SceneDropdownPath = "Editor Utility/Open Scene";

        private const string ScenesFolderPath = "Assets/Scenes/";

        [MainToolbarElement(SceneDropdownPath, defaultDockPosition = MainToolbarDockPosition.Right,
            defaultDockIndex = 3)]
        public static MainToolbarElement CreateToolbarDropdown()
        {
            var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
            var content = new MainToolbarContent("Open Scene", icon, "Select scene to open");
            var dropdown = new MainToolbarDropdown(content, ShowSceneDropdownMenu);
            return dropdown;
        }
        
        private static void ShowSceneDropdownMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Create Scene..."), false, CreateScene);
            menu.AddSeparator("");
            AddSceneMenuItems(menu);

            menu.DropDown(dropDownRect);
        }

        private static void CreateScene() => EditorApplication.ExecuteMenuItem("File/New Scene");

        private static void AddSceneMenuItems(GenericMenu menu)
        {
            var scenePaths = AssetDatabase.FindAssets("t:SceneAsset")
                .Select(AssetDatabase.GUIDToAssetPath).ToArray();

            // scenes in the "Scenes" folder should always appear at the top
            foreach (var scenePath in scenePaths.Where(p => p.StartsWith(ScenesFolderPath)))
                AddSceneMenuItem(menu, scenePath);

            menu.AddSeparator("");
            foreach (var scenePath in scenePaths.Where(p => !p.StartsWith(ScenesFolderPath)))
                AddSceneMenuItem(menu, scenePath);
        }

        private static void AddSceneMenuItem(GenericMenu menu, string scenePath)
        {
            var itemName = ScenePathToMenuPath(scenePath);
            var isSelected = IsSceneLoaded(scenePath);
            menu.AddItem(new GUIContent(itemName), isSelected, () => { EditorSceneManager.OpenScene(scenePath); });
        }

        private static bool IsSceneLoaded(string scenePath)
        {
            return SceneManager.GetSceneByPath(scenePath).isLoaded;
        }

        private static string ScenePathToMenuPath(string scenePath)
        {
            var withoutExtension = scenePath[..^6];
            return RemovePrefix(withoutExtension, ScenesFolderPath, "Assets/", "Packages/");

            static string RemovePrefix(string input, params string[] prefixes)
            {
                if (string.IsNullOrEmpty(input) || prefixes == null || prefixes.Length == 0)
                    return input;

                foreach (var prefix in prefixes)
                {
                    if (!string.IsNullOrEmpty(prefix) && input.StartsWith(prefix))
                        return input[prefix.Length..];
                }

                return input;
            }
        }
    }
}