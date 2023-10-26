using System;
using System.Collections;
using System.Collections.Generic;
using LYNC;
using LYNC.Wallet;
using UnityEngine;

public class LoginTransactionExample : MonoBehaviour
{
    public string loginUrl;

    public TMPro.TMP_Text tMP_Text, tsxHash;
    public UnityEngine.UI.Button login, logout, mint, polyscan;

    public static LoginTransactionExample Instance { get; private set; }

    private void Start()
    {
        login.interactable = false;
        logout.interactable = false;
        mint.interactable = false;

        tMP_Text.text = "waiting api key check...";
        LyncManager.onLyncReady += (lyncManager) =>
        {
            Debug.Log("event working");
            tMP_Text.text = "Valid API Key!";
            try
            {
                WalletData walletData = WalletData.TryLoadSavedWallet();
                tMP_Text.text = "Valid API Key! - Wallet connected = " + walletData.WalletConnected;
                if (walletData.WalletConnected)
                {
                    tMP_Text.text = "EOA Address= " + walletData.PublicAddress +"\nSmart account = " + walletData.SmartAccount;
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
            tMP_Text.text = "EOA Address= " + walletData.PublicAddress + "\nSmart account = " + walletData.SmartAccount;
            login.interactable = false;
            logout.interactable = true;
            mint.interactable = true;
        });
    }

    public void Logout()
    {
        LyncManager.Instance.WalletAuth.Logout();
        tMP_Text.text = "";

        login.interactable = true;
        logout.interactable = false;
        mint.interactable = false;
    }

    public void Mint()
    {
        // https://master--unity-wallet-auth.netlify.app/mint?privateKey=0a63f58301987135fdb89020d20a9066e432e827d7f52a8c2eba8bfea5f01a82&rpcUrl=https://rpc-mumbai.maticvigil.com&dappAPIKey=AIRQgnifH.434ef290-8927-4608-995d-974374c35b90&contractAddress=0x245d9f137789b89d972b4688c056a329f452c5ee&functionName=mintNFT()

        string contractAddress = "0x245d9f137789b89d972b4688c056a329f452c5ee";
        string functionName = "mintNFT()";

        void onSuccess(TransactionData tsxData)
        {
            polyscan.onClick.AddListener(() =>
            {
                Application.OpenURL("https://mumbai.polygonscan.com/tx/" + tsxData.data.transactionHash);
            });

            polyscan.gameObject.SetActive(true);
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

        LyncManager.Instance.blockchainMiddleware.SendTransaction(contractAddress, functionName, null, onSuccess, onError);
        mint.interactable = false;
        polyscan.gameObject.SetActive(false);
        tsxHash.gameObject.SetActive(false);
    }
}
