using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class PlayerProperty : MonoBehaviourPunCallbacks
{
    public GameObject cinemacineVirtualCam;
    public GameObject cinemacineBrain;
    public GameObject skinMesh;

    private void Start()
    {
        SetLocalPlayer();
    }

    public void SetLocalPlayer()
    {
        if (photonView.IsMine)
        {

        }

        else
        {
            cinemacineVirtualCam.SetActive(false);
            cinemacineBrain.SetActive(false);
            GetComponent<ThirdPersonController>().enabled = false;
        }
    }

}
