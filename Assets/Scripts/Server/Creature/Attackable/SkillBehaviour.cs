using Assets.Scripts.Both.Creature;
using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.DynamicObject;
using System.Collections.Generic;
using Unity.Netcode;
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

        public void Cast(List<SkillTag> tags, NetworkObject caster)
        {
            tags.ForEach(t => { 
                switch (t.Tag)
                {
                    case TagType.Attack:
                        {
                            AttackTypeBehaviour(t, caster);
                            
                            return;
                        }
                    case TagType.Effect:
                        {
                            return;
                        }
                    case TagType.Special:
                        {
                            SpecialTypeBehaviour(t, caster);

                            return;
                        }
                }
            });
        }

        private void AttackTypeBehaviour(SkillTag tag, NetworkObject caster)
        {
            if (tag.Tag != TagType.Attack) return;
            ICreature creature = caster.GetComponent<ICreature>();
            if (creature is null) return;

            switch (tag.Attack) // except AttackTag.Normal
            {
                case AttackTag.Bullet:
                    {
                        //if (!IsClient) return;
                        var bullet = Instantiate(Resources.Load<GameObject>("DynamicObject/Bullet/Bullet"));
                        bullet.transform.localPosition = (creature as NetworkBehaviour).transform.localPosition;
                        IBulletInitial script = bullet.GetComponent<Bullet>();
                        script.InjectBulletInfo(100, GetSkillDirection(caster.GetComponent<Animator>()), 175f);

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

        private void SpecialTypeBehaviour(SkillTag tag, NetworkObject caster)
        {
            if (tag.Tag != TagType.Special) return;
            ICreature creature = caster.GetComponent<ICreature>();
            if (creature is null) return;

            switch (tag.Special) // except AttackTag.Normal
            {
                case SpecialTag.Summon:
                    {
                        //if (!IsClient) return;
                        var critter = Instantiate(Resources.Load<GameObject>("OtherCreature/Bat/Bat"));
                        critter.transform.localPosition = (creature as NetworkBehaviour).transform.localPosition;
                        //IBulletInitial script = bullet.GetComponent<Bullet>();
                        //script.InjectBulletInfo(100, GetSkillDirection(caster.GetComponent<Animator>()), 175f);

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
    }
}