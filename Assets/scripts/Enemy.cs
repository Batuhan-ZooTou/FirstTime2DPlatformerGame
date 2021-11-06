using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Health , IDamageable<int> , IKnockable<bool>
{
    private Animator animator;
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    public HealthBar healthBar;
    public GameObject health_bar;
    public float KnockRes;
    public bool hurt;
    public int power;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        hurt = true;
        power = damage;
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
        animator.SetTrigger("Hurt");

    }
    protected override void OnDeath()
    {
        base.OnDeath();
        animator.SetBool("Died", true);
        GetComponent<Collider2D>().enabled = false;
        rigidbody2D.isKinematic = true;
        spriteRenderer.sortingOrder = 0;
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
