using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int initialHealth;
    [SerializeField] private int currentHealth;

    public event Action OnPlayerDeath;
    public event Action OnHealthChanged;

    public void Start()
    {
        currentHealth = initialHealth;
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke();
        
        if (currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }
}
