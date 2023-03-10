using TMPro;
using UnityEngine;

namespace Assets.Scripts.Both.UIHolder
{
    public class ResultPanelHolder : MonoBehaviour
    {
        public GameObject Container;
        public TMP_Text Label;
        public TMP_Text Content;
        public TMP_Text TimingText;

        private bool isStart;

        public void StartTiming() => isStart = true;

        private void Update()
        {
            if (!isStart) return;

            var numTxt = GameController.Instance.Timer.Value <= 0 ? "0.00" : GameController.Instance.Timer.Value.ToString("F");
            TimingText.text = "Remaining to reroom " + numTxt + "s";
        }
    }
}