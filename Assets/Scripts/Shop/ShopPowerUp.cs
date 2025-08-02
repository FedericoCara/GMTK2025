using System;
using System.Collections.Generic;
using System.Linq;
using PowerUp;
using UnityEngine;

namespace Shop
{
    public class ShopPowerUp : MonoBehaviour
    {
        public List<PowerUpInShop> powerUpDisplay;
        public List<BasePowerUp> powerUpAvailable;
        public List<Transform> playerPositions;
        private Dictionary<BasePowerUp, int> _powerUpsGiven = new Dictionary<BasePowerUp, int>();
        private ObjectiveCompleteLaps _lapInfo;
        private bool _active;
        private int _lastHorizontal;
        private int _currentPlayerPosition;
        private Transform _player;

        private void Start()
        {
            _lapInfo = FindAnyObjectByType<ObjectiveCompleteLaps>();
        }

        private void Update()
        {
            if (_active)
            {
                var input = GetDiscreteHorizontalInput();
                if (input < 0 && _currentPlayerPosition>0)
                {
                    _currentPlayerPosition--;
                    Debug.Log($"Moving left to {_currentPlayerPosition}");
                    UpdatePlayerPosition();
                }else if (input > 0 && _currentPlayerPosition < playerPositions.Count - 1)
                {
                    _currentPlayerPosition++;
                    Debug.Log($"Moving right {_currentPlayerPosition}");
                    UpdatePlayerPosition();
                }
            }
        }

        public void Deactivate()
        {
            _active = false;
        }

        public void Activate(Transform player)
        {
            _active = true;
            _player = player;
            int currentLap = _lapInfo.currentLap;
            List<BasePowerUp> options = GetOptions(powerUpDisplay.Count);
            for (int i = 0; i < powerUpDisplay.Count; i++)
            {
                powerUpDisplay[i].SetPowerUp(options[i]);
            }

            _currentPlayerPosition = playerPositions.Count / 2;
            UpdatePlayerPosition();
        }

        private void UpdatePlayerPosition()
        {
            Debug.Log($"Moving player from {_player.position} to {playerPositions[_currentPlayerPosition].position}");
            _player.position = playerPositions[_currentPlayerPosition].position;
        }

        private List<BasePowerUp> GetOptions(int count)
        {
            // Inicializar diccionario si faltan powerUps
            foreach (var powerUp in powerUpAvailable)
            {
                if (!_powerUpsGiven.ContainsKey(powerUp))
                    _powerUpsGiven[powerUp] = 0;
            }

            List<BasePowerUp> options = new();
            System.Random rng = new System.Random();

            for (int i = 0; i < count; i++)
            {
                // Encontrar el mínimo de veces otorgado
                int minGiven = _powerUpsGiven.Values.Min();

                // Filtrar los powerUps con ese valor
                var leastGiven = powerUpAvailable
                    .Where(p => _powerUpsGiven[p] == minGiven)
                    .ToList();

                // Seleccionar aleatoriamente de los menos dados
                var chosen = leastGiven[rng.Next(leastGiven.Count)];

                options.Add(chosen);
                _powerUpsGiven[chosen]++; // Actualizamos el contador
            }

            return options;
        }

        private int GetDiscreteHorizontalInput()
        {
            return ( Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) ) ? -1
                : (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) ? 1 
                : 0;
        }
    }
}