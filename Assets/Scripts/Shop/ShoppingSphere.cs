using System;
using System.Collections;
using Cinemachine;
using KartGame.KartSystems;
using UnityEngine;
using UnityEngine.Serialization;
using Weapons;

namespace Shop
{
    public class ShoppingSphere : MonoBehaviour
    {
        public static ShoppingSphere Instance => FindFirstObjectByType<ShoppingSphere>();
        public ArcadeKart player;
        public Animator animator;
        public Transform playerPosition;
        public Transform resumePosition;
        public Transform cameraLookAt;
        public ShopPowerUp shopPowerUp;
        private Vector3 _previousVelocity;
        private CinemachineVirtualCamera _vcam;
        private Transform _previousLookAt;

        private void Start()
        {
            _vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            //Activate();
        }

        public void Activate()
        {
            var kartAnim = player.GetComponent<KartAnimation>();
            StartAnimation();
            RenderSettings.fog = true;
            Vector3 delta = SetupPlayer(player.gameObject, kartAnim);
            _previousLookAt = _vcam.LookAt;
            FlushCamera(delta, cameraLookAt, cameraLookAt);
            shopPowerUp.Activate(player.transform);
            Invoke(nameof(Exit), 10);
        }

        public void Exit()
        {
            shopPowerUp.Deactivate();
            var kartAnim = player.GetComponent<KartAnimation>();
            Vector3 delta = RestorePlayer(kartAnim);
            FlushCamera(delta, player.transform, _previousLookAt);
            RenderSettings.fog = false;
        }

        private void StartAnimation()
        {
            animator.enabled = true;
            animator.Rebind();
            animator.Update(0f);
        }

        private Vector3 SetupPlayer(GameObject playerGo, KartAnimation kartAnim)
        {
            Vector3 delta = resumePosition.position - this.player.transform.position;
            playerGo.transform.position = playerPosition.position;
            playerGo.transform.rotation = Quaternion.identity;
            var playerRigidbody = playerGo.GetComponent<Rigidbody>();
            _previousVelocity = playerRigidbody.linearVelocity;
            playerRigidbody.linearVelocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;
            this.player.SetCanMove(false);
            playerRigidbody.isKinematic = true;
            this.player.enabled = false;
            kartAnim.enabled = false;
            var autoShooter = player.GetComponentInChildren<AutoShooter>();
            if (autoShooter)
                autoShooter.enabled = false;
            return delta;
        }

        private Vector3 RestorePlayer(KartAnimation kartAnim)
        {
            Vector3 delta = resumePosition.position - player.transform.position;
            var playerRigidbody = player.GetComponent<Rigidbody>();
            player.transform.position = resumePosition.position;
            player.SetCanMove(true);
            player.GetComponent<Rigidbody>().linearVelocity = _previousVelocity;
            playerRigidbody.isKinematic = false;
            player.enabled = true;
            kartAnim.enabled = true;
            var autoShooter = player.GetComponentInChildren<AutoShooter>();
            if (autoShooter)
                autoShooter.enabled = true;
            return delta;
        }

        private void FlushCamera(Vector3 delta, Transform cameraFollow, Transform cameraLookAt)
        {
            _vcam.OnTargetObjectWarped(player.transform, delta);
            _vcam.PreviousStateIsValid = false;
            _vcam.transform.position = cameraLookAt.position;
            _vcam.Follow = cameraFollow;
            _vcam.LookAt = cameraLookAt;
        }
    }
}
