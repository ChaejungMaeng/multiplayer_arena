using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Spec.Specialisation.SpecMulti
{
    public class Shoot : MonoBehaviourPunCallbacks
    {

        public PlayerManager playerManager;

        [SerializeField]
        public Transform cameraTransform;

        private LineRenderer lineRenderer;
        public Transform laserHit;

        bool isAiming;

        private void FixedUpdate()
        {
            // isAiming = playerManager.aiming;

            if (isAiming && Input.GetKey(KeyCode.Mouse0))
            {
                Debug.Log("Fire");

                if (Physics.Raycast(transform.position, cameraTransform.forward, 10))
                {
                    Debug.Log("Hit");
                }
            }
        }
    }
}
