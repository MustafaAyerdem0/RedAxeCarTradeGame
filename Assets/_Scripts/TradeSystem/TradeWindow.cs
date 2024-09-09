using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TradeWindow : MonoBehaviourPun
{
    [SerializeField]
    private TMP_Text otherPlayerNameText;
    TradeRequest localTradeRequest;
    public Toggle ourToggle;
    public Toggle otherToggle;

    public TMP_Text ourMoney;
    public TMP_Text otherMoney;
    public TMP_InputField ourMoneyInputField;

    public static TradeWindow instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnEnable()
    {
        localTradeRequest = RCC_PhotonDemo.instance.ourPlayer?.GetComponent<TradeRequest>();
        otherPlayerNameText.text = localTradeRequest?.targetPlayerNickname;
        ourMoneyInputField.interactable = true;
        ourToggle.interactable = true;
        ourToggle.isOn = false;
        otherToggle.isOn = false;
    }

    public void ChangeMoney()
    {
        int intMoney;
        if (int.TryParse(ourMoneyInputField.text, out intMoney))
        {
            ourMoneyInputField.text = MoneyManager.instance.GetMaxMoney(intMoney).ToString();
            //Debug.LogError("cast edildi");
        }
        photonView.RPC("ChangeMoneyRPC", localTradeRequest.targetPhotonView.Owner, ourMoneyInputField.text);

    }


    [PunRPC]
    public void ChangeMoneyRPC(string moneyCount)
    {
        otherMoney.text = moneyCount;
    }

    public void ConfirmTrade()
    {
        ourMoneyInputField.interactable = false;
        photonView.RPC("ConfirmTradeRPC", localTradeRequest.targetPhotonView.Owner);
    }

    [PunRPC]
    public void ConfirmTradeRPC()
    {
        otherToggle.isOn = true;
        ourMoneyInputField.interactable = false;

    }

    public void CheckTradeAgreement()
    {
        if (ourToggle.isOn && otherToggle.isOn)
            InventoryController.instance.ExitTrade();
    }

    public void ToggleOnValueChanged(bool isOn)
    {
        if (isOn)
        {
            ourToggle.interactable = false;
            ConfirmTrade();
        }
    }

}
