using System.IO;
using System.Text;
using System.Xml;
using LYNC.Wallet;
using UnityEditor.Android;
using UnityEngine;

public class LYNCPostBuildProcessor : IPostGenerateGradleAndroidProject
{
    string xmlPath;

    public void OnPostGenerateGradleAndroidProject(string basePath)
    {
        xmlPath = GetManifestPath(basePath);
        if (File.Exists(xmlPath))
        {
            string xmlContent = File.ReadAllText(xmlPath);

            xmlContent = xmlContent.Replace("LYNC_DEEPLINK_SCHEME", DeepLinkRegistration.DeepLinkUrl);
            xmlContent = xmlContent.Replace("LYNC_DEEPLINK_HOST", "");
            Debug.Log(xmlContent);

            using StreamWriter writer = new(xmlPath);
            writer.WriteLine(xmlContent);
        }
    }

    public int callbackOrder { get { return 2; } }

    private string GetManifestPath(string basePath)
    {
        var pathBuilder = new StringBuilder(basePath);
        pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
        pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
        pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
        return pathBuilder.ToString();
    }
}

