using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class PlayfabLoginUi : MonoBehaviour
{
    public static PlayfabLoginUi instance;

    public InputField registerMailText;
    public InputField registerpasswordText;

    public InputField loginMailText;
    public InputField loginpasswordText;
    public Text statusText;


    public InputField photonNickname;
    public Button photonConnectButton;

    private void OnEnable()
    {
        PlayfabManager.onLoginSuccessAction += OpenPhotonPanel;
    }

    private void OnDisable()
    {
        PlayfabManager.onLoginSuccessAction -= OpenPhotonPanel;
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    private void Start()
    {
        photonNickname.interactable = false;
        photonConnectButton.interactable = false;
    }

    public void OpenPhotonPanel()
    {
        photonNickname.interactable = true;
        photonConnectButton.interactable = true;
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = registerMailText.text,
            Password = registerpasswordText.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, PlayfabManager.instance.OnRegisterSuccess, PlayfabManager.instance.OnError);
    }

    public void Login()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = PlayfabLoginUi.instance.loginMailText.text,
            Password = PlayfabLoginUi.instance.loginpasswordText.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, PlayfabManager.instance.OnLoginSuccess, PlayfabManager.instance.OnError);
    }



}
