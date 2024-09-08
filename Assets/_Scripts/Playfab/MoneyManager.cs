using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;
    public TMP_Text moneyText;

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
    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        moneyText.text = PlayerData.instance.ourMoney.ToString();
    }
    public int GetMaxMoney(int value)
    {
        return Mathf.Min(PlayerData.instance.ourMoney, value);
    }
}
