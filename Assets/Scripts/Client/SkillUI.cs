using Assets.Scripts.Both.Creature.Attackable.SkillExecute;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Client
{
    public class SkillUI : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        [SerializeField] Image imgCD;
        [SerializeField] float timer;

        SkillActive skill;
        public void Setup(SkillActive skill) //after setup skill.Timer auto countdown will see it, dont asign new var
        {
            timer = skill.Timer;

            this.skill = skill;
        }

        void Start()
        {
            if (text is null || imgCD is null)
            {
                gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            if (skill is null) return;

            if (!skill.CanActive && skill.Timer < timer)
            {
                if (!imgCD.enabled) imgCD.enabled = true;
                imgCD.fillAmount = skill.Timer / timer;
            }
            else
            {
                if (imgCD.enabled)
                {
                    imgCD.enabled = false;
                    imgCD.fillAmount = 1f;
                }
            }
        }
    }
}