using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : Health , IKnockable<bool> , IDamageable<int>
{
    public int damage;
    public bool knockFromRight;
    public float knockPower;

    private new Rigidbody2D rigidbody2D;
    private new Transform transform;
    private void Awake()
    {
        transform = GetComponent<Transform>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Rolling();
    }
    /// <summary>
    /// rolling the ball as it move
    /// </summary>
    void Rolling()
    {
        // get the speed for
        float speed = rigidbody2D.velocity.magnitude;
        // check for which way is rolling
        float roll = rigidbody2D.velocity.x;
        if (roll<0)
        {
            // ileri yönde hýzýna göre döndür
            transform.Rotate(Vector3.forward * speed);
        }
        else if (roll>0)
        {
            transform.Rotate(Vector3.back * speed);
        }
    }
    /// <summary>
    /// dealing damage to touched colliders if they are IDamageable
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && collision.collider.GetComponent<Health>().invincible.Equals(false))
        {
            collision.collider.GetComponent<IDamageable<int>>().TakeDamage(damage);
            if (collision.collider.transform.position.x < transform.position.x)
            {
                knockFromRight = true;
                collision.collider.GetComponent<IKnockable<bool>>().KnockBack(knockFromRight);
            }
            else
            {
                knockFromRight = false;
                collision.collider.GetComponent<IKnockable<bool>>().KnockBack(knockFromRight);
            }
            
        }   
    }
    public override void KnockBack(bool right)
    {
        base.KnockBack(right);
        if (right)
        {
            rigidbody2D.AddForce(new Vector2(-knockPower, knockPower));
        }
        else
        {
            rigidbody2D.AddForce(new Vector2(knockPower, knockPower));
        }
    }

}
