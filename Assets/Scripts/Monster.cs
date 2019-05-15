using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Monster : Character {

    public enum TypeMonster { stalk , normal }
    private EnemieController enemieScript;
    [HideInInspector]
    public bool isIdleCoroutine;
    private Direction currentToWalkPosition;

    public Monster.TypeMonster typeMode;
    public Transform posRay;
    public LayerMask layerLimits;
    public LayerMask layerPlayer;
    public float distanceRay;
    public float delayAttak;
    [HideInInspector]
    public Transform playerTransform;

    [Range(0f, 3f)]
    public float distanceAttack;
    public Vector2 directionKnowBack;
    private float currentWallPositionXRight, currentWallPositionXLeft;
    private Coroutine idleCoroutine;
    private bool dontAttack;
    
    public void SetMonster(GameObject monster)
    {
        animChar = monster.GetComponent<Animator>();
        RgbChar = monster.GetComponent<Rigidbody2D>();
        transform = monster.transform;
        enemieScript = monster.GetComponent<EnemieController>();
        scaleX = transform.localScale.x;
        playerTransform = PlayerController.instance.transform;
    }
    public void StayMode(bool isWall)
    {
        if(isWall)
            switch (currentToWalkPosition)
            {
                case Monster.Direction.left:
                    StopIdleCoroutine();
                    idleCoroutine = enemieScript.StartCoroutine(DelayTimeIdle(2f, Direction.right));
                    break;
                case Monster.Direction.right:
                    StopIdleCoroutine();
                    idleCoroutine = enemieScript.StartCoroutine(DelayTimeIdle(2f, Direction.left));
                    break;
                default:
                    if (CurrentDirection() == Direction.right)
                    {
                        StopIdleCoroutine();
                        idleCoroutine = enemieScript.StartCoroutine(DelayTimeIdle(2f, Direction.right));
                    }
                    else
                    {
                        StopIdleCoroutine();
                        idleCoroutine = enemieScript.StartCoroutine(DelayTimeIdle(2f, Direction.left));
                    }
                    break;
            }      
    }
    public bool IsPlayer()
    {
        
        RaycastHit2D ray = Physics2D.Raycast(posRay.position, Vector2.right, 3f, layerPlayer);
        RaycastHit2D ray2 = Physics2D.Raycast(posRay.position, Vector2.left, 3f, layerPlayer);
        
        bool isPlayer = (ray.collider != null ||
            ray2.collider != null) && playerTransform.position.x > currentWallPositionXLeft && playerTransform.position.x < currentWallPositionXRight;

        return isPlayer;
    }
    public bool IsWall()
    {
       
        RaycastHit2D ray = Physics2D.Raycast(posRay.position, Vector2.right, 500, layerLimits);
        RaycastHit2D ray2 = Physics2D.Raycast(posRay.position, Vector2.left, 500, layerLimits);
        if (ray.transform)
        {
            currentWallPositionXRight = ray.point.x;
            currentWallPositionXLeft = ray2.point.x;
            switch (currentToWalkPosition)
            {
                case Direction.left:
                    return Mathf.Abs(currentWallPositionXLeft - posRay.position.x) <= distanceRay;
                    
                case Direction.right:
                    return Mathf.Abs(currentWallPositionXRight - posRay.position.x) <= distanceRay;

                default:
                        if (CurrentDirection() == Direction.right)
                        {
                            return Mathf.Abs(currentWallPositionXRight - posRay.position.x) <= distanceRay;
                        }                       
                        else
                        {
                            return Mathf.Abs(currentWallPositionXLeft - posRay.position.x) <= distanceRay;
                        }       
            }
            
        }
        return false;
    }
    public void StopIdleCoroutine()
    {
        if (idleCoroutine != null) enemieScript.StopCoroutine(idleCoroutine);
    }
    public void SetAnimDirection(Direction currentDirection)
    {
        if(currentDirection != Direction.stay)
        {
            animChar.SetBool("Walk", true);
        }
        else
        {
            animChar.SetBool("Walk", false);
        }
    }
    public Direction CurrentDirectionToWalk()
    {
        return currentToWalkPosition;
    }
    public void AttackingMode(bool isAttacking)
    {
        if (!isAttacking && !dontAttack)
        {
            if (Vector2.Distance(playerTransform.position, transform.position) <= distanceAttack )
            {
                    animChar.SetTrigger("Attack");
                    enemieScript.StartCoroutine(DelayTimeAttack(currentToWalkPosition));
            }
            else
            {

                if (typeMode == Monster.TypeMonster.stalk)
                {
                    if (playerTransform.position.x > transform.position.x)
                    {
                        currentToWalkPosition = Direction.right;
                    }
                    else
                    {
                        currentToWalkPosition = Direction.left;
                    }
                }
                else
                {
                    bool isWall = IsWall();

                    StayMode(isWall);
                }
            }
        }
    }
    private Direction CurrentDirection()
    {
        if (beginDirection == Direction.left)
        {
            if (transform.localScale.x > 0)
            {
                return Direction.left;
            }
            else
            {
                return Direction.right;
            }
        }
        else if (beginDirection == Direction.right)
        {
            if (transform.localScale.x > 0)
            {
                return Direction.right;
            }
            else
            {
                return Direction.left;
            }
        }
        return Direction.stay;
    }
    public void SetHit(Vector2 posHit, float damage, LayerMask layerPlayer)
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(posHit, 0.5f, layerPlayer);
        foreach (Collider2D collider in enemiesToDamage)
        {
            PlayerController playerController = collider.GetComponent<PlayerController>();
            if (playerController)
            {
                playerController.GetDamage(damage, transform.position);
            }
        }

    }  
    public override void GetDamage(float damage)
    {
        animChar.SetBool("Attack", false);                                   
        RgbChar.velocity = new Vector2(0, RgbChar.velocity.y);
        base.GetDamage(damage);
    }
    public IEnumerator GetKnowBack(float forceKnowBack, Vector3 posPlayer)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        RgbChar.velocity = new Vector2(0, RgbChar.velocity.y);
        if (posPlayer.x >= transform.position.x)
        {
            RgbChar.AddForce(new Vector2(-directionKnowBack.x, directionKnowBack.y) * forceKnowBack);
        }
        else
        {
            RgbChar.AddForce(new Vector2(directionKnowBack.x, directionKnowBack.y) * forceKnowBack);
        }
        yield return new WaitForSeconds(0.05f);
        RgbChar.velocity = new Vector2(0, RgbChar.velocity.y);
    }
    IEnumerator DelayTimeIdle(float time, Direction nextDirection)
    {
        isIdleCoroutine = true;
        
        currentToWalkPosition = Direction.stay;
        SetAnimDirection(currentToWalkPosition);
        yield return new WaitForSeconds(time);
        currentToWalkPosition = nextDirection;
        SetAnimDirection(currentToWalkPosition);
        isIdleCoroutine = false;
    }
    IEnumerator DelayTimeAttack(Direction currentDirection)
    {
        dontAttack = true;
        currentToWalkPosition = Direction.stay;
        yield return new WaitForSeconds(delayAttak);
        if (!(Vector2.Distance(playerTransform.position, transform.position) <= distanceAttack))
        {
            dontAttack = false;
            currentToWalkPosition = currentDirection;
        }
        else
        {
            animChar.SetTrigger("Attack");
            enemieScript.StartCoroutine(DelayTimeAttack(currentDirection));
        }
    }
}
