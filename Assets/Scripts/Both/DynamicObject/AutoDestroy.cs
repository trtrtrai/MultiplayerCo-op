using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.DynamicObject
{
    public class AutoDestroy : MonoBehaviour
    {
        [SerializeField] private float time;
        //[SerializeField] private bool isSetup = false;
        [SerializeField] private NetworkObject owner;

        public void Setup(float time)
        {
            this.time = time;
            owner = GetComponent<NetworkObject>();

            //isSetup = true;

            if (!owner.IsOwner) return; // it's owner by server
            StartCoroutine(Wait());
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time);

            owner.Despawn();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}