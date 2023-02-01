using Assets.Scripts.Both.Creature.Attackable;
using Assets.Scripts.Both.DynamicObject;
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

        public void Cast(List<SkillTag> tags)
        {
            tags.ForEach(t => { 
                switch (t.Tag)
                {
                    case TagType.Attack:
                        {
                            AttackTypeBehaviour(t);
                            
                            return;
                        }
                }
            });
        }

        private void AttackTypeBehaviour(SkillTag tag)
        {
            if (tag.Tag != TagType.Attack) return;

            switch (tag.Attack) // except AttackTag.Normal
            {
                case AttackTag.Bullet:
                    {
                        var bullet = Instantiate(Resources.Load<GameObject>("DynamicObject/Bullet/Bullet"));
                        IBulletInitial script = bullet.AddComponent<Bullet>();
                        script.InjectBulletInfo(100, Vector2.up, 175f);

                        bullet.SetActive(true);
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
    }
}