using System;
using System.Collections;
using System.Collections.Generic;
using LYNC;
using LYNC.Wallet;
using UnityEngine;

public class LoginTransactionExample : MonoBehaviour
{
    public string loginUrl;

    public TMPro.TMP_Text debug_Text, tsxHash;
    public UnityEngine.UI.Button login, logout, mint, polygonScan;

    public static LoginTransactionExample Instance { get; private set; }

    private void Start()
    {
        login.interactable = false;
        logout.interactable = false;
        mint.interactable = false;

        debug_Text.text = "Waiting for API key checks!!";
        LyncManager.onLyncReady += (lyncManager) =>
        {
            try
            {
                WalletData walletData = WalletData.TryLoadSavedWallet();
                if (walletData.WalletConnected)
                {
                    debug_Text.text = "EOA Address= " + walletData.PublicAddress +"\nSmart account = " + walletData.SmartAccount+"\nEmail Address= " + walletData.Email +"\nUser Name = " + walletData.UserName;
                    logout.interactable = true;
                    mint.interactable = true;
                }
                else
                {
                    login.interactable = true;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ErrorDisplay.ShowError(e.Message);
            }
        };
    }

    public void Login()
    {
        LyncManager.Instance.WalletAuth.ConnectWallet(loginUrl, (walletData) =>
        {
            debug_Text.text = "EOA Address= " + walletData.PublicAddress +"\nSmart account = " + walletData.SmartAccount+"\nEmail Address= " + walletData.Email +"\nUser Name = " + walletData.UserName;
            login.interactable = false;
            logout.interactable = true;
            mint.interactable = true;
        });
    }

    public void Logout()
    {
        LyncManager.Instance.WalletAuth.Logout();
        debug_Text.text = "";

        login.interactable = true;
        logout.interactable = false;
        mint.interactable = false;
    }

    public void Mint()
    {
        string contractAddress = "0xAFBab1c079dA6076166D1677d6Ce4fFf57718066";
        string functionName = "mintNFT(uint256 _mintNum)";
        List<string> args = new List<string> { "1"};
        void onSuccess(TransactionData tsxData)
        {
            polygonScan.onClick.AddListener(() =>
            {
                Application.OpenURL("https://testnet.bscscan.com/tx/" + tsxData.data.transactionHash);
            });

            polygonScan.gameObject.SetActive(true);
            tsxHash.gameObject.SetActive(true);
            tsxHash.text = "tsxHash = " + tsxData.data.transactionHash;
            mint.interactable = true;
        }

        void onError(string msg)
        {
            // ErrorDisplay.ShowError(msg); 
            tsxHash.gameObject.SetActive(true);
            tsxHash.text = "Error = " + msg;
        }

        LyncManager.Instance.blockchainMiddleware.SendTransaction(contractAddress, functionName, args, onSuccess, onError);
        mint.interactable = false;
        polygonScan.gameObject.SetActive(false);
        tsxHash.gameObject.SetActive(false);
    }
}
