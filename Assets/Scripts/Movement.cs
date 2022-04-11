using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

namespace Spec.Specialisation.SpecMulti
{
    public class Movement : MonoBehaviourPunCallbacks
    {

        float horizontal;
        float vertical;

        [SerializeField]
        float speed = 4.0f;

        [SerializeField]
        float speedSmoothTime = 0.1f;

        float currentSpeed;
        float speedSmoothVelocity;

        [SerializeField]
        float turnSmoothTime = 0.1f;
        float turnSmoothVelocity;


        [SerializeField]
        Transform cameraTransformParent;
        [SerializeField]
        Transform cameraTransform;
        [SerializeField]
        CharacterController controller;

        Vector2 input;
        Vector3 inputDirection;

        private Vector3 playerVelocity;
        float jumpHeight = 5.0f;
        private float gravityValue = 4.81f;

        private float _directionY;
        private float _jumpSpeed = 1.5f;

        bool moving = false;

        bool groundedPlayer;

        // Update is called once per frame
        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

            groundedPlayer = controller.isGrounded;

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 forward = cameraTransform.transform.forward;
            forward.y = 0;
            Vector3 move = cameraTransform.transform.right * x + forward * z;
            if (groundedPlayer && move.y < 0)
            {
                move.y = 0f;
            }
            if (move.sqrMagnitude > 1)
            {
                move.Normalize();
            }

            if (Input.GetButtonDown("Jump") && groundedPlayer)
            {
                _directionY = _jumpSpeed;
                Debug.Log("jump");
                move.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                gravityValue = 4.81f;
            }
            _directionY -= gravityValue * Time.deltaTime;
            move.y = _directionY;
            if (groundedPlayer)
            {
                gravityValue = 4.81f;
            }

            float targetRotation = cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);

            float targetSpeed = speed * inputDirection.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

            Vector3 moveDir = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;

            controller.Move(new Vector3(move.x, move.y, move.z) * speed * Time.deltaTime);

        }

    }
}
