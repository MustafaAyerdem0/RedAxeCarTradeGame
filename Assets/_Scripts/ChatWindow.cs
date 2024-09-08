using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatWindow : MonoBehaviour
{
    private void OnEnable()
    {
        //Debug.LogError("chat window opened");
        if (RCC_PhotonDemo.instance.differentPlayer) RCC_PhotonDemo.instance.ClearAllChat();
    }
}
