using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradePopupManager : MonoBehaviourPun
{
    public static TradePopupManager Instance;
    public GameObject tradePopup;
    public TMP_Text popupText;

    private string senderName;

    void Awake()
    {
        Instance = this;
    }
    public void ShowTradePopup(string sender)
    {
        senderName = sender;
        tradePopup.SetActive(true);
        popupText.text = senderName + " send a trade request to you.";
    }

    public void OnAccept()
    {
        photonView.RPC("TradeRequestResponse", RCC_PhotonDemo.instance.ourPlayer.GetComponent<TradeRequest>().targetPhotonView.Owner, senderName, true);
        TradeRequestResponse(senderName, true);
        tradePopup.SetActive(false);
    }


    public void OnReject()
    {
        photonView.RPC("TradeRequestResponse", RCC_PhotonDemo.instance.ourPlayer.GetComponent<TradeRequest>().targetPhotonView.Owner, senderName, false);
        TradeRequestResponse(senderName, false);
        tradePopup.SetActive(false);
    }

    [PunRPC]
    void TradeRequestResponse(string sender, bool accepted)
    {
        if (accepted)
        {
            TradeWindowManager.Instance.OpenTradeWindow(sender, PhotonNetwork.NickName);
            InventoryController.instance.ShowInventory();
        }
        else
        {
            Debug.Log(sender + " player reject to request.");
        }
    }
}
