using System;
using Assets.Scripts.Both.Scriptable;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Both.UIHolder
{
    public class RoomButton : MonoBehaviour
    {
        public Button BackBtn;
        public Button StartGameBtn;
        public List<RoomPlayer> Players;
        //kick player btn?

        public TMP_Text IpAddrTxt;

        public BossNameHolder Script;

        public void DisableBossChoice()
        {
            Script.ListBossBtn.ForEach((b) =>
            {
                b.interactable = false;
            });
        }

        public void ShutDown()
        {
            RoomController.Instance.OutRoom();
        }

        public void StartGame()
        {
            NetworkListener.BossName = (BossName)Enum.Parse(typeof(BossName), Script.Current.GetComponentInChildren<TMP_Text>().text);
            RoomController.Instance.StartGame();
        }
    }
}