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
        public List<GameObject> LeaderBoardItems;

        public void OutGamePlay()
        {
            GameController.Instance.ToRoomScene();
        }
    }
}