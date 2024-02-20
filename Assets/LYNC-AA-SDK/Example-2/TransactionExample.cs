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
            string contractAddress = "0x245d9f137789b89d972b4688c056a329f452c5ee";
            string functionName = "mintNFT()";
            send721Trx.interactable = false;
            LYNC.LyncManager.Instance.blockchainMiddleware.SendUserPaidTransaction(contractAddress, functionName, null,"0", On721TrxCompleted, onError);
        });


        // ******************* 1155 *******************
        send1155Trx.onClick.AddListener(() =>
        {
            string contractAddress = "0x537d5298759F0151A1a08c9215F4e0382722d5C9";
            string functionName = "mint(uint256 id, uint256 amount)";
            List<string> args = new List<string> { "1", "1" };
            send1155Trx.interactable = false;
            LYNC.LyncManager.Instance.blockchainMiddleware.SendUserPaidTransaction(contractAddress, functionName, args,"0", On721TrxCompleted, onError);
        });

        // ******************* 20 *******************
        send20Trx.onClick.AddListener(() =>
        {
            string contractAddress = "0xab2C8D39B611Eee2c0ABc9C12Af0221c1e861879";
            string functionName = "sendToken(uint256 amount)";
            List<string> args = new List<string> { "10" };
            send20Trx.interactable = false;
            LYNC.LyncManager.Instance.blockchainMiddleware.SendUserPaidTransaction(contractAddress, functionName, args,"0", On721TrxCompleted, onError);
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
        Debug.Log("Error: "+msg);
    }

    private void GoToPreviousScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + -1;
        SceneManager.LoadScene(nextSceneIndex);
    }

}
