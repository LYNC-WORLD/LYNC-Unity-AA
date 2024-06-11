using System.Collections;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace LYNC.Wallet
{
    public class DeepLinkManager : MonoBehaviour
    {
        // Windows configuration
        private readonly string launcherPath = (Application.streamingAssetsPath + "/Executables/Launcher.exe").Replace("/", "\\");
        private readonly string registerPath = (Application.streamingAssetsPath + "/Executables/register.reg").Replace("/", "\\");
        private readonly string sharedFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData);

        public static DeepLinkManager Instance { private set; get; }

        private System.Action<WalletData> _onSuccess = null;

        private void Start()
        {
            // Windows player or editor and not Android nor IPhone
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                RegisterCustomProtocol();
                OpenLauncher();
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            WalletAuth.walletConnectionRequested += StartProcess;
            Application.deepLinkActivated += onDeepLinkActivated;
        }

        private void OnDisable()
        {
            // WalletAuth.walletConnectionRequested -= StartProcess;
            Application.deepLinkActivated -= onDeepLinkActivated;
        }

        public void StartProcess(string loginUrl, System.Action<WalletData> onSuccess)
        {
            _onSuccess = onSuccess;
            string url = loginUrl + "?scheme=" + DeepLinkRegistration.DeepLinkUrl.Trim() + "&clientId=" + LyncManager.Instance.web3AuthClientID.Trim() + "&chainId=" + (int)LyncManager.Instance.chainID + "&network=" + LyncManager.Instance.network;
            // Open auth page for standalone and mobile
            Application.OpenURL(url);

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                OpenLauncher();
                StartCoroutine(ListenForConnectedWallet(onSuccess));
            }
        }

        private void onDeepLinkActivated(string url)
        {
            WalletData walletData = ExtractAndSaveWalletFromUrl(url);
            if (_onSuccess != null)
            {
                _onSuccess(walletData);
                _onSuccess = null;
            }
        }

        private WalletData ExtractAndSaveWalletFromUrl(string url)
        {

            // UnityEngine.Debug.Log(url);

            string publicAddress = url.Split(new string[] { "wallet=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];
            string encrypted = url.Split(new string[] { "encrypted=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];
            string smartAccount = url.Split(new string[] { "smartAccount=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];
            string email = url.Split(new string[] { "email=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];
            string userName = url.Split(new string[] { "userName=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];
            string idToken = url.Split(new string[] { "idToken=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];

            // UnityEngine.Debug.LogError("publicAddress: "+publicAddress+" encrypted: "+ encrypted + "smartAccount: "+smartAccount+ "email: "+email+ "userName: "+name);
            // Save wallet
            WalletData wallet = new WalletData(publicAddress, encrypted.Replace(" ", "+"), smartAccount, email, userName.Replace("%20", " "), idToken);
            return wallet;
        }

        #region Windows platform methods
        private IEnumerator ListenForConnectedWallet(System.Action<WalletData> onSuccess)
        {
            string text = File.ReadAllText(Path.Combine(sharedFilePath, ProcessType.UNITY_REDIRECT.ToString()));
            if (text.ToLower().IndexOf(DeepLinkRegistration.DeepLinkUrl.ToLower()) > -1)
            {
                string url = text.Replace(Process.GetCurrentProcess().Id.ToString(), "").Trim();
                WalletData wallet = ExtractAndSaveWalletFromUrl(url);
                onSuccess?.Invoke(wallet);

                StopAllCoroutines();
            }

            yield return new WaitForSeconds(0.1f);

            StartCoroutine(ListenForConnectedWallet(onSuccess));
        }

        private void OpenLauncher()
        {
            string processId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
            StartProcess(ProcessType.UNITY_REDIRECT, processId);
        }

        private void ClearSharedFile(ProcessType processType)
        {
            using (StreamWriter writer = File.CreateText(Path.Combine(sharedFilePath, processType.ToString())))
            {
                writer.Write("");
            }
        }

        private void RegisterCustomProtocol()
        {
            string tempFilePath = registerPath.Replace("register.reg", "temp.reg");
            string temp = File.ReadAllText(registerPath);
            temp = temp.Replace("%APP_NAME%", DeepLinkRegistration.DeepLinkUrl);
            temp = temp.Replace("%LAUNCHER_PATH%", launcherPath.Replace(@"\", @"\\"));

            using (StreamWriter writer = new StreamWriter(tempFilePath))
            {
                writer.Write(temp);
            }

            string args = "C:\\Windows\\System32\\reg.exe,import \"" + tempFilePath + "\"";
            StartProcess(ProcessType.UNITY_SAVEDEEPLINK, args);
        }

        private void StartProcess(ProcessType processType, string args)
        {
            ClearSharedFile(processType);
            string temp = "";
            if (processType == ProcessType.UNITY_REDIRECT)
            {
                temp = processType.ToString() + args;
            }
            else if (processType == ProcessType.UNITY_SAVEDEEPLINK)
            {
                temp = args;
            }

            using (StreamWriter writer = new StreamWriter(Path.Combine(sharedFilePath, processType.ToString())))
            {
                writer.Write(temp);
            }

            Application.OpenURL(launcherPath);
        }
        #endregion
    }

    public enum ProcessType
    {
        UNITY_REDIRECT, UNITY_SAVEDEEPLINK
    }
}