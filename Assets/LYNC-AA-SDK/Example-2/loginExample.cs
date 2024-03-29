using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LYNC;
using LYNC.Wallet;
using UnityEngine.SceneManagement;

public class loginExample : MonoBehaviour
{
    public Button loginButton;
    public string loginUrl;
    void Start()
    {
        loginButton.interactable = LyncManager.Instance.isLyncReady;
        LyncManager.onLyncReady += (lyncManager) =>
        {
            loginButton.interactable = true;
            WalletData walletData = WalletData.TryLoadSavedWallet();
            if (walletData.WalletConnected)
            {
                Debug.Log("CONNECTED");
                GoToNextScene();
            }
        };

        loginButton.onClick.AddListener(() =>
        {
            LyncManager.Instance.WalletAuth.ConnectWallet(loginUrl, (walletData) =>
            {
                GoToNextScene();
            });
        });
    }

    private void GoToNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log("GoToNextScene()");
        SceneManager.LoadScene(nextSceneIndex);
    }
}
