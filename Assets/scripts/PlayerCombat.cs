using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    /// <summary>
    /// Variables
    /// </summary>
    #region
    public Transform attackPoint;       //center of attack point from gameobject
    public LayerMask layerMask;     // attackable layers
    public bool isAttacking = false;    //checks if player attacking
    private bool knockFromRight;     //chechs if you hit other collider from right||

    [Header("Damage Values")]
    public float attackRange;       
    public int atkDamage = 40;
    public float atkSpeed = 2;
    #endregion
    /// <summary>
    /// gathers colliders deals damage and knock to them if colliders have interfaces to do
    /// </summary>
    public void Attack()
    {
        // colliderlarý algýlayacak olan alaný ayarla
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, layerMask);
        //hasar verilecek olan colliderlarý ayýkla
        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy==null)
            {
                return;
            }
            else if (enemy.GetComponent<Health>().invincible.Equals(false))
            {
                // vurulan hedefin Idamageable iterface i varsa hasar ver
                enemy.GetComponent<IDamageable<int>>().TakeDamage(atkDamage);
                // vurulan hedef ile vuran hedefin x posizyonlarýndan kimin saðda olup olmadýðýný öðren ve ona göre knockback interface i olanlarý varsa
                if (enemy.transform.position.x < transform.position.x)
                {
                    knockFromRight = true;
                    enemy.GetComponent<IKnockable<bool>>().KnockBack(knockFromRight);
                }
                else
                {
                    knockFromRight = false;
                    enemy.GetComponent<IKnockable<bool>>().KnockBack(knockFromRight);
                }
            }
            
        }
    }
    /// <summary>
    /// draws a gizmo on scene for qof
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
