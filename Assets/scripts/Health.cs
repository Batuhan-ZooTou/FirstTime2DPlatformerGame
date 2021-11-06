using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour ,IDamageable<int> ,IKnockable<bool>
{
    public int maxHealth;
    public int currentHealth;
    public bool invincible = false;
    public float invincibleTime;


    public virtual void TakeDamage(int damage)
    {
        if (!invincible)
        {
            invincible = true;
            currentHealth -= damage;
            Invoke("resetInvulnerability", invincibleTime);
        }
        Debug.Log("It was invincible");
    }
    public virtual void KnockBack(bool right)
    {
        Debug.Log("Knocked");
    }
    protected virtual void HitFeedBack()
    {
        Debug.Log("Get hurt");
    }
    protected bool Check›fDead()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            return true;
        }
        return false;
    }

    protected virtual void OnDeath()
    {
        // sil 
        Debug.Log("dead");
    }
    protected virtual void resetInvulnerability()
    {
        invincible = false;
    }









}
