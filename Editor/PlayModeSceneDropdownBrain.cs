namespace PlayModePlus.Editor
{
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEditor.Toolbars;

    public class PlayModeSceneDropdownBrain
    {
        private const string PrefsKey = "toolbar-play-selected-scene";
        private readonly string _mainToolbarPath;

        private SceneAsset _selectedScene;

        private SceneAsset SelectedScene
        {
            get
            {
                if (_selectedScene != null) return _selectedScene;
                var lastScenePath = PlayerPrefs.GetString(PrefsKey, defaultValue: null);
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
                PlayerPrefs.SetString(PrefsKey, lastScenePath);
            }
        }

        public string DisplayText => SelectedScene != null ? SelectedScene.name : "Active Scene";

        public PlayModeSceneDropdownBrain(string mainToolbarPath)
        {
            _mainToolbarPath = mainToolbarPath;
            EditorSceneManager.playModeStartScene = SelectedScene;
        }

        public void ShowSceneDropdownMenu(Rect dropDownRect)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Active Scene"), SelectedScene == null, () =>
            {
                SelectedScene = null;
                MainToolbar.Refresh(_mainToolbarPath);
            });
            menu.AddSeparator("");

            AddSceneMenuItems(menu);

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Create Scene..."), false, () =>
            {
                CreateScene();
                MainToolbar.Refresh(_mainToolbarPath);
            });

            menu.DropDown(dropDownRect);
        }


        private static void CreateScene()
        {
            const string baseName = "NewScene";
            var sceneNumber = 1;
            var newSceneName = baseName;

            while (AssetDatabase.LoadAssetAtPath<SceneAsset>($"Assets/{newSceneName}.unity") != null)
            {
                newSceneName = $"{baseName}{sceneNumber}";
                sceneNumber++;
            }

            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var newScenePath = $"Assets/{newSceneName}.unity";


            EditorSceneManager.SaveScene(newScene, newScenePath);
            AssetDatabase.Refresh();
        }

        private const string ScenesFolderPath = "Assets/Scenes/";

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
                MainToolbar.Refresh(_mainToolbarPath);
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