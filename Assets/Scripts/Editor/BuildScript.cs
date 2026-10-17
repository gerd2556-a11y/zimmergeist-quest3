using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace OfflinePet.Editor
{
    public static class BuildScript
    {
        [MenuItem("Zimmergeist/Build Quest 3 APK")]
        public static void BuildQuest3Apk()
        {
            Debug.Log("[BuildScript] Starting Quest 3 Android APK Build...");

            // 1. Ensure Target Platform is Android
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);

            // 2. Verified Meta Quest 3 Player Settings
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.offlinepet.zimmergeist");
            PlayerSettings.productName = "Zimmergeist";
            PlayerSettings.companyName = "OfflineStudio";
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.Android.bundleVersionCode = 1;

            // Target Quest Android API Levels (Android 12L / 14 compatible)
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34;

            // Meta Quest 3 requires 64-bit ARM (IL2CPP + ARM64)
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            // Linear Color Space is mandatory for OpenXR & Passthrough on Quest
            PlayerSettings.colorSpace = ColorSpace.Linear;

            string buildOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Quest3");
            if (!Directory.Exists(buildOutputDir))
            {
                Directory.CreateDirectory(buildOutputDir);
            }

            string apkPath = Path.Combine(buildOutputDir, "Zimmergeist.apk");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/MainMRScene.unity" },
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[BuildScript] APK Build SUCCEEDED: {summary.totalSize} bytes at {apkPath}");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError($"[BuildScript] APK Build FAILED with {summary.totalErrors} errors.");
            }
        }
    }
}
