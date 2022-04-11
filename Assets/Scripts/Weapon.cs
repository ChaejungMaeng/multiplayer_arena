using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Spec.Specialisation.SpecMulti
{

    public class Weapon : MonoBehaviourPunCallbacks
    {

        #region Public Variables

        public Camera fpsCam;
        public GameObject player;

        public int damage = 10;
        public float range = 100f;

        public bool isFiring;

        #endregion

        #region Private Variables

        [Header("Specs")]
        [SerializeField]
        private bool addBulletSpread = true;
        [SerializeField]
        private Vector3 bulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
        [SerializeField]
        private ParticleSystem shootingSystem;
        [SerializeField]
        private Transform bulletSpawnPoint;
        [SerializeField]
        private ParticleSystem impactParticleSystem;
        [SerializeField]
        private TrailRenderer bulletTrail;
        [SerializeField]
        private float shootDelay = 0.25f;
        [SerializeField]
        private LayerMask mask;

        private float lastShootTime;

        PlayerManager playerManager;

        #endregion;

        // public bool isAiming;


        // Start is called before the first frame update
        void Start()
        {
            if (!photonView.IsMine) { return; }
            playerManager = player.GetComponent<PlayerManager>();
        }

        // Update is called once per frame
        void Update()
        {
            // isAiming = playerAbilities.aiming;

            if (!photonView.IsMine) { return; }

            if (photonView.IsMine)
            {
                if (Input.GetButton("Fire1"))
                {
                    Shoot();
                    isFiring = true;
                }
                else
                {
                    isFiring = false;
                }
            }

        }

        void Shoot()
        {
            if (lastShootTime + shootDelay < Time.time)
            {
                Vector3 direction = GetDirection();
                RaycastHit hit;

                Instantiate(shootingSystem, bulletSpawnPoint.position, Quaternion.LookRotation(fpsCam.transform.forward));

                lastShootTime = Time.time;

                // TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

                // StartCoroutine(SpawnTrail(trail, hit, hitPosition));

                if (Physics.Raycast(bulletSpawnPoint.position, direction, out hit, range))
                {


                    Vector3 hitPosition = GetDirection() * 1000;

                    TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);

                    StartCoroutine(SpawnTrail(trail, hit, hitPosition));
                    Instantiate(impactParticleSystem, hit.point, Quaternion.LookRotation(hit.normal));

                    // PlayerManager playerHit = hit.transform.GetComponent<PlayerManager>();

                    // Si l'objet est damageable (player), take damage) 
                    hit.collider.gameObject.GetComponentInParent<IDamageable>()?.TakeDamage(damage);
                    Debug.Log(hit.collider.gameObject);

                    // if (playerHit)
                    // {
                    //     playerHit.TakeDamage(damage);
                    //     Debug.Log("take damage");
                    // }
                }
            }
        }

        private Vector3 GetDirection()
        {
            Vector3 direction = fpsCam.transform.forward;

            if (addBulletSpread)
            {
                direction += new Vector3(
                Random.Range(-bulletSpreadVariance.x, bulletSpreadVariance.x),
                Random.Range(-bulletSpreadVariance.y, bulletSpreadVariance.y),
                Random.Range(-bulletSpreadVariance.z, bulletSpreadVariance.z));
                direction.Normalize();
            }
            return direction;
        }

        private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit, Vector3 HitPosition)
        {
            float time = 0;

            Vector3 startPosition = Trail.transform.position;

            while (time < 0.5f)
            {
                Trail.transform.position = Vector3.Lerp(startPosition, HitPosition, time * 2);
                time += Time.deltaTime;

                yield return null;
            }

            // Trail.transform.position = Hit.point;

            Destroy(Trail.gameObject, Trail.time);
        }


        // [PunRPC]
        // void SendRayCoordinates(RaycastHit hit, Vector3 HitPosition){

        // }
    }
}