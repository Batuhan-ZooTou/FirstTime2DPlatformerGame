using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Direction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public float vertical;
    public Vector2 aimDirection;
    public Vector2 angle;
    public Vector2 RoL;
    /// <summary>
    /// calls methods 
    /// </summary>
    private void FixedUpdate()
    {
        DirectionOfAim();
        AngleOfAim();
        CheckForHorizontalAngle();
    }
    /// <summary>
    /// gets direction based on arrow keys
    /// </summary>
    public void DirectionOfAim()
    {
        vertical = playerMovement.verticalMove;
        
        if (playerMovement.horizontalMove > 0)
        {

            if (vertical > 0)
            {
                angle = new Vector2(1,1);
            }
            else if (vertical < 0)
            {
                angle = new Vector2(-1, 4);
            }
            else
            {
                angle = new Vector2(0,1);
            }
        }
        else if (playerMovement.horizontalMove < 0)
        {
            if (vertical > 0)
            {
                angle = new Vector2(1, -1);
            }
            else if (vertical < 0)
            {
                angle = new Vector2(-1, -4);
            }
            else
            {
                angle = new Vector2(0, -1);
            }
        }
        else if (vertical > 0)
        {
            angle = new Vector2(1, 0);
        }
        else if (vertical < 0)
        {
            angle = new Vector2(-1, 0);
        }
        else
        {
            angle = new Vector2(0, 0);
        }
    }
    /// <summary>
    /// gets angle of direction (tan)
    /// </summary>
    public void AngleOfAim()
    {
        
        var aimAngle = Mathf.Atan2(angle.x, angle.y);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }
        aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
    }
    public void CheckForHorizontalAngle()
    {
        if (aimDirection.x==1)
        {
            RoL = new Vector2(1, 0);
        }
        else if(aimDirection.x == -1)
        {
            RoL = new Vector2(-1, 0);
        }
        else
        {
            RoL = new Vector2(0, 0);
        }
    }

}
