using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour {

    private Rigidbody2D rgbProjectile;
    private LayerMask collisionLayer;
    private float damage;
    private float speed;
    private float timeDesactive;
    public void instance (float damage, float speed, float timeDesactive, LayerMask collisionLayer)
    {
        this.collisionLayer = collisionLayer;
        this.damage = damage;
        this.speed = speed;
        this.timeDesactive = timeDesactive;
    }
	void Awake () {
        rgbProjectile = GetComponent<Rigidbody2D>();
	}
    public void ProjectileActive(Vector2 direction)
    {
        gameObject.SetActive(true);
        rgbProjectile.velocity = direction*speed;
        transform.localScale = direction.x>0 ? new Vector3(1, 1, 1) : new Vector3(1, -1, 1);
        StartCoroutine(DesactiveTime());
    }
    IEnumerator DesactiveTime()
    {
        yield return new WaitForSeconds(timeDesactive);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayer) != 0)
        {
            collision.gameObject.GetComponent<EnemieController>().GetDamage(damage);
            gameObject.SetActive(false);
        }else if (collision.gameObject.layer == 8)
        {
            gameObject.SetActive(false);
        }
    }
}
