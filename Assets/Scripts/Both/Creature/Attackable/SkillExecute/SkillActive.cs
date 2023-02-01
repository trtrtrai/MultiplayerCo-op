using Assets.Scripts.Both.Creature.Controllers;
using Assets.Scripts.Both.Scriptable;
using Assets.Scripts.Server.Creature.Attackable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class SkillActive : MonoBehaviour, IActiveDetect
    {
        [SerializeField] protected ICreatureController owner;
        [SerializeField] protected ISkill creatureSkill;
        [SerializeField] protected List<SkillTag> skillTags;
        [SerializeField] protected bool canActive;
        [SerializeField] protected float timer;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            //Debug.Log("SkillActive Start");
            //var ownerObj = FindObjectsOfType<Creature>().ToList().FirstOrDefault(t => t.GetComponent<NetworkObject>().NetworkObjectId.ToString() == gameObject.transform.parent.name);
            owner = gameObject.transform.parent.GetComponent<ICreatureController>(); //ownerObj.GetComponent<ICreatureController>();

            if (owner is null)
            {
                //Debug.Log("Skill's owner is missing!");
                //Destroy
                gameObject.SetActive(false);
            }

            creatureSkill = (owner as NetworkBehaviour).GetComponent<ICreature>().GetSkills().FirstOrDefault(i => i.SkillName.ToString().Equals(gameObject.name));

            skillTags = Resources.Load<SkillModel>("AssetObjects/Skills/" + creatureSkill.SkillName.ToString()).SkillTags;
            creatureSkill.AddListener(ActivateSkill);
            ResetSkill();
        }

        protected virtual void ActivateSkill(Action callback, NetworkObject owner)
        {
            if (!canActive) return;

            canActive = false;
            StartCoroutine(DelayActive(callback));
        }

        private IEnumerator DelayActive(Action callback)
        {
            yield return new WaitForSeconds(creatureSkill.CastDelay);

            //Debug.Log("Activated " + creatureSkill.SkillName);

            StartCoroutine(CDSkill(callback));

            SpecializedBehaviour();
        }

        private IEnumerator CDSkill(Action callback)
        {
            while (timer > 0f)
            {
                timer -= Time.fixedDeltaTime; // - something...

                yield return null;
            }

            ResetSkill();
            callback();
        }

        private void SkillTagExecute()
        {
            // maybe call to GameController
            SkillBehaviour.Instance.Cast(skillTags);
        }

        private void ResetSkill()
        {
            timer = creatureSkill.Cooldown;
            canActive = true;
        }

        protected virtual void SpecializedBehaviour()
        {
            SkillTagExecute();
        }

        public virtual void SkillTagExecuteCollider2d(GameObject obj)
        {
            
        }

        public virtual void SkillTagExecuteTrigger2d(GameObject obj)
        {
            
        }
    }

    public interface IActiveDetect
    {
        void SkillTagExecuteCollider2d(GameObject obj);
        void SkillTagExecuteTrigger2d(GameObject obj);
    }
}