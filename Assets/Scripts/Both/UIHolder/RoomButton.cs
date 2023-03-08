using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    public Button BackBtn;
    public Button StartGameBtn;
    public List<RoomPlayer> Players;
    //kick player btn?

    public TMP_Text IpAddrTxt;

    public void ShutDown()
    {
        RoomController.Instance.OutRoom();
    }

    public void StartGame()
    {
        RoomController.Instance.StartGame();
    }
}
