using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileController : MonoBehaviour {

    private Rigidbody2D rgbProjectile;
    private LayerMask collisionLayer;
    private float damage;
    private float speed;
    public void instance (float damage, float speed, float timeDesactive, LayerMask collisionLayer)
    {
        this.collisionLayer = collisionLayer;
        this.damage = damage;
        this.speed = speed;
    }
	void Awake () {
        rgbProjectile = GetComponent<Rigidbody2D>();
	}
    public void ProjectileActive(Vector2 direction)
    {
        gameObject.SetActive(true);
        rgbProjectile.velocity = direction*speed;
        transform.localScale = direction.x>0 ? new Vector3(1, 1, 1) : new Vector3(1, -1, 1);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayer) != 0)
        {
            collision.gameObject.GetComponent<EnemieController>().GetDamage(damage);
            gameObject.SetActive(false);
        }
    }
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }
}
