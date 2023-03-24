using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Both.UIHolder
{
    public class ResultPanelHolder : MonoBehaviour
    {
        public GameObject Container;
        public TMP_Text Label;
        public TMP_Text Content;
        public Button ReroomBtn;
        public List<GameObject> LeaderboardItems;

        public void OutGamePlay()
        {
            GameController.Instance.ToRoomScene();
        }

        public TMP_Text GetLeaderboardNameTxt(int index)
        {
            return LeaderboardItems[index].transform.GetChild(0).GetComponentInChildren<TMP_Text>();
        }

        public TMP_Text GetLeaderboardDmgTxt(int index)
        {
            return LeaderboardItems[index].transform.GetChild(1).GetChild(0).GetComponentsInChildren<TMP_Text>()[1];
        }

        public TMP_Text GetLeaderboardHealingTxt(int index)
        {
            return LeaderboardItems[index].transform.GetChild(1).GetChild(1).GetComponentsInChildren<TMP_Text>()[1];
        }

        public void ActiveLeaderboadItem(int index)
        {
            LeaderboardItems[index].SetActive(true);
        }
    }
}