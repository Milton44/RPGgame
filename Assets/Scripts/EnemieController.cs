using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemieController : MonoBehaviour {
   
    public Monster monster;
    public Transform transformHit;
    [Space(10)]
    [Header("DeadEffect")]
    [Range(0f,5f)]
    public float timeEffect;
    public SpriteRenderer sprite;
    private Coroutine deadEffectCoroutine;
    void Start () {
        monster.SetMonster(gameObject);
    }
    
    public void GetDamage(float damage)
    {   
        monster.GetDamage(damage);   
    }
    public void CheckAttack(float damage)
    {
        monster.SetHit(transformHit.position, damage, monster.layerPlayer);
    }
    public void DestroyObject()
    {
        Destroy(gameObject);
    }

}
