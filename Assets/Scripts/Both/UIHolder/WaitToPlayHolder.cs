using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Both.UIHolder
{
    public class WaitToPlayHolder : MonoBehaviour
    {
        public GameObject Container;
        public TMP_Text Text;
        public float waitTime;

        public void Setup()
        {
            StartCoroutine(WaitToPlay());
        }

        private void Start()
        {
            if (Container is null || Text is null) gameObject.SetActive(false);
        }

        private IEnumerator WaitToPlay()
        {
            Container.SetActive(true);
            Time.timeScale = 0f;
            while (waitTime > 0f)
            {
                waitTime -= Time.fixedUnscaledDeltaTime / 2;
                Text.text = "boss fight after " + waitTime.ToString("F");

                yield return null;
            }

            Time.timeScale = 1f;
            gameObject.SetActive(false);
        }
    }
}