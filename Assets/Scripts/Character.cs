using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character {
    public enum Direction { right, left, stay }

    [HideInInspector]
    public bool isDeath;

    [HideInInspector]
    public bool desactiveControls;

    [HideInInspector]
    public bool invunerable;

    [HideInInspector]
    public Rigidbody2D RgbChar;

    [HideInInspector]
    public Animator animChar;

    [HideInInspector]
    public Transform transform;

    public float life;
    public float velocityX;
    public Direction beginDirection;

    protected float scaleX;
   
    public void FlipCharacter(float horizontal)
    {
        if (beginDirection == Direction.right)
        {
            if (horizontal > 0 && transform.localScale.x != scaleX )
            {
                transform.localScale = new Vector3(scaleX,
                    transform.localScale.y, transform.localScale.z);
            }else if (horizontal<0 && transform.localScale.x != scaleX * -1)
            {
                transform.localScale = new Vector3(scaleX*-1,
                    transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            if (horizontal > 0 && transform.localScale.x != scaleX * -1)
            {
                transform.localScale = new Vector3(scaleX*-1,
                    transform.localScale.y, transform.localScale.z);
            }
            else if (horizontal < 0 && transform.localScale.x != scaleX)
            {
                transform.localScale = new Vector3(scaleX ,
                    transform.localScale.y, transform.localScale.z);
            }
        }
    }
    public void FlipCharacter(Direction direction)
    {
        if (beginDirection == Direction.right)
        {
            if (direction == Direction.right && transform.localScale.x != scaleX)
            {
                transform.localScale = new Vector3(scaleX,
                    transform.localScale.y, transform.localScale.z);
            }
            else if (direction == Direction.left && transform.localScale.x != scaleX * -1)
            {
                transform.localScale = new Vector3(scaleX * -1,
                    transform.localScale.y, transform.localScale.z);
            }
        }
        else if(Direction.left == beginDirection)
        {
            if (direction == Direction.right && transform.localScale.x != scaleX * -1)
            {
                transform.localScale = new Vector3(scaleX * -1,
                    transform.localScale.y, transform.localScale.z);
            }
            else if (direction == Direction.left && transform.localScale.x != scaleX)
            {
                transform.localScale = new Vector3(scaleX,
                    transform.localScale.y, transform.localScale.z);
            }
        }
    }
    public void SetVelocity(float horizontal)
    {
        if (!desactiveControls)
        {
            if (horizontal != 0)
            {
                RgbChar.velocity = new Vector2(horizontal * velocityX, RgbChar.velocity.y);

            }
            else if (horizontal == 0)
            {
                RgbChar.velocity = new Vector2(0, RgbChar.velocity.y);
            }
        }
        
    }
    public void SetVelocity(Direction direction)
    {
        
            switch (direction)
            {
                case Direction.right:
                    RgbChar.velocity = new Vector2(velocityX, RgbChar.velocity.y);
                    break;
                case Direction.left:
                    RgbChar.velocity = new Vector2(velocityX * -1, RgbChar.velocity.y);
                    break;
                case Direction.stay:
                    RgbChar.velocity = new Vector2(0, RgbChar.velocity.y);
                    break;
            }
        
       
    }
    public virtual void GetDamage(float damage)
    {
        if (!invunerable)
        {
            life = life - damage;

            if (life <= 0)
            {
                animChar.SetTrigger("Death");
                isDeath = true;
            }
            else
            {
                animChar.SetTrigger("Hurt");
            }
        }
    }
    public void SetLife(float life)
    {
        this.life = life;
    }
    public float getLife()
    {
        return life;
    }
}
