using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace Spec.Specialisation.SpecMulti
{

    public class PlayerUI : MonoBehaviour
    {


        #region Public fields

        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        #endregion

        #region Private Fields

        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private Text playerNameText;

        [Tooltip("UI Slider to display Player's Health")]
        [SerializeField]
        private Slider playerHealthSlider;

        private PlayerManager target;

        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;
        Camera cam;


        #endregion


        #region MonoBehaviour Callbacks

        void Awake()
        {

            ///it's really not recommended to use "Find", because this is a slow operation. This is out of scope for this tutorial to 
            ///implement a more complex handling of such case, but a good exercise when you'll feel comfortable with Unity and scripting to find ways into 
            ///coding a better management of the reference of the Canvas element that takes loading and unloading into account.

            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

            _canvasGroup = this.GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = target.health;
            }
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }

        }

        void LateUpdate()
        {
            // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
            if (targetRenderer != null)
            {
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }


            // #Critical
            // Follow the Target GameObject on screen.
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = cam.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }


        #endregion


        #region Public Methods

        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }
            target = _target;

            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            cam = this.target.GetComponentInChildren<Camera>();

            Debug.Log(cam);

            // Get data from the Player that won't change during the lifetime of this Component
            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }

            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
        }

        #endregion
    }
}