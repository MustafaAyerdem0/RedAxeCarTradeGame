using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TradeWindow : MonoBehaviourPun
{
    public TMP_Text otherPlayerNameText;
    TradeRequest localTradeRequest;
    public Toggle ourToggle;
    public Toggle otherToggle;

    public TMP_Text ourMoney;
    public TMP_Text otherMoney;

    private void OnEnable()
    {
        localTradeRequest = RCC_PhotonDemo.instance.ourPlayer?.GetComponent<TradeRequest>();
        otherPlayerNameText.text = localTradeRequest?.targetPlayerNickname;
    }

    public void ChangeMoney()
    {
        photonView.RPC("ChangeMoneyRPC", localTradeRequest.targetPhotonView.Owner, ourMoney.text);
    }


    [PunRPC]
    public void ChangeMoneyRPC(string moneyCount)
    {
        otherMoney.text = moneyCount;
    }

    public void ConfirmTrade()
    {
        ourMoney.transform.parent.parent.GetComponent<TMP_InputField>().interactable = false;
        photonView.RPC("ConfirmTradeRPC", localTradeRequest.targetPhotonView.Owner);
    }

    [PunRPC]
    public void ConfirmTradeRPC()
    {
        otherToggle.isOn = true;
    }
}
