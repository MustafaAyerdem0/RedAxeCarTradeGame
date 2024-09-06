using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradePopupManager : MonoBehaviourPun
{
    public static TradePopupManager Instance;
    public GameObject tradePopup; // Ticaret popup penceresi
    public TMP_Text popupText; // Popup'da gösterilecek metin

    private string senderName;

    void Awake()
    {
        Instance = this;
    }

    public void ShowTradePopup(string sender)
    {
        senderName = sender;
        tradePopup.SetActive(true); // Popup'ı aç
        popupText.text = senderName + " sana pazarlık yapmak için istek gönderdi.";
    }

    // Kabul butonuna tıklandığında çağrılır
    public void OnAccept()
    {
        // RPC ile kabul bilgisini hem isteği gönderen oyuncuya hem de hedef oyuncuya gönder
        photonView.RPC("TradeRequestResponse", RpcTarget.All, senderName, true);
        tradePopup.SetActive(false); // Popup'ı kapat
    }

    // Reddet butonuna tıklandığında çağrılır
    public void OnReject()
    {
        // RPC ile red bilgisini isteği gönderen oyuncuya gönder
        photonView.RPC("TradeRequestResponse", RpcTarget.All, senderName, false);
        tradePopup.SetActive(false); // Popup'ı kapat
    }

    [PunRPC]
    void TradeRequestResponse(string sender, bool accepted)
    {
        if (accepted)
        {
            // İstek kabul edildiyse her iki oyuncu için de ticaret penceresini aç
            TradeWindowManager.Instance.OpenTradeWindow(sender, PhotonNetwork.NickName);
        }
        else
        {
            // İstek reddedildiyse mesaj göster
            Debug.Log(sender + " oyuncusu isteği reddetti.");
        }
    }
}
