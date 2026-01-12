namespace PlayModePlus.Editor
{
    using System.Linq;
    using UnityEditor;

    internal sealed class SceneAssetPostprocessor : AssetPostprocessor
    {
        public static event System.Action SceneAssetPathModified;
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (deletedAssets.Any(IsScene) || movedAssets.Any(IsScene))
            {
                SceneAssetPathModified?.Invoke();
            }
        }

        private static bool IsScene(string assetPath)
        {
            return assetPath.EndsWith(".unity", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}