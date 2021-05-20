using UnityEditor;

public class AnimationPostprocessor : AssetPostprocessor
{
    private const string AnimationAssetPath = "Assets/Plugins/Wolf3D ReadyPlayerMe SDK/Resources/Animation";

    private void OnPreprocessModel()
    {
        if (assetPath.Contains(AnimationAssetPath))
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.useFileScale = false;
            modelImporter.animationType = ModelImporterAnimationType.Human;
            
            #if UNITY_2019_2_OR_NEWER
            modelImporter.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            #endif
        }
    }
}
