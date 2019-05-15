using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeapomSystem : MonoBehaviour {

    public enum TypeWeapom {
        Magic, Bow, Sword
    }
    public static TypeWeapom currentWeapom;
    public LayerMask layerEnemie;
    [Space(10)]
    [Header("Sword")]
    public float damageSword;
    public float radius;
    [Range(0f, 1f)]
    public float slowAnimSpeed;
    public Transform postionHit;
    public GameObject hitEffetc;
    [Space(10)]
    [Header("Bow")]
    public float damageBow;
    public float destroySeconds;
    public float speed;
    public Transform beginPosition;
    public GameObject bowPrefab;
    private List<Transform> bows;
    private IEnumerator<Transform> enumBows;
    private PlayerController playerScript;
    private void Awake()
    {
        GameObject inventary = new GameObject("Invetary");
        bows = new List<Transform>(10);
        for(int cont = 0; cont<10; cont++)
        {
            bows.Add(Instantiate(bowPrefab.transform, inventary.transform));
            bows[cont].gameObject.SetActive(false);
            ProjectileController p = bows[cont].GetComponent<ProjectileController>();
            p.instance(damageBow, speed, destroySeconds, layerEnemie);
        }
        enumBows = bows.GetEnumerator();
        currentWeapom = TypeWeapom.Sword;
        playerScript = GetComponent<PlayerController>();
    }

    void Update () {
        bool mudeWeapom = Input.GetKeyDown(KeyCode.Z);
   
        if (mudeWeapom)
        {
            NextWeapom();
        }
        TypeAttack();
        
    }
    private void NextWeapom()
    {
        switch (currentWeapom)
        {
            case TypeWeapom.Bow:
                currentWeapom = TypeWeapom.Sword;
                break;
            case TypeWeapom.Sword:
                currentWeapom = TypeWeapom.Magic;
                break;

            case TypeWeapom.Magic:
                currentWeapom = TypeWeapom.Bow;
                break;
        }
    }
    public void TypeAttack()
    {

        switch (WeapomSystem.currentWeapom)
        {
            case WeapomSystem.TypeWeapom.Bow:
                playerScript.player.CheckComboBow(Input.GetKey(KeyCode.C));
                break;
            case WeapomSystem.TypeWeapom.Magic:

                break;
            case WeapomSystem.TypeWeapom.Sword:
                bool attack = Input.GetKeyDown(KeyCode.C);
                playerScript.player.CheckComboSword(attack);
                if (attack && playerScript.player.isGround)
                {
                    CancelInvoke();
                    playerScript.player.attackState = true;
                    Invoke("SetAttackState", 2f);
                }
                break;

        }

    }
    public void SetHitSword(float rotationHit)
    {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(postionHit.position, radius, layerEnemie);
        if (enemiesToDamage.Length > 0) StartCoroutine(slowAnim());
        Animator animPlayer = playerScript.player.animChar;
        bool isKnowBack = animPlayer.GetCurrentAnimatorClipInfo(0)[0].clip.name == "attackAir2" || animPlayer.GetCurrentAnimatorClipInfo(0)[0].clip.name == "attack2";
        foreach (Collider2D collider in enemiesToDamage)
        {
            EnemieController enemieController = collider.GetComponent<EnemieController>();
            enemieController.GetDamage(damageSword);
            if (isKnowBack)
            {
                StartCoroutine(enemieController.monster.GetKnowBack(3000f, transform.position));
            }
            SetHitEffet(collider.transform.position,rotationHit);
        }

    }
    public void BowInstantiate()
    {
        if(!enumBows.MoveNext()) enumBows.Reset();
        enumBows.Current.position = beginPosition.position;
        Vector2 direction = transform.localScale.x>0? Vector2.right : Vector2.left;
        enumBows.Current.GetComponent<ProjectileController>().ProjectileActive(direction);
    }
    public void SetAttackState()
    {
        playerScript.player.attackState = false;
    }
    IEnumerator slowAnim()
    {
        Animator animPlayer = playerScript.player.animChar;
        animPlayer.SetFloat("velocityAnim", slowAnimSpeed);
        //string currentNameAnim = animPlayer.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        //yield return new WaitUntil(() => !(animPlayer.GetCurrentAnimatorClipInfo(0)[0].clip.name == currentNameAnim));
        yield return new WaitForSeconds(0.35f);
        animPlayer.SetFloat("velocityAnim", 1);
       
    }
    private void SetHitEffet(Vector3 positionEffect, float rotationHit)
    {
        GameObject objeto; 
        if (transform.localScale.x> 0)
        {
            objeto = Instantiate(hitEffetc, positionEffect, Quaternion.Euler(hitEffetc.transform.localScale.x, -180, rotationHit));
        }
        else
        {
            objeto = Instantiate(hitEffetc, positionEffect, Quaternion.Euler(hitEffetc.transform.localScale.x, 0, rotationHit));
        }
        Destroy(objeto, 0.4f);
    }
}
