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
            ModifyXmlFile(xmlPath);
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

    private void ModifyXmlFile(string xmlPath)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(xmlPath);
        XmlNode applicationNode = doc.SelectSingleNode("/manifest/application");
        if (applicationNode != null)
        {

            // Find <activity> element inside <application>
            XmlNode activityNode = applicationNode.SelectSingleNode("activity");
            if (activityNode != null)
            {
                bool intentFilterExists = false;
                XmlNodeList intentFilterNodes = activityNode.SelectNodes("intent-filter");
                foreach (XmlNode intentFilterNode in intentFilterNodes)
                {
                    XmlNode dataNode = intentFilterNode.SelectSingleNode("data");
                    if (dataNode != null &&
                        dataNode.Attributes["android:scheme"].Value == DeepLinkRegistration.DeepLinkUrl &&
                        dataNode.Attributes["android:host"].Value == "")
                    {
                        intentFilterExists = true;
                        break;
                    }
                }

                Debug.Log("intentFilterExists: " + intentFilterExists);

                if (!intentFilterExists){
                    XmlElement intentFilterElement = doc.CreateElement("intent-filter");

                    XmlAttribute actionAttribute = doc.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
                    actionAttribute.Value = "android.intent.action.VIEW";
                    XmlElement actionElement = doc.CreateElement("action");
                    actionElement.Attributes.Append(actionAttribute);

                    XmlAttribute categoryDefaultAttribute = doc.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
                    categoryDefaultAttribute.Value = "android.intent.category.DEFAULT";
                    XmlElement categoryDefaultElement = doc.CreateElement("category");
                    categoryDefaultElement.Attributes.Append(categoryDefaultAttribute);

                    XmlAttribute categoryBrowsableAttribute = doc.CreateAttribute("android", "name", "http://schemas.android.com/apk/res/android");
                    categoryBrowsableAttribute.Value = "android.intent.category.BROWSABLE";
                    XmlElement categoryBrowsableElement = doc.CreateElement("category");
                    categoryBrowsableElement.Attributes.Append(categoryBrowsableAttribute);

                    XmlAttribute dataSchemeAttribute = doc.CreateAttribute("android", "scheme", "http://schemas.android.com/apk/res/android");
                    dataSchemeAttribute.Value = DeepLinkRegistration.DeepLinkUrl; //"LYNC_DEEPLINK_SCHEME";
                    XmlAttribute dataHostAttribute = doc.CreateAttribute("android", "host", "http://schemas.android.com/apk/res/android");
                    dataHostAttribute.Value = "";
                    XmlElement dataElement = doc.CreateElement("data");
                    dataElement.Attributes.Append(dataSchemeAttribute);
                    dataElement.Attributes.Append(dataHostAttribute);

                    // Append child elements to <intent-filter>
                    intentFilterElement.AppendChild(actionElement);
                    intentFilterElement.AppendChild(categoryDefaultElement);
                    intentFilterElement.AppendChild(categoryBrowsableElement);
                    intentFilterElement.AppendChild(dataElement);

                    // Append <intent-filter> to <activity>
                    activityNode.AppendChild(intentFilterElement);
                }
                else
                {
                    Debug.Log("The specified <intent-filter> already exists.");
                }
            }
            else
            {
                Debug.LogError("No <activity> element found inside <application>.");
            }
        }
        else
        {
            Debug.LogError("No <application> element found in the XML.");
        }
        doc.Save(xmlPath);
        Debug.Log("XML file modified successfully.");
        Debug.Log(File.ReadAllText(xmlPath));
    }
}

