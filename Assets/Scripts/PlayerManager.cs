using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Spec.Specialisation.SpecMulti
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable, IDamageable
    {

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(isFiringWeapon);
                stream.SendNext(health);
            }
            else
            {
                this.isFiringWeapon = (bool)stream.ReceiveNext();
                this.health = (float)stream.ReceiveNext();
            }
        }


        #endregion

        #region Public Fields

        public static GameObject LocalPlayerInstance;

        [Header("General")]
        [Tooltip("Set Character's Max Health")]
        [SerializeField]
        const float maxHealth = 100;

        [SerializeField]
        [Range(0, 100)]
        public float health = maxHealth;

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        public GameObject playerUiPrefab;

        public bool isFiringWeapon;
        Weapon _weapon;

        public Healthbar healthBar;

        #endregion

        #region Private Variables

        bool firing = false;

        #endregion

        #region Monobehaviour CallBacks

#if !UNITY_5_4_OR_NEWER
/// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
void OnLevelWasLoaded(int level)
{
    this.CalledOnLevelWasLoaded(level);
}
#endif

        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }


            // As a simple exercise, you can create a private method that would instantiate and send the "SetTarget" message, and from the 
            // various places, call that method instead of duplicating the code.

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

#if UNITY_5_4_OR_NEWER
        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }
#endif

        private void Awake()
        {

            if (!photonView.IsMine)
            {
                if (GetComponent<Movement>() != null)
                {
                    Destroy(GetComponent<Movement>());
                }
                if (GetComponentInChildren<Camera>() != null)
                {
                    Destroy(GetComponentInChildren<Camera>());
                }
            }


            if (photonView.IsMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            CameraControls _cameraControls = this.gameObject.GetComponentInChildren<CameraControls>();

            if (_cameraControls != null)
            {
                if (photonView.IsMine)
                {
                    _cameraControls.UpdateCameraControls();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
            }

            _weapon = this.gameObject.GetComponentInChildren<Weapon>();


            Cursor.lockState = CursorLockMode.Locked;
            // healthBar.SetMaxHealth(maxHealth);

            if (playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            }
            else
            {
                Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }


#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }



        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (photonView.IsMine)
            {
                isFiringWeapon = _weapon.isFiring;

                if (health <= 0f)
                {
                    GameManager.Instance.LeaveRoom();
                }

                if (Input.GetKeyDown(KeyCode.T))
                {
                    TakeDamage(20);
                }
            }
        }

        #endregion

        #region Private Methods

#if UNITY_5_4_OR_NEWER

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }


#endif
        #endregion


        #region Pun Callbacks

        public void TakeDamage(float damage)
        {
            photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage);


            if (!photonView.IsMine)
            {
                return;
            }
            health -= damage;
            healthBar.SetHealth(health);
        }

        [PunRPC]
        void RPC_TakeDamage(float damage)
        {
            if (!photonView.IsMine) { return; }

            health -= damage;

            if (health <= 0)
            {
                Debug.Log("dead");
            }
        }


        #endregion
    }
}