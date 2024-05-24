using LYNC;
using LYNC.Wallet;
using UnityEngine;
using UnityEngine.UI;

public class ExampleLogin : MonoBehaviour
{
    public TMPro.TMP_Text addressTxt, loginDateTxt;
    public Button login, logout;
    [Space]
    public string loginUrl;
    public TokenExample[] tokens = new TokenExample[3];

    void Start()
    {
        Application.targetFrameRate = 30;

        LyncManager.onLyncReady += LyncReady;

        foreach (var token in tokens)
        {
            token?.SetInteractable(false);
        }
    }

    private void LyncReady(LyncManager Lync)
    {
        WalletData walletData = WalletData.TryLoadSavedWallet();
        if (walletData.WalletConnected)
        {
            try
            {
                login.interactable = false;
                logout.interactable = true;
                addressTxt.text = "EOA Address: " + walletData.PublicAddress+  "\nSmartAccount: " + walletData.SmartAccount;
                loginDateTxt.text = "Login Date: " + walletData.loginDate.ToString();

                foreach (var token in tokens)
                {
                    token?.SetInteractable(true);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        login.onClick.AddListener(() =>
        {
            Lync.WalletAuth.ConnectWallet(loginUrl, (wallet) =>
            {
                login.interactable = false;
                logout.interactable = true;
                addressTxt.text = "EOA Address: " + wallet.PublicAddress + "\nSmartAccount: " + wallet.SmartAccount;
                loginDateTxt.text = "Login Date: " + wallet.loginDate.ToString();

                foreach (var token in tokens)
                {
                    token?.SetInteractable(true);
                }
            });
        });

        logout.onClick.AddListener(() =>
        {
            try
            {
                addressTxt.text = "";
                loginDateTxt.text = "";

                login.interactable = true;
                logout.interactable = false;

                Lync.WalletAuth.Logout();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        });
    }
}
