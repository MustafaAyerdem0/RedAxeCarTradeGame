using UnityEngine;

public class TradeWindowManager : MonoBehaviour
{
    public static TradeWindowManager Instance;
    public GameObject tradeWindow;

    void Awake()
    {
        Instance = this;
    }
    public void OpenTradeWindow(string player1, string player2)
    {
        tradeWindow.SetActive(true);
        Debug.Log(player1 + " and " + player2 + " start trading.");
    }
}
