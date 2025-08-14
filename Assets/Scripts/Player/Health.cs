using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action OnUpdateHealth;
    public Action OnTakeDamage;
    
    public Action OnDeath;

    public bool IsGodMode;
    
    [SerializeField] private int maxHealth = 6;
    [SerializeField] private int health;

    private void Start()
    {
        ResetLife();
    }

    public void Heal(int amount)
    {
        health += amount;
        OnUpdateHealth?.Invoke();
    }
    
    public void TakeDamage(int damage)
    {
        if (!IsGodMode)
            health -= damage;

        if (health <= 0)
            health = 0;
        
        OnTakeDamage?.Invoke();

        if (health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    public void ResetLife()
    {
        Heal(maxHealth);
    }
    
    [ContextMenu("Kill Char")]
    public void KillChar()
    {
        TakeDamage(health);
    }
}