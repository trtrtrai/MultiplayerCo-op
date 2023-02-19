using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.DynamicObject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
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
                            EffectTypeBehaviour(t, args);

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

        private void EffectTypeBehaviour(SkillTag tag, SkillPackageEventArg args)
        {
            if (tag.Tag != TagType.Effect) return;
            ICreature creature = args.Caster.GetComponent<ICreature>();
            if (creature is null) return;
            var cont = (creature as NetworkBehaviour).GetComponentsInChildren<Transform>().ToListPooled().FirstOrDefault(o => o.name.Equals("TempContainer"));
            switch (tag.Effect)
            {
                case EffectTag.Add:
                    {
                        var obj = Instantiate(Resources.Load<GameObject>("DynamicObject/StatsTemp"), cont);
                        obj.AddComponent<TemporaryStats>().Setup(creature, tag.StatsType, (int)tag.EffectNumber, tag.Duration);

                        break;
                    }
            }
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
                            StartCoroutine(Wait(tag.Duration, () => { if (anim != null) anim.SetBool("isAttack", false); }));
                        }
                        catch
                        {

                        }

                        break;
                    }
                case AttackTag.Bullet:
                    {
                        var bullet = Instantiate(Resources.Load<GameObject>("DynamicObject/Bullet/Bullet"));

                        var direction = GetSkillDirection(args.Caster.GetComponent<Animator>());
                        bullet.transform.localPosition = args.Caster.transform.localPosition + (Vector3)(args.CastPlace * direction);

                        IBulletInitial script = bullet.GetComponent<Bullet>();

                        var damage = creature.GetStats(Both.Scriptable.StatsType.Strength).GetValue();
                        if (tag.AddOrMultiple) //multiple
                        {
                            damage = (int)(damage * tag.EffectNumber);
                        }
                        else
                        {
                            damage = damage + (int)tag.EffectNumber;
                        }

                        script.InjectBulletInfo(damage, direction, 250f, creature, tag.Duration);

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
                        var offset = Vector3.zero; //optimize: spawn with odd/even number?
                        for (int i = 0; i < tag.SummonAmount; i++)
                        {
                            switch (tag.Place)
                            {
                                case SummonPlace.Position:
                                    {
                                        Summon(tag.SummonCreature, args.Caster.transform.localPosition + offset, args);
                                        break;
                                    }
                                case SummonPlace.Target:
                                    {
                                        if (args.Target is null) return;

                                        Summon(tag.SummonCreature, args.Target.transform.localPosition + offset, args);
                                        break;
                                    }
                            }

                            offset.x += 0.5f;
                            offset.y += 0.5f;
                        }

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

        private void Summon(string name, Vector3 position, SkillPackageEventArg args)
        {
            var critter = GameController.Instance.CreatureInstantiate(name);
            critter.tag = GetCreatureTag(args.Caster.tag);

            critter.transform.localPosition = position;

            var script = critter.GetComponent<Both.Creature.Creature>();
            GameController.Instance.SpawnCreature(script, critter.tag, true);
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

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}