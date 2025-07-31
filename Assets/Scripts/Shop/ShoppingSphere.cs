using System;
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

        public void Activate(GameObject player)
        {
            _player = player.GetComponent<ArcadeKart>();
            player.transform.position = playerPosition.position;
            RenderSettings.fog = true;
            _player.SetCanMove(false);
            Invoke(nameof(Exit), 10);
        }

        public void Exit()
        {
            _player.SetCanMove(true);
            _player.transform.position = startingPosition.position;
            RenderSettings.fog = false;
        }
    }
}
