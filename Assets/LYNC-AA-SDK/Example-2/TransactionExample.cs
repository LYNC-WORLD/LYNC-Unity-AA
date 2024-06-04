using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using LYNC;
using LYNC.Wallet;

public class TransactionExample : MonoBehaviour
{
    public Button logOutButton;
    public TMP_Text eoaAddress;
    public TMP_Text smartAccountAddress;
    public TMP_Text transactionHash;
    public Button send721Trx;
    public Button send1155Trx;
    public Button send20Trx;

    void Start()
    {
        transactionHash.gameObject.SetActive(false);
        WalletData walletData = WalletData.TryLoadSavedWallet();
        if (walletData.WalletConnected)
        {
            Debug.Log("CONNECTED");
            eoaAddress.text = "EOA address : " + walletData.PublicAddress;
            smartAccountAddress.text = "Smart Account Address : " + walletData.SmartAccount;
        }

        logOutButton.onClick.AddListener(() =>
        {
            LyncManager.Instance.WalletAuth.Logout();
            GoToPreviousScene();
        });

        // ******************* 721 *******************
        send721Trx.onClick.AddListener(() =>
        {
            string contractAddress = "0xAf61e084cCB2CcF57792cE8E0800907C1c9A1252";
            string functionName = "mintNewNFT()";
            send721Trx.interactable = false;
            LYNC.LyncManager.Instance.blockchainMiddleware.SendTransaction(contractAddress, functionName, null, On721TrxCompleted, onError);
        });


        // ******************* 1155 *******************
        send1155Trx.onClick.AddListener(() =>
        {
            string contractAddress = "0x79107Ad6bd949bD955640FC1C861A2d2909E2bbD";
            string functionName = "mint(uint256 id, uint256 amount)";
            List<string> args = new List<string> { "1","1"};
            send1155Trx.interactable = false;
            LYNC.LyncManager.Instance.blockchainMiddleware.SendTransaction(contractAddress, functionName, args, On1155TrxCompleted, onError);
        });

        // ******************* 20 *******************
        send20Trx.onClick.AddListener(() =>
        {
            string contractAddress = "0x3F163A36c778ef91417D579C0Ae25F3DF253a0bF";
            string functionName = "sendToken(uint256 amount)";
            List<string> args = new List<string> { "100" };
            send20Trx.interactable = false;
            LYNC.LyncManager.Instance.blockchainMiddleware.SendUserPaidTransaction(contractAddress, functionName, args,"0", On20TrxCompleted, onError);
        });
    }


    private void On721TrxCompleted(TransactionData tsxData)
    {
        Debug.Log("TRX DONE for ERC-721: " + tsxData.data.transactionHash);
        transactionHash.text = "Transaction Completed for ERC-721 " + tsxData.data.transactionHash;
        transactionHash.gameObject.SetActive(true);
        send721Trx.interactable = true;
    }
    private void On1155TrxCompleted(TransactionData tsxData)
    {
        Debug.Log("TRX DONE for ERC-1155: " + tsxData.data.transactionHash);
        transactionHash.text = "Transaction Completed for ERC-1155 " + tsxData.data.transactionHash;
        transactionHash.gameObject.SetActive(true);
        send1155Trx.interactable = true;
    }

    private void On20TrxCompleted(TransactionData tsxData)
    {
        Debug.Log("TRX DONE for ERC-20: " + tsxData.data.transactionHash);
        transactionHash.text = "Transaction Completed for ERC-20 " + tsxData.data.transactionHash;
        transactionHash.gameObject.SetActive(true);
        send20Trx.interactable = true;
    }

    private void onError(string msg){
        transactionHash.text = "Transaction Failed: " + msg;
        transactionHash.gameObject.SetActive(true);
        Debug.Log("Error: "+msg);
    }

    private void GoToPreviousScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + -1;
        SceneManager.LoadScene(nextSceneIndex);
    }

}
