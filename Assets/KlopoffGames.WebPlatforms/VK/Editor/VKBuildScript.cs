using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.VK.Editor
{
    public class VKBuildScript
    {
        [MenuItem("Builder/Build for VK (Staging)")]
        public static void BuildForVKStaging ()
        {
            string buildPath = EditorUtility.SaveFolderPanel("", "", "");
            
            Application.runInBackground = true;
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.WebGL, new []
            {
                "VK_GAMES"
            });
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSpeed);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.WebGL, Il2CppCompilerConfiguration.Release);
            EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", "time");
            PlayerSettings.WebGL.template = "PROJECT:VK";
            
            BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes,
                buildPath,
                BuildTarget.WebGL,
                BuildOptions.None
            );
        }
        
        [MenuItem("Builder/Build for VK (Production)")]
        public static void BuildForVKProduction ()
        {
            string buildPath = EditorUtility.SaveFolderPanel("", "", "");
            
            Application.runInBackground = true;
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.WebGL, new []
            {
                "VK_GAMES", "PRODUCTION"
            });
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSpeed);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.WebGL, Il2CppCompilerConfiguration.Master);
            EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", "speed");
            PlayerSettings.WebGL.template = "PROJECT:VK";
            
            BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes,
                buildPath,
                BuildTarget.WebGL,
                BuildOptions.None
            );
        }
    }
}
