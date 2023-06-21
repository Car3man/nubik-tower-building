using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace KlopoffGames.WebPlatforms.LocalWebGL.Editor
{
    public class LocalWebGLBuildScript
    {
        [MenuItem("Builder/Build for LocalWebGL")]
        public static void BuildForLocalWebGL ()
        {
            string buildPath = EditorUtility.SaveFolderPanel("", "", "");
            
            Application.runInBackground = true;
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.WebGL, new []
            {
                "LOCAL_WEBGL"
            });
            PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, Il2CppCodeGeneration.OptimizeSpeed);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.WebGL, Il2CppCompilerConfiguration.Release);
            EditorUserBuildSettings.SetPlatformSettings(BuildPipeline.GetBuildTargetName(BuildTarget.WebGL), "CodeOptimization", "time");
            PlayerSettings.WebGL.template = "PROJECT:LocalWebGL";
            
            BuildPipeline.BuildPlayer(
                EditorBuildSettings.scenes,
                buildPath,
                BuildTarget.WebGL,
                BuildOptions.None
            );
        }
    }
}
