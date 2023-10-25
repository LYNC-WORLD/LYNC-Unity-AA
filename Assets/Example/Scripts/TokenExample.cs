using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TokenExample : MonoBehaviour
{
    public Sprite sprite;
    public string title;
    [Space]
    public string contractAddress;
    public string functionName;
    public List<string> args = new List<string>();

    private Button mintBtn;

    private void Awake()
    {
        mintBtn = transform.Find("MintBtn").GetComponent<Button>();
    }

    void Start()
    {


        // OnClick
        mintBtn.onClick.AddListener(() =>
        {
            mintBtn.interactable = false;
            LYNC.LyncManager.Instance.blockchainMiddleware.SendTransaction(contractAddress, functionName, args, OnMintCompleted, onError);
            StartCoroutine(TextAnimator());
        });

        transform.Find("Image").GetComponent<Image>().sprite = sprite;
        transform.Find("titleTxt").GetComponent<TMPro.TMP_Text>().text = title;
        transform.Find("contractAddressTxt").GetComponent<TMPro.TMP_Text>().text = contractAddress;
        transform.Find("functionNameTxt").GetComponent<TMPro.TMP_Text>().text = functionName;
    }


    private async void OnMintCompleted(TransactionData trxData)
    {
        StopAllCoroutines();
        mintBtn.transform.Find("Text (TMP)").GetComponent<TMPro.TMP_Text>().color = Color.green;
        mintBtn.transform.Find("Text (TMP)").GetComponent<TMPro.TMP_Text>().text = "Minted";
        await Task.Delay(2000);
        mintBtn.interactable = true;
        mintBtn.transform.Find("Text (TMP)").GetComponent<TMPro.TMP_Text>().color = Color.magenta;
        mintBtn.transform.Find("Text (TMP)").GetComponent<TMPro.TMP_Text>().text = "Polyscan";
        mintBtn.onClick.RemoveAllListeners();
        mintBtn.onClick.AddListener(() => OpenPolyscan(trxData.data.transactionHash));
    }

    private async void onError(string msg)
    {
        StopAllCoroutines();
        mintBtn.transform.Find("Text (TMP)").GetComponent<TMPro.TMP_Text>().text = msg;
        await Task.Delay(2000);
        mintBtn.interactable = true;
    }

    private void OpenPolyscan(string trxHash) => Application.OpenURL("https://mumbai.polygonscan.com/tx/" + trxHash);

    public void SetInteractable(bool interactable) => mintBtn.interactable = interactable;

    private IEnumerator TextAnimator()
    {
        yield return new WaitForSeconds(0.3f);
        TMPro.TMP_Text text = mintBtn.transform.Find("Text (TMP)").GetComponent<TMPro.TMP_Text>();
        text.text += ".";
        text.text = text.text.Replace("....", "");
        text.text = text.text.Replace("Mint", "Minting");
        text.text = text.text.Replace("Mintinging", "Minting");
        StartCoroutine(TextAnimator());
    }
}
