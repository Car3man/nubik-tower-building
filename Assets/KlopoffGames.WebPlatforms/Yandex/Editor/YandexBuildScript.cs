using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.Yandex.Editor
{
    public class YandexBuildScript
    {
        [MenuItem("Builder/Build for Yandex (Staging)")]
        public static void BuildForYandexStaging ()
        {
            string buildPath = EditorUtility.SaveFolderPanel("", "", "");
            
            Application.runInBackground = true;
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.WebGL, new []
            {
                "YANDEX_GAMES"
            });
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSize);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.WebGL, Il2CppCompilerConfiguration.Release);
            EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", "time");
            PlayerSettings.WebGL.template = "PROJECT:Yandex";
            
            BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes,
                buildPath,
                BuildTarget.WebGL,
                BuildOptions.None
            );
        }
        
        [MenuItem("Builder/Build for Yandex (Production)")]
        public static void BuildForYandexProduction ()
        {
            string buildPath = EditorUtility.SaveFolderPanel("", "", "");
            
            Application.runInBackground = true;
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.WebGL, new []
            {
                "YANDEX_GAMES", "PRODUCTION"
            });
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSize);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.WebGL, Il2CppCompilerConfiguration.Master);
            EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", "size");
            PlayerSettings.WebGL.template = "PROJECT:Yandex";
            
            BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes,
                buildPath,
                BuildTarget.WebGL,
                BuildOptions.None
            );
        }
    }
}
