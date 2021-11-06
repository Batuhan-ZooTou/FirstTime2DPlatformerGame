using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health , IDamageable<int> ,IKnockable<bool>
{
    private PlayerAnimation playerAnimation;
    private PlayerMovement playerMovement;
    private new Rigidbody2D rigidbody2D;
    public HealthBar healthBar;
    public GameObject health_bar;
    public float KnockRes;
    public bool hurt;
    public int power;       // power depends on damage you take
    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        power = damage;
        hurt = true;
        healthBar.SetHealth(currentHealth);
        DiedOrHurt();
    }
    public void DiedOrHurt()
    {
        if (Check›fDead())
        {
            OnDeath();
        }
        else if (hurt == true)
        {
            HitFeedBack();
        }
    }
    protected override void HitFeedBack()
    {
        base.HitFeedBack();
        hurt = false;
        playerMovement.SetMovementState(PlayerMovement.MovementStates.Hurt);
        playerAnimation.TriggerHurtAnimation();
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        playerMovement.SetMovementState(PlayerMovement.MovementStates.Death);
        playerAnimation.PlayDeathAnimation();
        GetComponent<Collider2D>().enabled = false;
        rigidbody2D.isKinematic = true;
        Destroy(health_bar);
        this.enabled = false;
    }

    public override void KnockBack(bool right)
    {
        base.KnockBack(right);
        if (right)
        {
            rigidbody2D.AddForce(new Vector2(-(power / KnockRes), power / KnockRes));
        }
        else
        {
            rigidbody2D.AddForce(new Vector2(power / KnockRes, power / KnockRes));
        }
    }

}
