using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class TradeRequest : MonoBehaviourPun
{
    public Transform targetPlayer; // Yakındaki hedef oyuncunun Transform'u
    float interactionRange = 10.0f; // Etkileşim mesafesi
    private PhotonView targetPhotonView; // Hedef oyuncunun PhotonView'u

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && photonView.IsMine)
        {
            FindClosestPlayer();
            if (targetPlayer == null) return;

            // Hedef oyuncu mesafesi kontrolü
            float distance = Vector3.Distance(transform.position, targetPlayer.position);
            if (distance <= interactionRange)
            {
                // Hedef oyuncunun PhotonView'unu bul
                targetPhotonView = targetPlayer.GetComponent<PhotonView>();

                if (targetPhotonView != null)
                {
                    // Sadece hedef oyuncuya RPC ile ticaret isteği gönder
                    photonView.RPC("SendTradeRequest", targetPhotonView.Owner, PhotonNetwork.NickName);
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
    void SendTradeRequest(string senderName)
    {
        // Hedef oyuncunun ekranına ticaret isteği popup'ı göster
        TradePopupManager.Instance.ShowTradePopup(senderName);
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