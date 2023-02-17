using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.DynamicObject
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private float timer;
        [SerializeField] private bool isSetup = false;
        [SerializeField] private NetworkObject owner;

        public void Setup(float time)
        {
            timer = time;
            owner = GetComponent<NetworkObject>();

            isSetup = true;
        }

        private void FixedUpdate()
        {
            if (!owner.IsOwner) return; // it's owner by server

            if (!isSetup) return;

            if (timer > 0)
            {
                timer -= Time.fixedDeltaTime;
            }
            else
            {
                owner.Despawn();
                Destroy(gameObject);
            }
        }
    }
}