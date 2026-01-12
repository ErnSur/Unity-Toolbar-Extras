namespace QuickEye.ToolbarExtras
{
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.Toolbars;
    using UnityEngine;

    public class PlayModeSceneDropdown
    {
        private const string MainToolbarElementPath = "Play Mode/Play-Mode Scene";
        private const string SelectedScenePrefsKey = MainToolbarElementPath + "_SelectedScene";
        private const string ScenesFolderPath = "Assets/Scenes/";

        private static readonly PlayModeSceneDropdown Instance = new();


        [MainToolbarElement(MainToolbarElementPath, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 3)]
        public static MainToolbarElement CreateToolbarDropdown()
        {
            var icon = EditorGUIUtility.IconContent("UnityLogo").image as Texture2D;
            var content = new MainToolbarContent(Instance.DisplayText, icon, "Select scene to play");
            var dropdown = new MainToolbarDropdown(content, Instance.ShowSceneDropdownMenu);
            return dropdown;
        }

        private SceneAsset _selectedScene;


        private SceneAsset SelectedScene
        {
            get
            {
                if (_selectedScene != null) return _selectedScene;
                var lastScenePath = PlayerPrefs.GetString(SelectedScenePrefsKey, defaultValue: null);
                if (lastScenePath != null)
                {
                    _selectedScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(lastScenePath);
                }

                return _selectedScene;
            }
            set
            {
                _selectedScene = value;
                EditorSceneManager.playModeStartScene = value;
                var lastScenePath = value == null ? null : AssetDatabase.GetAssetPath(_selectedScene);
                PlayerPrefs.SetString(SelectedScenePrefsKey, lastScenePath);
            }
        }

        public string DisplayText => SelectedScene != null ? SelectedScene.name : "Active Scene";


        public PlayModeSceneDropdown()
        {
            EditorSceneManager.playModeStartScene = SelectedScene;
            SceneAssetPostprocessor.SceneAssetPathModified += () => MainToolbar.Refresh(MainToolbarElementPath);
        }

        public void ShowSceneDropdownMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Active Scene"), SelectedScene == null, () =>
            {
                SelectedScene = null;
                MainToolbar.Refresh(MainToolbarElementPath);
            });
            menu.AddSeparator("");

            AddSceneMenuItems(menu);

            menu.DropDown(dropDownRect);
        }

        private void AddSceneMenuItems(GenericMenu menu)
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

        private void AddSceneMenuItem(GenericMenu menu, string scenePath)
        {
            var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            var itemName = ScenePathToMenuPath(scenePath);
            var isSelected = scene == SelectedScene;
            menu.AddItem(new GUIContent(itemName), isSelected, () =>
            {
                SelectedScene = scene;
                MainToolbar.Refresh(MainToolbarElementPath);
            });
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