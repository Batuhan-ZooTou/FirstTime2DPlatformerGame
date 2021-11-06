using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private Animator animator;

    /// <summary>
    /// Get Component
    /// </summary>
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }
    /// <summary>
    /// Anim based on states
    /// </summary>
    #region
    public void PlayJumpingAnim()
    {
        animator.SetBool("Grounded", false);
    }
    public void PlayIdleAnim()
    {
        animator.SetFloat("moveSpeed", Mathf.Abs(playerMovement.horizontalMove));
        animator.SetBool("Grounded", true);
    }

    public void PlayRunningAnim()
    {
        animator.SetFloat("moveSpeed", Mathf.Abs(playerMovement.horizontalMove));
        animator.SetBool("Grounded", true);
    }

    public void StopRunningAnim()
    {
        animator.SetFloat("moveSpeed", Mathf.Abs(playerMovement.horizontalMove));
    }

    public void StopJumpingAnim()
    {
        animator.SetBool("Grounded", true);
    }

    public void TriggerAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }
    public void TriggerHurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }
    public void PlayDeathAnimation()
    {
        animator.SetBool("Died", true);
    }
    #endregion

}
