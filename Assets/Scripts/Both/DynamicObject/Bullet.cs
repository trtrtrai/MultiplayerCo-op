using Assets.Scripts.Both.Creature;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.DynamicObject
{
    public class Bullet : NetworkBehaviour, IBulletInitial
    {
        [SerializeField] protected Rigidbody2D rigid;
        [SerializeField] protected int damage;
        [SerializeField] protected Vector2 direction;
        [SerializeField] protected float speed;
        [SerializeField] protected float time;
        [SerializeField] protected bool isSetup = false;
        //float mass for knockback v.v.

        protected Creature.Creature owner; //inject

        private void Start()
        {
            rigid = GetComponent<Rigidbody2D>();

            if (rigid is null) Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            rigid.velocity = speed * Time.fixedDeltaTime * direction;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsOwner) return;
            if (!IsSpawned) return;
            if (collision.gameObject is null) return;

            var collisionTag = collision.gameObject.tag;
            if (collisionTag.Equals("Bullet")) return;
            var detectRs = GameController.Instance.CreatureTagDetect(owner.tag, collisionTag);
            if (detectRs is null) //wall,rock,...
            {
                BulletCollisioned();
                return;
            }
            else if (detectRs == true)
            {
                var dmgRs = GameController.Instance.Damage(collision.gameObject.GetComponent<ICreature>(), NetworkObject, damage);

                GameController.Instance.Log(owner, dmgRs);
                BulletCollisioned();
            }
        }

        private void BulletCollisioned()
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }

        public void InjectBulletInfo(int damage, Vector2 direction, float speed, ICreature caster, float duration)
        {
            this.damage = damage;
            this.direction = direction.normalized;
            this.speed = speed;
            owner = caster as Creature.Creature;
            time = duration;

            isSetup = true; // run timer duration

            StartCoroutine(Wait());
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time);

            BulletCollisioned();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            StopAllCoroutines();
        }
    }

    public interface IBulletInitial
    {
        void InjectBulletInfo(int damage, Vector2 direction, float speed, ICreature caster, float duration);
    }
}