using TMPro;
using UnityEngine;

namespace Assets.Scripts.Both.DynamicObject
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;

        public void Setup(Vector3 direction, int damage)
        {
            GetComponentInChildren<TMP_Text>().text = "-" + damage.ToString();
            this.direction = direction;

            gameObject.AddComponent<AutoDestroy>().Setup(0.5f);
        }

        private void FixedUpdate()
        {
            GetComponent<RectTransform>().position = Vector3.Lerp(GetComponent<RectTransform>().position, GetComponent<RectTransform>().position + direction, .8f);
        }
    }
}