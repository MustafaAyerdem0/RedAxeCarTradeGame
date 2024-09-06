using UnityEngine;

public class TradeWindowManager : MonoBehaviour
{
    public static TradeWindowManager Instance;
    public GameObject tradeWindow; // Ticaret penceresi UI

    void Awake()
    {
        Instance = this;
    }

    // Ticaret penceresini her iki oyuncu için aç
    public void OpenTradeWindow(string player1, string player2)
    {
        tradeWindow.SetActive(true); // Ticaret penceresini aç
        Debug.Log(player1 + " ve " + player2 + " ticarete başladı.");
    }
}
