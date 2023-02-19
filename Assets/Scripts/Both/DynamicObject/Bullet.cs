using Assets.Scripts.Both.Creature;
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
        [SerializeField] protected float timer;
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
            rigid.velocity = direction * speed * Time.fixedDeltaTime;

            if (!isSetup) return;

            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
            }
            else
            {
                BulletCollisioned();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsOwner) return;
            if (!IsSpawned) return;
            if (collision.gameObject is null) return;

            var collisionTag = collision.gameObject.tag;
            var detectRs = GameController.Instance.CreatureTagDetect(owner.tag, collisionTag);
            if (detectRs is null) //wall,rock,...
            {
                BulletCollisioned();
                return;
            }
            else if (detectRs == true)
            {
                GameController.Instance.Damage(collision.gameObject.GetComponent<ICreature>(), NetworkObject, damage);
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
            timer = duration;

            isSetup = true; // run timer duration
        }
    }

    public interface IBulletInitial
    {
        void InjectBulletInfo(int damage, Vector2 direction, float speed, ICreature caster, float duration);
    }
}