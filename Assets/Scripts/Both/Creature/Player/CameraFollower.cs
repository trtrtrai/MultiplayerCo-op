using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Both.Creature.Player
{
    public class CameraFollower : NetworkBehaviour
    {
        public Transform Target; //player focus's transform
        public float SmoothSpeed = 5f; //speed of camera focus scale with delta time
        public Vector3 Offset; //offset from camera to player

        private bool start = false; //active focus

        private void LateUpdate()
        {
            if (!start) return;

            //Lerp from player pos to camera pos
            var cameraPos = Target.localPosition + Offset;
            var smoothPos = Vector3.Lerp(transform.localPosition, cameraPos, SmoothSpeed * Time.deltaTime);
            transform.localPosition = smoothPos;
        }

        /// <summary>
        /// Active focus
        /// </summary>
        public void StartFocus()
        {
            start = true;
        }
    }
}