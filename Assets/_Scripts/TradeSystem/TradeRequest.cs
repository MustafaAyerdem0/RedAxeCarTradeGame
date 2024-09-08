using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeRequest : MonoBehaviourPun
{
    [HideInInspector]
    public Transform targetPlayer;
    [HideInInspector]
    public string targetPlayerNickname;
    private readonly float interactionRange = 10.0f;
    [HideInInspector]
    public PhotonView targetPhotonView;
    private GameObject tradeRequestInfoPanel;
    private Text tradeRequestInfoText;
    private string lastTradeNickname;


    private void Start()
    {
        tradeRequestInfoPanel = RCC_PhotonDemo.instance.tradeRequestInfoPanel;
        tradeRequestInfoText = RCC_PhotonDemo.instance.tradeRequestInfoText;
    }
    void Update()
    {
        if (photonView.IsMine && !TradeWindowManager.Instance.tradeWindow.activeSelf && !TradePopupManager.Instance.tradePopup.activeSelf)
        {
            FindClosestPlayer();

            if (targetPlayer != null)
            {
                targetPhotonView = targetPlayer.GetComponent<PhotonView>();
                targetPlayerNickname = targetPhotonView.Owner.NickName;
                if (lastTradeNickname != targetPlayerNickname) RCC_PhotonDemo.instance.differentPlayer = true;
                else RCC_PhotonDemo.instance.differentPlayer = false;
                lastTradeNickname = targetPlayerNickname;
                tradeRequestInfoPanel.SetActive(true);
                tradeRequestInfoText.text = "Press T to trade with " + targetPlayerNickname;
            }
            else
            {
                tradeRequestInfoPanel.SetActive(false);
                tradeRequestInfoText.text = "";
                return;
            }

            float distance = Vector3.Distance(transform.position, targetPlayer.position);
            if (Input.GetKeyDown(KeyCode.T) && distance <= interactionRange)
            {

                targetPhotonView = targetPlayer.GetComponent<PhotonView>();
                targetPlayerNickname = targetPhotonView.Owner.NickName;

                if (targetPhotonView != null)
                {
                    photonView.RPC("SendTradeRequest", targetPhotonView.Owner, photonView.ViewID);
                }
            }
        }
        else if (photonView.IsMine)
        {
            tradeRequestInfoPanel.SetActive(false);
            tradeRequestInfoText.text = "";
        }
    }

    [PunRPC]
    void SendTradeRequest(int viewID)
    {
        TradeRequest localTradeRequest = RCC_PhotonDemo.instance.ourPlayer.GetComponent<TradeRequest>();
        localTradeRequest.targetPhotonView = PhotonView.Find(viewID).GetComponent<PhotonView>();
        localTradeRequest.targetPlayer = localTradeRequest.targetPhotonView.transform;
        localTradeRequest.targetPlayerNickname = localTradeRequest.targetPhotonView.Owner.NickName;
        TradePopupManager.Instance.ShowTradePopup(localTradeRequest.targetPlayerNickname);
    }


    void FindClosestPlayer()
    {
        TradeRequest[] allPlayers = FindObjectsOfType<TradeRequest>();
        if (allPlayers.Length > 0)
        {
            float closestDistance = interactionRange;
            Transform closestPlayer = null;
            foreach (TradeRequest player in allPlayers)
            {
                PhotonView playerPhotonView = player.GetComponent<PhotonView>();
                if (!playerPhotonView.IsMine)
                {
                    float distance = Vector3.Distance(transform.position, player.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = player.transform;
                    }
                }
            }
            if (closestPlayer != null) targetPlayer = closestPlayer;
            else targetPlayer = null;
        }
        else targetPlayer = null;
    }

}