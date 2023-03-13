using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class GameButton : MonoBehaviour
{
    public Button SettingBtn;
    private GameObject playerCtrl;

    private void Start()
    {
        StartCoroutine(FindPlayerControl());
    }

    public void OutGamePlay()
    {
        playerCtrl.GetComponent<PlayerInput>().enabled = true;
        /*playerCtrl.SetActive(true);*/
        GameController.Instance.ToRoomScene();
    }

    public void ResetPlayerInput()
    {
        playerCtrl.GetComponent<PlayerInput>().actions["PlayMenu"].started -= A_started;
        playerCtrl.GetComponent<PlayerInput>().enabled = true;
        /*playerCtrl.SetActive(true);*/
    }

    public void OnSettingBtnClick()
    {
        playerCtrl.GetComponent<PlayerInput>().actions["PlayMenu"].started -= A_started;
        playerCtrl.GetComponent<PlayerInput>().enabled = false;
        /*playerCtrl.SetActive(false);*/
    }

    public void OnContinueBtnClick()
    {
        if (playerCtrl is null) return;

        playerCtrl.GetComponent<PlayerInput>().enabled = true;
        playerCtrl.GetComponent<PlayerInput>().actions["PlayMenu"].started += A_started;
        /*playerCtrl.SetActive(true);*/
    }

    IEnumerator FindPlayerControl()
    {
        while (playerCtrl is null)
        {
            var pCs = GameObject.FindGameObjectsWithTag("Player");

            pCs.ToList().ForEach((p) =>
            {
                //Debug.Log("Butn" + p.GetComponent<NetworkObject>().OwnerClientId + " " + NetworkManager.Singleton.LocalClientId);
                if (p.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    playerCtrl = p;
                }
            });

            yield return null;
        }

        playerCtrl.GetComponent<PlayerInput>().actions["PlayMenu"].started += A_started;
    }

    private void A_started(CallbackContext callback)
    {
        SettingBtn.onClick.Invoke();
    }
}
