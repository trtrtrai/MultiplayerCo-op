using Assets.Scripts.Both.Creature;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.DynamicObject
{
    public class Bullet : NetworkBehaviour, IBulletInitial
    {
        [SerializeField] protected Rigidbody2D rigid;
        [SerializeField] protected float damage;
        [SerializeField] protected Vector2 direction;
        [SerializeField] protected float speed;

        protected ICreature owner; //inject

        private void Start()
        {
            rigid = GetComponent<Rigidbody2D>();

            if (rigid is null) Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            rigid.velocity = direction * speed * Time.fixedDeltaTime;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!IsOwner) return;
            if (collision.gameObject is null) return;

            var collisionTag = collision.gameObject.tag;

            if (collisionTag.Equals("Mobs")) //always take damage
            {
                Debug.Log(collision.gameObject.name + " take " + damage + " damage!");
                NetworkObject.Despawn();
                Destroy(gameObject);
                return;
            }
            if (collisionTag.Equals(tag)) return; //same tag

            if (tag.Equals("Character") || tag.Equals("Ally"))
            {
                if (collisionTag.Equals("Boss") || collisionTag.Equals("Enemy") || collisionTag.Equals("Mobs"))
                {
                    Debug.Log(collision.gameObject.name + " take " + damage + " damage!");
                    NetworkObject.Despawn();
                    Destroy(gameObject);
                    return;
                }
            }

            if (tag.Equals("Boss") || tag.Equals("Enemy"))
            {
                if (collisionTag.Equals("Character") || collisionTag.Equals("Ally") || collisionTag.Equals("Mobs"))
                {
                    Debug.Log(collision.gameObject.name + " take " + damage + " damage!");
                    NetworkObject.Despawn();
                    Destroy(gameObject);
                    return;
                }
            }

            Debug.Log(collision.gameObject.name + " is not creature");
            NetworkObject.Despawn();
            Destroy(gameObject);
        }

        public void InjectBulletInfo(float damage, Vector2 direction, float speed)
        {
            this.damage = damage;
            this.direction = direction.normalized;
            this.speed = speed;
        }
    }

    public interface IBulletInitial
    {
        void InjectBulletInfo(float damage, Vector2 direction, float speed);
    }
}