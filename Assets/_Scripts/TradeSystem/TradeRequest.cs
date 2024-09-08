using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class TradeRequest : MonoBehaviourPun
{
    public Transform targetPlayer; // Yakındaki hedef oyuncunun Transform'u
    public string targetPlayerNickname; // Yakındaki hedef oyuncunun Transform'u
    float interactionRange = 10.0f; // Etkileşim mesafesi
    public PhotonView targetPhotonView; // Hedef oyuncunun PhotonView'u


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && photonView.IsMine)
        {
            FindClosestPlayer();
            if (targetPlayer == null) return;

            // Hedef oyuncu mesafesi kontrolü
            float distance = Vector3.Distance(transform.position, targetPlayer.position);
            if (distance <= interactionRange)
            {
                // Hedef oyuncunun PhotonView'unu bul
                targetPhotonView = targetPlayer.GetComponent<PhotonView>();
                targetPlayerNickname = targetPhotonView.Owner.NickName;

                if (targetPhotonView != null)
                {
                    // Sadece hedef oyuncuya RPC ile ticaret isteği gönder
                    photonView.RPC("SendTradeRequest", targetPhotonView.Owner, photonView.ViewID);
                    Debug.Log("Ticaret isteği " + targetPhotonView.Owner.NickName + " oyuncusuna gönderildi.");
                }
            }
            else
            {
                Debug.Log("Hedef oyuncu çok uzakta.");
            }
        }
    }

    [PunRPC]
    void SendTradeRequest(int viewID)
    {
        Debug.LogError(viewID);
        TradeRequest localTradeRequest = RCC_PhotonDemo.instance.ourPlayer.GetComponent<TradeRequest>();
        localTradeRequest.targetPhotonView = PhotonView.Find(viewID).GetComponent<PhotonView>();
        localTradeRequest.targetPlayer = localTradeRequest.targetPhotonView.transform;
        localTradeRequest.targetPlayerNickname = localTradeRequest.targetPhotonView.Owner.NickName;
        // Hedef oyuncunun ekranına ticaret isteği popup'ı göster
        TradePopupManager.Instance.ShowTradePopup(localTradeRequest.targetPlayerNickname);
    }


    void FindClosestPlayer()
    {
        // TradeRequest componentine sahip tüm objeleri bul
        TradeRequest[] allPlayers = FindObjectsOfType<TradeRequest>();

        // Eğer birden fazla TradeRequest varsa devam et
        if (allPlayers.Length > 0)
        {
            float closestDistance = interactionRange; // Mesafe için bir limit belirliyoruz.
            Transform closestPlayer = null;

            foreach (TradeRequest player in allPlayers)
            {
                PhotonView playerPhotonView = player.GetComponent<PhotonView>();

                // Kendini listeden çıkarmak için isMine kontrolü yapıyoruz
                if (!playerPhotonView.IsMine)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);

                    // Eğer mesafe en küçük mesafeden küçükse ve limitin içindeyse
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = player.transform;
                    }
                }
            }

            // Eğer yakın oyuncu bulunursa targetPlayer değişkenine ata
            if (closestPlayer != null)
            {
                targetPlayer = closestPlayer;
            }
            else
            {
                targetPlayer = null; // Eğer 2 birimlik mesafede kimse yoksa null yap
            }
        }
        else
        {
            targetPlayer = null; // Hiç oyuncu yoksa null yap
        }
    }

}