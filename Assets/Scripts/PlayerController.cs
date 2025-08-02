using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private int initialHealth;
    [SerializeField] private int currentHealth;
    
    public int Health => currentHealth;

    public event Action OnPlayerDeath;
    public event Action<int> OnHealthChanged;

    public void Awake()
    {
        currentHealth = initialHealth;
    }

    public void Damage(int damage)
    {
        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            OnPlayerDeath?.Invoke();
        }
    }
}
