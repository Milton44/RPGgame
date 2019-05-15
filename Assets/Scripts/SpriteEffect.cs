using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteEffect : MonoBehaviour {

    [Space(10)]
    [Header("Ghost Player Effect")]
    [Range(0,1)]
    public float durationEffect;
    public float speedDast;
    public GameObject prefabSpriteEffect;
    public int quantSprites;
    public float timeEffect;
    public string currentClipName;

    private SpriteRenderer sprite;
    private Animator playerAnim;
    private List<SpriteRenderer> spriteRenders;
    private IEnumerator<SpriteRenderer> enumSprite;
    private float currentTimeEffect;
    private Vector2 direction;
    private Rigidbody2D rgbPlayer;
    private float currentGaravity;
    private PlayerController playerScript;
    [HideInInspector]
    public int controllerDast { set; get; }
    [HideInInspector]
    public bool dastIsPlaynig = false;

    [Space(10)]
    [Header("Shadow Player Effect")]
    public GameObject shadowPrefab;
    public Transform positionRaycast;
    public LayerMask layerCollision;
    private Transform shadow;
    public float distance;
    public bool debugRay;


    private float scaleX;

    [Space(10)]
    [Header("Invunerable Player Effect")]
    public float timeInvunerable;
    [Range(0, 1)]
    public float minOpacit;
    public int layerNoCollision;

    private Coroutine ghostEffect, invunerableEffect;
    void Awake () {
        GameObject paternPrefabs = new GameObject("SpriteEffectsPrefabs");
        spriteRenders = new List<SpriteRenderer>(quantSprites);

        for(int cont = 0; cont< quantSprites; cont++)
        {
            GameObject objeto = Instantiate(prefabSpriteEffect, paternPrefabs.transform);
            objeto.SetActive(false);
            spriteRenders.Add(objeto.GetComponent<SpriteRenderer>());
        }
        
        sprite = GetComponent<SpriteRenderer>();
        enumSprite = spriteRenders.GetEnumerator();
        enumSprite.MoveNext();
        playerAnim = GetComponent<Animator>();
        rgbPlayer = GetComponent<Rigidbody2D>();
        currentGaravity = rgbPlayer.gravityScale;

        shadow = Instantiate(shadowPrefab.transform);
        scaleX = shadow.localScale.x;
       playerScript = GetComponent<PlayerController>();
    }

	void Update () {
        GhostEffect();
        ShadowEffect();
	}
    public void StartEffect(Vector2 direction)
    {
        if (!dastIsPlaynig && controllerDast > 0)
        {
            ghostEffect = StartCoroutine(DurationEffect());
            this.direction = direction;
            controllerDast = 0;
        }
    }
    public void CancelEffect()
    {
        playerAnim.SetBool("Dast", false);
        rgbPlayer.gravityScale = currentGaravity;
        if(ghostEffect != null)
            StopCoroutine(ghostEffect);
        dastIsPlaynig = false;
        
    }
    IEnumerator DurationEffect()
    {
        dastIsPlaynig = true;
        playerAnim.SetBool("Dast", true);
        rgbPlayer.gravityScale = 0;
        rgbPlayer.velocity = rgbPlayer.velocity * Vector2.right;
        yield return new WaitForSeconds(durationEffect);
        playerAnim.SetBool("Dast", false);
        rgbPlayer.gravityScale = currentGaravity;
        dastIsPlaynig = false;
    }
    private void GhostEffect()
    {
        if (dastIsPlaynig)
        {
            if (currentTimeEffect <= 0)
            {
                SpriteRenderer currentRenderer = enumSprite.Current;
                if (!enumSprite.MoveNext()) enumSprite.Reset();
                currentRenderer.sprite = sprite.sprite;
                currentRenderer.gameObject.transform.position = transform.position;
                currentRenderer.gameObject.SetActive(false);
                currentRenderer.transform.localScale = transform.localScale;
                currentRenderer.gameObject.SetActive(true);
                currentTimeEffect = timeEffect;
            }
            else
            {
                currentTimeEffect -= Time.deltaTime;
            }
            Vector2 velocityX = direction * speedDast;
            rgbPlayer.velocity = new Vector2(velocityX.x, rgbPlayer.velocity.y);
            if (rgbPlayer.velocity.y != 0) playerAnim.SetBool("Dast", false);

        }
    }
    private void ShadowEffect()
    {
        RaycastHit2D[] rayAll = Physics2D.RaycastAll(positionRaycast.position, Vector2.down, distance, layerCollision);
       
        foreach (RaycastHit2D ray in rayAll)
        {
            if (transform.TransformPoint(PlayerController.instance.localDownPosition).y > ray.point.y)
            {
                if (debugRay) Debug.DrawRay(positionRaycast.position, Vector2.down, Color.red, 0.01f);
                if (ray.transform != null)
                {
                    shadow.gameObject.SetActive(true);
                    shadow.position = ray.point;
                    float currentScalex = scaleX / ray.distance;
                    if (currentScalex > scaleX) currentScalex = scaleX;
                    shadow.localScale = new Vector3(currentScalex, shadow.localScale.y, shadow.localScale.z);
                }
                else
                {
                    shadow.gameObject.SetActive(false);
                }
                return;
            }
        }
        
    }

    IEnumerator InvunerableEffect()
    {
        playerScript.player.invunerable = true;
        gameObject.layer = layerNoCollision;
        float timeIni = Time.time;
        while (true)
        {
            for(float opacit = minOpacit; opacit<1; opacit += 0.1f)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, opacit);
                yield return new WaitForEndOfFrame();
            }
            for (float opacit = 1; opacit > 0; opacit -= 0.1f)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, opacit);
                yield return new WaitForEndOfFrame();
            }
            if (Time.time - timeIni >= timeInvunerable) break;
        }
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
        gameObject.layer = 10;
        playerScript.player.invunerable = false;
    }
    public void StartInvunerable()
    {
        if (invunerableEffect != null) StopCoroutine(invunerableEffect);
        invunerableEffect = StartCoroutine(InvunerableEffect());
        
    }
}
