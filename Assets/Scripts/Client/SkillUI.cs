using Assets.Scripts.Both.Creature.Attackable.SkillExecute;
using Assets.Scripts.Both.Creature.Controllers;
using Assets.Scripts.Both.Scriptable;
using System.Collections;
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
        [SerializeField] float CD;
        [SerializeField] bool trigger;
        [SerializeField] int index;

        SkillActive skill;
        PlayerControl control;
        public void Setup(SkillActive skill, PlayerControl control, int index) //after setup skill.Timer auto countdown will see it, dont asign new var
        {
            timer = skill.Timer;
            CD = skill.Timer;

            this.skill = skill;
            this.control = control;
            this.index = index;

            var script = Resources.Load<SkillModel>("AssetObjects/Skills/" + skill.name);

            if (script.SkillIcon)
            {
                GetComponent<Image>().sprite = script.SkillIcon;
            }

            switch (this.index) 
            {
                case 0:
                    {
                        trigger = this.control.AttackTrigger.Value;
                        this.control.AttackTrigger.OnValueChanged += Triggered;
                        break;
                    }
                case 1:
                    {
                        trigger = this.control.SpAttackTrigger.Value;
                        this.control.SpAttackTrigger.OnValueChanged += Triggered;
                        break;
                    }
                case 2:
                    {
                        trigger = this.control.SpAttackTrigger2.Value;
                        this.control.SpAttackTrigger2.OnValueChanged += Triggered;
                        break;
                    }
            }           
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
            if (skill is null || control is null) return;

            if (trigger && timer < CD) //detect playercontrol
            {
                if (!imgCD.enabled) imgCD.enabled = true;
                imgCD.fillAmount = timer / CD;
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

        private void Triggered(bool oldV, bool newV)
        {
            trigger = newV;

            if (newV)
            {
                StartCoroutine(CDSkill());
            }
            else
            {
                timer = skill.Timer; // skill.Timer can be reduce or increase if game have new features
                CD = skill.Timer; // so it's not const variable
            }
        }

        private IEnumerator CDSkill()
        {
            while (timer > 0f)
            {
                timer -= Time.fixedDeltaTime / 2f;

                yield return null;
            }
        }

        private void OnDestroy()
        {
            switch (index)
            {
                case 0:
                    {
                        control.AttackTrigger.OnValueChanged -= Triggered;
                        break;
                    }
                case 1:
                    {
                        control.SpAttackTrigger.OnValueChanged -= Triggered;
                        break;
                    }
                case 2:
                    {
                        control.SpAttackTrigger2.OnValueChanged -= Triggered;
                        break;
                    }
            }
        }
    }
}