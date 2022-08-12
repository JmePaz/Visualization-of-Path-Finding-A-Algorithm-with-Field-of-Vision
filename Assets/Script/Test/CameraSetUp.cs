using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSetUp : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Camera cameraUI;
    [SerializeField] private GameObject followPlayerCameraObj;
    [SerializeField] private GameObject topViewCameraObj;


    [SerializeField] private CinemachineVirtualCamera followPlayerVCamera;
    [SerializeField] private CinemachineVirtualCamera topViewVCamera;
    void Start()
    {
        //get component
        // followPlayerVCamera = followPlayerCameraObj.GetComponentInChildren<CinemachineVirtualCamera>();
        // topViewVCamera = topViewCameraObj.GetComponentInChildren<CinemachineVirtualCamera>();
        // Debug.Log("Follow Player V Camera " +followPlayerVCamera.enabled);
        // Debug.Log("Top View V Camera " +topViewVCamera.enabled);

        // set all to false
        SetActiveVirtualCameras(false);
    }

    internal void SetUpPlayerCamera(GameObject player1, GameObject player2){

        // for close up camera
        followPlayerVCamera.m_Follow = player1.transform;
        // for top view camera
        topViewVCamera.m_Follow = player2.transform;
       // topViewVCamera.m_LookAt = player.transform;

        // open all virtual cameras
        Debug.Log("Camera Activated");
        SetActiveVirtualCameras(true);

        // closing ui camera
        cameraUI.enabled = false;

    }

    private void SetActiveVirtualCameras(bool state){
        followPlayerCameraObj.SetActive(state);
        topViewCameraObj.SetActive(state);
    }

    internal void SetUpCameraUIBack(){
        SetActiveVirtualCameras(false);
        cameraUI.enabled = true;
    }

}
