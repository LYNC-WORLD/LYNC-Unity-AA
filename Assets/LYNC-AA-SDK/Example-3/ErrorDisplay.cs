using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorDisplay : MonoBehaviour
{
    public GameObject errorGO;
    public static ErrorDisplay Instance { get; private set; } = null;

    private void Awake()
    {
        Instance = this;
    }

    public static void ShowError(string error)
    {
        Debug.Log("Showing error...");
        var go = Instantiate(Instance.errorGO, Vector3.zero, Quaternion.identity);
        go.transform.Find("Panel").GetComponentInChildren<TMPro.TMP_Text>().text = error;
        go.transform.Find("Panel").GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => Destroy(go));
    }
}
