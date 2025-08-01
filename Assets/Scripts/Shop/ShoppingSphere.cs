using System;
using Cinemachine;
using KartGame.KartSystems;
using UnityEngine;

namespace Shop
{
    public class ShoppingSphere : MonoBehaviour
    {
        public static ShoppingSphere Instance => FindFirstObjectByType<ShoppingSphere>();
        public Transform playerPosition;
        public Transform startingPosition;
        private ArcadeKart _player;
        private Vector3 _previousVelocity;
        private CinemachineVirtualCamera _vcam;

        private void Start()
        {
            _vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        }

        public void Activate(GameObject player)
        {
            _player = player.GetComponent<ArcadeKart>();
            Vector3 delta = startingPosition.position - _player.transform.position;
            player.transform.position = playerPosition.position;
            RenderSettings.fog = true;
            var playerRigidbody = player.GetComponent<Rigidbody>();
            _previousVelocity = playerRigidbody.linearVelocity;
            playerRigidbody.linearVelocity = Vector3.zero;
            _player.SetCanOnlyMoveSideways(true);
            FlushCamera(delta);
            Invoke(nameof(Exit), 10);
        }

        public void Exit()
        {
            Vector3 delta = startingPosition.position - _player.transform.position;
            _player.transform.position = startingPosition.position;
            _player.SetCanOnlyMoveSideways(false);
            _player.GetComponent<Rigidbody>().linearVelocity = _previousVelocity;
            FlushCamera(delta);
            RenderSettings.fog = false;
        }

        private void FlushCamera(Vector3 delta)
        {
            _vcam.OnTargetObjectWarped(_player.transform, delta);
            _vcam.PreviousStateIsValid = false;
        }
    }
}
