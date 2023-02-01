using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.DynamicObject
{
    public class Bullet : NetworkBehaviour, IBulletInitial
    {
        [SerializeField] private Rigidbody2D rigid;
        [SerializeField] private float damage;
        [SerializeField] private Vector2 direction;
        [SerializeField] protected float speed;

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
            if (collision.gameObject is null) return;

            Debug.Log(collision.gameObject.name + " take " + damage + " damage!");
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