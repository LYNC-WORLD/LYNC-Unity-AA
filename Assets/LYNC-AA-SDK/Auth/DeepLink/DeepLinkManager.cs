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
        private readonly string sharedFilePath = @"C:\ProgramData\launcherdata.txt";

        public static DeepLinkManager Instance { private set; get; }

        private System.Action<WalletData> _onSuccess = null;
        private Coroutine runningCoroutine = null;

        private void Start()
        {
            // Windows player or editor and not Android nor IPhone
            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                RegisterCustomProtocol();
                ClearSharedFile();
                OpenLauncher();
            }
            WalletAuth.walletConnectionRequested += StartProcess;
            Application.deepLinkActivated += onDeepLinkActivated;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        public void StartProcess(string loginUrl, System.Action<WalletData> onSuccess)
        {
            _onSuccess = onSuccess;
            string url = loginUrl + "?scheme=" + DeepLinkRegistration.DeepLinkUrl.Trim() + "&clientId=" + LyncManager.Instance.web3AuthClientID.Trim();

            // Open auth page for standalone and mobile
            Application.OpenURL(url);

            if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
            {
                ClearSharedFile();
                OpenLauncher();
                runningCoroutine = StartCoroutine(ListenForConnectedWallet(onSuccess));
            }
        }

        private void onDeepLinkActivated(string url)
        {
            WalletData walletData = ExtractAndSaveWalletFromUrl(url);
            if (_onSuccess != null)
            {
                WalletAuth.walletConnectionRequested -= StartProcess;
                _onSuccess(walletData);
                _onSuccess = null;
            }
        }

        private WalletData ExtractAndSaveWalletFromUrl(string url)
        {

            UnityEngine.Debug.Log(url);

            string publicAddress = url.Split(new string[] { "wallet=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];
            string encrypted = url.Split(new string[] { "encrypted=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];
            string smartAccount = url.Split(new string[] { "smartAccount=" }, System.StringSplitOptions.None)[1].Split(new string[] { "&" }, System.StringSplitOptions.None)[0];
            
            UnityEngine.Debug.Log("publicAddress: "+publicAddress+" encrypted: "+ encrypted + "smartAccount: "+smartAccount);
            // Save wallet
            WalletData wallet = new WalletData(publicAddress, encrypted.Replace(" ", "+"), smartAccount);
            return wallet;
        }

        #region Windows platform methods
        private IEnumerator ListenForConnectedWallet(System.Action<WalletData> onSuccess)
        {
            string text = File.ReadAllText(sharedFilePath);
            if (text.IndexOf(DeepLinkRegistration.DeepLinkUrl) > -1)
            {
                string url = text.Replace(Process.GetCurrentProcess().Id.ToString(), "").Trim();
                WalletData wallet = ExtractAndSaveWalletFromUrl(url);
                onSuccess?.Invoke(wallet);

                ClearSharedFile();
                StopCoroutine(runningCoroutine);
            }

            yield return new WaitForSeconds(0.1f);

            runningCoroutine = StartCoroutine(ListenForConnectedWallet(onSuccess));
        }

        private void OpenLauncher()
        {
            string processId = Process.GetCurrentProcess().Id.ToString();
            Process p = Process.Start(launcherPath, "processid" + processId);
        }

        private void ClearSharedFile()
        {
            using (StreamWriter writer = File.CreateText(sharedFilePath))
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

            Process regeditProcess = new Process();
            regeditProcess.StartInfo.FileName = "reg.exe";
            regeditProcess.StartInfo.Arguments = "import \"" + tempFilePath + "\"";
            regeditProcess.StartInfo.UseShellExecute = false;
            regeditProcess.Start();
            regeditProcess.WaitForExit();

            File.Delete(tempFilePath);
        }
        #endregion
    }
}