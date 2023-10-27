#if UNITY_STANDALONE_WIN
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System;


public class LYNCWindowsPostBuildProcessor : MonoBehaviour
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        string executablesDir = Application.streamingAssetsPath + "/Executables";
        var launcher = Resources.Load("Launcher") as TextAsset;
        var register = Resources.Load("register") as TextAsset;


        string launcherDestinationPath = executablesDir + "/Launcher.exe";
        string registerDestinationPath = executablesDir + "/register.reg";
        try
        {
            if (!Directory.Exists(executablesDir))
                Directory.CreateDirectory(executablesDir);

            File.WriteAllBytes(launcherDestinationPath, launcher.bytes);
            File.WriteAllBytes(registerDestinationPath, register.bytes);
            Debug.Log("Files moved successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }
    }
}
#endif
