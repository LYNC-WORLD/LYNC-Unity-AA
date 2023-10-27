using UnityEngine;

namespace LYNC.Wallet
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif
    public class DeepLinkRegistration : MonoBehaviour
    {
        public string deepLinkUrl = "lync";
        public static string DeepLinkUrl { private set; get; }

        private void Start()
        {
            DeepLinkRegistration.DeepLinkUrl = deepLinkUrl;

            if (!string.IsNullOrEmpty(deepLinkUrl))
            {
#if UNITY_EDITOR
                UnityEditor.PlayerSettings.macOS.urlSchemes = new string[] { deepLinkUrl };
#endif
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (string.IsNullOrEmpty(deepLinkUrl))
                Debug.LogError("DeepLink URL is empty or null, set its value in LYNC prefab.");
        }

        private void OnValidate()
        {
            if (!string.IsNullOrEmpty(deepLinkUrl))
            {
                UnityEditor.PlayerSettings.macOS.urlSchemes = new string[] { deepLinkUrl };
                UnityEditor.PlayerSettings.iOS.iOSUrlSchemes = new string[] { deepLinkUrl };
            }
            else
                Debug.LogError("DeepLink URL is empty or null, set its value in LYNC prefab.");

            DeepLinkRegistration.DeepLinkUrl = deepLinkUrl;
        }
#endif
    }
}