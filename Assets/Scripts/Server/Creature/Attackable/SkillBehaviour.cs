using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.DynamicObject;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Server.Creature.Attackable
{
    public class SkillBehaviour : MonoBehaviour
    {
        public static SkillBehaviour Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void Cast(List<SkillTag> tags, SkillPackageEventArg args)
        {
            tags.ForEach(t => { 
                switch (t.Tag)
                {
                    case TagType.Attack:
                        {
                            AttackTypeBehaviour(t, args);
                            
                            return;
                        }
                    case TagType.Effect:
                        {
                            return;
                        }
                    case TagType.Special:
                        {
                            SpecialTypeBehaviour(t, args);

                            return;
                        }
                }
            });
        }

        private void AttackTypeBehaviour(SkillTag tag, SkillPackageEventArg args)
        {
            if (tag.Tag != TagType.Attack) return;
            ICreature creature = args.Caster.GetComponent<ICreature>();
            if (creature is null) return;

            switch (tag.Attack)
            {
                case AttackTag.Normal:
                    {
                        var anim = args.Caster.GetComponent<Animator>();
                        try
                        {
                            anim.SetBool("isAttack", true);
                            StartCoroutine(Wait(tag.Duration, () => anim.SetBool("isAttack", false)));
                        }
                        catch
                        {

                        }

                        break;
                    }
                case AttackTag.Bullet:
                    {
                        var bullet = Instantiate(Resources.Load<GameObject>("DynamicObject/Bullet/Bullet"));
                        bullet.tag = args.Caster.tag;

                        var direction = GetSkillDirection(args.Caster.GetComponent<Animator>());
                        bullet.transform.localPosition = args.Caster.transform.localPosition + (Vector3)(args.CastPlace * direction);
                        //Debug.Log(bullet.transform.localPosition);
                        IBulletInitial script = bullet.GetComponent<Bullet>();
                        script.InjectBulletInfo(100, direction, 175f);

                        GameController.Instance.SpawnGameObject(bullet, true);
                        break;
                    }
                case AttackTag.SelfArea:
                    {
                        break;
                    }
                case AttackTag.TargetArea:
                    {
                        break;
                    }
            }
        }

        private void SpecialTypeBehaviour(SkillTag tag, SkillPackageEventArg args)
        {
            if (tag.Tag != TagType.Special) return;
            ICreature creature = args.Caster.GetComponent<ICreature>();
            if (creature is null) return; //Caster dead?

            switch (tag.Special)
            {
                case SpecialTag.Summon:
                    {
                        var critter = GameController.Instance.CreatureInstantiate(tag.SummonCreature, GetCreatureTag(args.Caster.tag));
                        critter.tag = GetCreatureTag(args.Caster.tag);

                        switch (tag.Place)
                        {
                            case SummonPlace.Position:
                                {
                                    critter.transform.localPosition = args.Caster.transform.localPosition;
                                    break;
                                }
                            case SummonPlace.Target:
                                {
                                    if (args.Target is null) return;

                                    critter.transform.localPosition = args.Target.transform.localPosition;
                                    break;
                                }
                        }

                        GameController.Instance.SpawnGameObject(critter, true);
                        break;
                    }
                case SpecialTag.Teleport:
                    {
                        break;
                    }
                case SpecialTag.Immortal:
                    {
                        break;
                    }
            }
        }

        private Vector2 GetSkillDirection(Animator animator)
        {
            switch (Mathf.Abs(animator.GetInteger("orientation")))
            {
                case 1:
                    {
                        return Vector2.up;
                    }
                case 2:
                    {
                        return Vector2.down;
                    }
                case 3:
                    {
                        return Vector2.left;
                    }
                case 4:
                    {
                        return Vector2.right;
                    }
                default:
                    {
                        return Vector2.zero;
                    }
            }
        }

        private string GetCreatureTag(string casterTag)
        {
            switch (casterTag)
            {
                case "Character":
                    {
                        return "Ally";
                    }
                case "Boss":
                    {
                        return "Enemy";
                    }
                case "Mobs":
                    {
                        return "Mobs";
                    }
                case "Enemy":
                    {
                        return "Enemy";
                    }
                case "Ally":
                    {
                        return "Ally";
                    }
            }

            return "Mobs";
        } //for summon creature

        private IEnumerator Wait(float time, Action callback)
        {
            yield return new WaitForSeconds(time);

            callback();
        }
    }
}