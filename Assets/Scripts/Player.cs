using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Player : Character {

    [HideInInspector]
    public bool isGround;

    [HideInInspector]
    public bool attackState = false;

    [HideInInspector]
    public bool isBow = false;

    private bool isWall;
    private SpriteEffect spriteEffect;
    private const float minGravity = 0.1f;
    private PlayerController playerController;

    public Vector2 directionKnowBack;
    public float forceKnowBack;
    public float forceJump;

    public void SetPlayer(GameObject player)
    {
        this.animChar = player.GetComponent<Animator>();
        this.RgbChar = player.GetComponent<Rigidbody2D>();
        this.transform = player.transform;
        this.spriteEffect = player.GetComponent<SpriteEffect>();
        this.playerController = player.GetComponent<PlayerController>();
        this.scaleX = transform.localScale.x;
    }
    public void CheckComboSword(bool attack)
    {
        if (desactiveControls)
        {
            return;
        }
        if (attack && !isDasting())
        {
            var currentClip = animChar.GetCurrentAnimatorClipInfo(0);
            string currentClipName = currentClip[0].clip.name;
            if (currentClipName == "attack0" || currentClipName == "attackAir0")
            {
                animChar.SetInteger("ComboAttack", 1);
            }
            else if (currentClipName == "attack1" || currentClipName == "attackAir1")
            {
                animChar.SetInteger("ComboAttack", 2);

            }
            else if (currentClipName == "attack2" || currentClipName == "attackAir2")
            {
                return;
            }
            else
            {
                animChar.SetInteger("ComboAttack", 0);
                animChar.SetTrigger("Attack");
                if (!isGround)
                {

                    RgbChar.velocity = Vector2.zero;
                    RgbChar.gravityScale = minGravity;
                }
            }
        }

    }
    public void CheckComboBow(bool attack)
    {
        bool isDast = isDasting();

        if (desactiveControls)
        {
            return;
        }
        if (attack && !isDast)
        {
            if (isBow == false)
            {

                animChar.SetTrigger("BowTrigger");
                RgbChar.velocity = Vector2.zero;

            }


            if (isBow && !isGround && RgbChar.velocity.y < 0) RgbChar.gravityScale = minGravity;
            else RgbChar.gravityScale = 2;

        }
        isBow = attack && !isDast;
        animChar.SetBool("Bow", isBow);

    }
    public void InputControls(float horizontal, bool jump, bool attack, bool dast)
    {

        bool isAttack = isAttacking();
        bool isDast = isDasting();
        int animState = 0;
        if (isAttack || isDast || isBow) horizontal = 0;

        if (!isGround && RgbChar.velocity.y <= 0 && (isAttack || isBow) && !isDast) RgbChar.gravityScale = minGravity;
        else if (RgbChar.gravityScale == minGravity && !isAttack && !isDast && !isBow) RgbChar.gravityScale = 2;

        FlipCharacter(horizontal);
        if (isDast)
        {
            horizontal = 0;
            animState = 0;
            attackState = false;
            isBow = false;
        }
        else
        {
            SetVelocity(horizontal);
        }
        if (RgbChar.velocity.x != 0)
        {
            animState = 1;
            attackState = false;
            isBow = false;

        }
        else if (RgbChar.velocity.x == 0)
        {
            animState = 0;
        }
        if (jump)
        {
            attackState = false;
            //isBow = false;
            Jump();
        }
        animChar.SetInteger("AnimState", animState);
        animChar.SetBool("AttackState", attackState);
        animChar.SetBool("Bow", isBow);
        if (spriteEffect.controllerDast == 0)
        {
            if ((isWall || isGround) && !isDast)
            {
                spriteEffect.controllerDast = 1;
            }

        }
        Dast(dast, horizontal);
    }
    public bool isHurt()
    {
        var currentClip = animChar.GetCurrentAnimatorClipInfo(0);

        return currentClip[0].clip.name == "hurt";
    }
    private bool isAttacking()
    {
        var currentClip = animChar.GetCurrentAnimatorClipInfo(0);
        string currentClipName = currentClip[0].clip.name;

        return currentClipName == "attack0" || currentClipName == "attack1"
            || currentClipName == "attack2" || currentClipName == "attackAir0" || currentClipName == "attackAir1"
            || currentClipName == "attackAir2";
    }
    public void CheckGround(Vector2 posA, Vector2 posB, LayerMask layerGround)
    {
        isGround = (Physics2D.Raycast(posA, Vector2.down, 0.05f, layerGround).collider != null ||
            Physics2D.Raycast(posB, Vector2.down, 0.05f, layerGround).collider != null) && RgbChar.velocity.y == 0;
        //isGround = Physics2D.Raycast(posA, posB.normalized, Vector2.Distance(posA,posB), layerGround).collider != null;
        //Debug.DrawRay(posA, posB.normalized, Color.red, 0.01f);
        Debug.Log(RgbChar.velocity);
        animChar.SetBool("isGround", isGround);
        animChar.SetFloat("speedY", RgbChar.velocity.y);
    }
    public void CheckWall(Vector2 posWallLeft, Vector2 posWallRight, LayerMask layerWall)
    {
        
        isWall = RgbChar.velocity.y < 0 && !isGround && (Physics2D.Raycast(posWallRight, Vector2.down, 1f, layerWall).collider != null ||
            Physics2D.Raycast(posWallLeft, Vector2.down, 1f, layerWall).collider != null);

        if (isWall) spriteEffect.CancelEffect();
        animChar.SetBool("isWall", isWall);
    }
    private void Jump()
    {
        if (isGround && !isAttacking())
        {
            RgbChar.AddForce(new Vector2(0, forceJump));
        }
        else if (!isGround && isWall)
        {
            if (transform.localScale.x < 0)
            {
                RgbChar.velocity = Vector2.zero;
                RgbChar.AddForce(new Vector2(500, forceJump));
            }
            else
            {
                RgbChar.velocity = Vector2.zero;
                RgbChar.AddForce(new Vector2(-500, forceJump));
            }
        }

    }
    void Dast(bool inputDast, float horizontal)
    {
        if (inputDast && !isWall)
        {
            Vector2 direction = transform.localScale.x > 0 ? transform.right.normalized : -transform.right.normalized;
            spriteEffect.StartEffect(direction);

        }
    }
    public void GetDamage(float damage, Vector2 posEnemie)
    {
        base.GetDamage(damage);
        spriteEffect.CancelEffect();
        spriteEffect.StartInvunerable();
        playerController.StartCoroutine(TimeDesactiveControlls(0.4f));
        RgbChar.velocity = Vector2.zero;
        animChar.SetInteger("AnimState", 0);
        if (posEnemie.x >= transform.position.x)
        {
            RgbChar.AddForce(new Vector2(-directionKnowBack.x, directionKnowBack.y) * forceKnowBack);
        }
        else
        {
            RgbChar.AddForce(new Vector2(directionKnowBack.x, directionKnowBack.y) * forceKnowBack);

        }
    }
    private bool isDasting()
    {
        return spriteEffect.dastIsPlaynig;
    }
    public void SetForce(float force)
    {
        if (transform.localScale.x < 0)
        {
            RgbChar.AddForce(new Vector2(force * -1, 0));
        }
        else
        {
            RgbChar.AddForce(new Vector2(force, 0));
        }
    }
    public void SetGravity()
    {
        RgbChar.gravityScale = 2;
    }
    IEnumerator TimeDesactiveControlls(float time)
    {
        desactiveControls = true;
        yield return new WaitForSeconds(time);
        desactiveControls = false;
    }
}
