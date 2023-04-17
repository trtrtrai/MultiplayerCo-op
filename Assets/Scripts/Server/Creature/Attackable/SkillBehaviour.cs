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
            var effNum = 0;
            switch (tag.Effect)
            {
                case EffectTag.Add:
                    {
                        effNum = (int)tag.EffectNumber;

                        break;
                    }
                case EffectTag.Multiple:
                    {
                        effNum = (int)(creature.GetStats(tag.StatsType).GetValue() * tag.EffectNumber);

                        break;
                    }
                case EffectTag.Substract:
                    {
                        effNum = -(int)tag.EffectNumber;

                        break;
                    }
                case EffectTag.Divide:
                    {
                        effNum = (int)(creature.GetStats(tag.StatsType).GetValue() / tag.EffectNumber);

                        break;
                    }
            }

            if (!tag.IsEver)
            {
                var obj = Instantiate(Resources.Load<GameObject>("DynamicObject/StatsTemp"), cont);
                obj.AddComponent<TemporaryStats>().Setup(creature, tag.StatsType, effNum, tag.Duration);
            }
            else
            {
                creature.GetStats(tag.StatsType).SetValue(effNum);
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
                        if (args.Target is null) return;

                        if (tag.IsNormal)
                        {
                            var anim = args.Caster.GetComponent<Animator>();
                            try
                            {
                                anim.SetBool("isAttack", true);
                                StartCoroutine(Wait(.5f, () => { if (anim != null) anim.SetBool("isAttack", false); }));
                            }
                            catch
                            {

                            }
                        }

                        var directionEven = tag.Direction == CastDirection.Orientation ? GetSkillDirection(args.Caster.GetComponent<Animator>()) : (Vector2)(args.Target.localPosition - args.Caster.localPosition);
                        var radEven = tag.BulletRadian / 180f; //pos rad

                        var directionOdd = directionEven;
                        var radOdd = (360 - tag.BulletRadian) / 180f; //nega rad

                        UpdateDirection(radEven, ref directionEven); //init even rad
                        for (int i = 1; i <= tag.BulletAmount; i++)
                        {
                            if (i % 2 == 0)
                            {
                                FireBullet(creature, tag, directionEven, args);

                                //Debug.Log(i + ": " + directionEven);
                                UpdateDirection(radEven, ref directionEven);
                            }
                            else
                            {
                                FireBullet(creature, tag, directionOdd, args);

                                //Debug.Log(i + ": " + directionOdd);
                                UpdateDirection(radOdd, ref directionOdd);
                            }
                        }
                        break;
                    }
                case AttackTag.SelfArea:
                    {
                        switch (tag.AreaType)
                        {
                            case AreaType.Instance:
                                {
                                    break;
                                }
                            case AreaType.PerSeconds:
                                {
                                    var skillObj = GameController.Instance.InstantiateGameObject("SkillEffect/" + tag.ObjName, null);
                                    if (skillObj != null)
                                    {
                                        var position = args.Caster.localPosition; //Self --> cater pos
                                        skillObj.transform.localPosition = position;
  
                                        GameController.Instance.SpawnGameObject(skillObj, true);
                                        skillObj.GetComponent<StatsPerSeconds>().Setup(30, tag.PerSeconds, tag.StatsType, creature, GetCreatureTag(args.Caster.tag));
                                        skillObj.AddComponent<AutoDestroy>().Setup(tag.Duration);
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case AttackTag.TargetArea:
                    {
                        break;
                    }
            }

            static void FireBullet(ICreature creature, SkillTag tag, Vector2 direction, SkillPackageEventArg args)
            {
                var bullet = CreateBullet(tag.ObjName, direction, args);

                IBulletInitial script = bullet.GetComponent<Bullet>();

                var damage = creature.GetStats(Both.Scriptable.StatsType.Strength).GetValue();
                if (tag.AddOrMultiple) //multiple
                {
                    damage = (int)(damage * tag.EffectNumber);
                }
                else
                {
                    damage += (int)tag.EffectNumber;
                }

                script.InjectBulletInfo(damage, direction, 325f, creature, tag.Duration);

                GameController.Instance.SpawnGameObject(bullet, true);
            }

            static GameObject CreateBullet(string bulletName, Vector2 direction, SkillPackageEventArg args)
            {
                var bullet = Instantiate(Resources.Load<GameObject>("DynamicObject/Bullet/" + bulletName));

                bullet.transform.localPosition = args.Caster.transform.localPosition + (Vector3)(args.CastPlace * direction);

                return bullet;
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
                        Vector2 position = args.Caster.localPosition;

                        switch (tag.SPlace)
                        {
                            case SummonPlace.Position:
                                {
                                    position = args.CastPlace;
                                    break;
                                }
                            case SummonPlace.Target:
                                {
                                    if (args.Target is null) return;

                                    position = args.Target.transform.localPosition;
                                    break;
                                }
                        }

                        if (tag.SummonAmount % 2 == 1) //Odd num
                        {
                            Summon(tag.SummonCreature, position, args); //origin position
                        }
                        var actuallyAmount = tag.SummonAmount % 2 == 1 ? tag.SummonAmount - 1 : tag.SummonAmount;
                        var rad = 2f / actuallyAmount; // == 360 / actuallyAmount / 180f
                        var directionEven = Vector2.up / 2; //offset (0.5f, 0.5f) vector up of circle
                        UpdateDirection(rad, ref directionEven);
                        var directionOdd = Vector2.up / 2;

                        for (int i = 1; i <= actuallyAmount; i++)
                        {
                            if (i % 2 == 0)
                            {
                                //Debug.Log(i + ": " + position + directionEven);
                                Summon(tag.SummonCreature, position + directionEven, args);
                                UpdateDirection(rad, ref directionEven);
                            }
                            else
                            {
                                //Debug.Log(i + ": " + position + directionOdd);
                                Summon(tag.SummonCreature, position + directionOdd, args);
                                UpdateDirection(2f - rad, ref directionOdd);
                            }
                        }

                        break;
                    }
                case SpecialTag.Teleport:
                    {
                        switch (tag.TPlace)
                        {
                            case TeleportPlace.ObstaclesStop: //optimize after
                                {
                                    var direction = GetSkillDirection(args.Caster.GetComponent<Animator>()) * tag.Distance;
                                    var position = args.Caster.localPosition + (Vector3)direction;
                                    args.Caster.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                                    args.Caster.GetComponent<Rigidbody2D>().MovePosition(position);
                                    StartCoroutine(Wait(0.25f, () => { args.Caster.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Discrete; }));

                                    break;
                                }
                            case TeleportPlace.ThroughAll: //optimize after
                                {
                                    var direction = GetSkillDirection(args.Caster.GetComponent<Animator>()) * tag.Distance;
                                    var position = args.Caster.localPosition + (Vector3)direction;
                                    args.Caster.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Discrete;
                                    args.Caster.GetComponent<Rigidbody2D>().MovePosition(position);
                                    StartCoroutine(Wait(0.25f, () => { args.Caster.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous; }));

                                    break;
                                }
                            case TeleportPlace.Point:
                                {
                                    break;
                                }
                        }

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

        private void UpdateDirection(float rad, ref Vector2 direction) //local func for update bullet direction by radian
        {
            var x = Mathf.Cos(rad * (float)Math.PI) * direction.x - Mathf.Sin(rad * (float)Math.PI) * direction.y;
            var y = Mathf.Sin(rad * (float)Math.PI) * direction.x + Mathf.Cos(rad * (float)Math.PI) * direction.y;
            direction.x = x;
            direction.y = y;
        }

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