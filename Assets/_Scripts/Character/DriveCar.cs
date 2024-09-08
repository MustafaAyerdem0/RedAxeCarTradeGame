using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using StarterAssets;
using UnityEngine;

public class DriveCar : MonoBehaviour
{
    PhotonView photonView;
    PlayerProperty playerProperty;
    ThirdPersonController thirdPersonController;
    RCC_PhotonDemo rCC_PhotonDemo;
    CharacterController characterController;
    RCC_UIDashboardDisplay rCC_UIDashboardDisplay;
    Action getInCarAction;
    Action getOutCarAction;
    readonly float distanceThreshold = 2.0f;
    bool isWithinDistance = false;
    [HideInInspector]
    public bool inCar;


    private void Awake()
    {
        photonView = PhotonView.Get(this);
        playerProperty = GetComponent<PlayerProperty>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        characterController = GetComponent<CharacterController>();
        rCC_PhotonDemo = FindObjectOfType<RCC_PhotonDemo>();
        rCC_UIDashboardDisplay = FindObjectOfType<RCC_UIDashboardDisplay>();
    }

    private void OnEnable()
    {
        getInCarAction += GetInCar;
        getOutCarAction += GetOutCar;
    }

    private void OnDisable()
    {
        getInCarAction -= GetInCar;
        getOutCarAction -= GetOutCar;
    }


    private void GetInCar()
    {
        photonView.RPC("GetInCarRPC", RpcTarget.All);
        thirdPersonController.enabled = false;
        playerProperty.cinemacineBrain.SetActive(false);
        playerProperty.cinemacineVirtualCam.SetActive(false);
        rCC_PhotonDemo.carCamera.SetActive(true);
        RCC_SceneManager.Instance.activePlayerVehicle.canControl = true;
        rCC_UIDashboardDisplay.gauges.SetActive(true);

    }
    public void GetOutCar()
    {
        photonView.RPC("GetOutCarRPC", RpcTarget.All);
        thirdPersonController.enabled = true;
        playerProperty.cinemacineBrain.SetActive(true);
        playerProperty.cinemacineVirtualCam.SetActive(true);
        rCC_PhotonDemo.carCamera.SetActive(false);
        RCC_SceneManager.Instance.activePlayerVehicle.canControl = false;
        Vector3 leftPosition = RCC_SceneManager.Instance.activePlayerVehicle.transform.position + (-RCC_SceneManager.Instance.activePlayerVehicle.transform.right * 2);
        playerProperty.transform.position = leftPosition;
        rCC_UIDashboardDisplay.gauges.SetActive(false);
    }

    [PunRPC]
    private void GetInCarRPC()
    {
        playerProperty.skinMesh.SetActive(false);
        characterController.enabled = false;

    }
    [PunRPC]
    private void GetOutCarRPC()
    {
        playerProperty.skinMesh.SetActive(true);
        characterController.enabled = true;
    }

    public void CheckDistance()
    {
        Vector3 currentPositionXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPositionXZ = new Vector3(RCC_SceneManager.Instance.activePlayerVehicle.transform.position.x, 0, RCC_SceneManager.Instance.activePlayerVehicle.transform.position.z);
        float distance = Vector3.Distance(currentPositionXZ, targetPositionXZ);
        isWithinDistance = distance < distanceThreshold;
    }

    private void Update()
    {
        if (photonView.IsMine && RCC_SceneManager.Instance.activePlayerVehicle && RCC_SceneManager.Instance.activePlayerVehicle.GetComponent<PhotonView>().IsMine)
        {
            CheckDistance();

            if (Input.GetKeyDown(KeyCode.F) && isWithinDistance && !inCar)
            {
                getInCarAction?.Invoke();
                inCar = true;
            }

            else if (Input.GetKeyDown(KeyCode.F) && inCar)
            {
                getOutCarAction?.Invoke();
                inCar = false;
            }

            if (isWithinDistance && !inCar) rCC_UIDashboardDisplay.DriveCarInfo.SetActive(true);
            else rCC_UIDashboardDisplay.DriveCarInfo.SetActive(false);
        }
    }
}
