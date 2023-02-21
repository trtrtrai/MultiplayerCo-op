using Assets.Scripts.Both.Creature.Controllers;
using Assets.Scripts.Both.DynamicObject;
using Assets.Scripts.Both.Scriptable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Attackable.SkillExecute
{
    public class SkillActive : NetworkBehaviour
    {
        [SerializeField] protected ICreatureController owner;
        [SerializeField] protected ISkill creatureSkill;
        [SerializeField] protected List<SkillTag> skillTags;
        [SerializeField] protected bool canActive;
        [SerializeField] protected float timer;

        protected SkillPackageEventArg current;

        public virtual void SetupSkill()
        {
            //Debug.Log("SkillActive Start");
            if (IsOwner)
            {
                owner = gameObject.transform.parent.GetComponent<ICreatureController>();
                creatureSkill = (owner as NetworkBehaviour).GetComponent<ICreature>().GetSkills().FirstOrDefault(i => i.SkillName.ToString().Equals(gameObject.name));
            }
            else
            {
                var creature = gameObject.transform.parent.GetComponent<ICreature>();
                creatureSkill = creature.GetSkills().FirstOrDefault(i => i.SkillName.ToString().Equals(gameObject.name));
            }

            skillTags = Resources.Load<SkillModel>("AssetObjects/Skills/" + creatureSkill.SkillName.ToString()).SkillTags;
            creatureSkill.AddListener(ActivateSkill);
            ResetSkill();
        }

        protected virtual void ActivateSkill(Action callback, SkillPackageEventArg args)
        {
            if (!canActive) return;

            current = args;
            canActive = false;
            StartCoroutine(DelayActive(callback));
        }

        private IEnumerator DelayActive(Action callback)
        {
            if (skillTags[0].Tag == TagType.Special && skillTags[0].Special == SpecialTag.Summon)
            {
                var skillObj = GameController.Instance.InstantiateGameObject("SkillEffect/" + creatureSkill.SkillName.ToString(), null);
                if (skillObj != null)
                {
                    var position = FindSpawningPlace(current.Target.transform).localPosition;
                    skillObj.transform.localPosition = position;
                    current = new SkillPackageEventArg(current.Caster, current.Target, position);
                    GameController.Instance.SpawnGameObject(skillObj, true);
                    skillObj.AddComponent<AutoDestroy>().Setup(creatureSkill.CastDelay + .5f);
                }      
            }

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
            GameController.Instance.Cast(skillTags, current);
            current = null;
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

        private Transform FindSpawningPlace(Transform targetCreature)
        {
            var objs = GameObject.FindGameObjectsWithTag("SpecialPoint").Select(o => o.transform);

            List<float> distance = new List<float>();


            for (int i = 0; i < objs.Count(); i++)
            {
                distance.Add((objs.ElementAt(i).localPosition - targetCreature.localPosition).magnitude);
            }

            if (distance.Count == 0) return objs.ElementAt(0);

            return objs.ElementAt(distance.IndexOf(distance.Min()));
        }

        public virtual void SkillTagExecuteCollider2d(GameObject obj)
        {
            
        }

        public virtual void SkillTagExecuteTrigger2d(GameObject obj)
        {
            
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopAllCoroutines();
        }
    }

    public interface IActiveDetect
    {
        void SkillTagExecuteCollider2d(GameObject obj);
        void SkillTagExecuteTrigger2d(GameObject obj);
    }
}