using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeapomSystem), typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour {
    [System.Serializable]
    public class CheckGround
    {
        public LayerMask layerGround;
        public Transform transA, transB;
    }

    public Transform isWallLeft, isWallRight, positionRaycast;
    public Player player;
    public CheckGround checkGround; 
    public LayerMask layerEnemie;
    public LayerMask layerWall;

    public static PlayerController instance;

    private WeapomSystem weapomSystem;
    private bool colisionGround;

    [HideInInspector]
    public Vector3 localDownPosition;
    private void Awake()
    {
        instance = this;
    }
    void Start () {
        player.SetPlayer(gameObject);
        weapomSystem = GetComponent<WeapomSystem>();
        RepositionIsGroundPosition();
    }
    private void FixedUpdate()
    {
            player.CheckGround(checkGround.transA.position,
                checkGround.transB.position, checkGround.layerGround);
            player.CheckWall(isWallLeft.position, isWallRight.position, layerWall);
        
    }
    void Update () {
        if (!player.desactiveControls)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            bool attack = Input.GetKeyDown(KeyCode.C);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool dast = Input.GetKeyDown(KeyCode.X);


            player.InputControls(horizontal, jump, attack, dast);

        }

	}
    public void SetForce(float force)
    {
        player.SetForce(force);
    }
    public void GetDamage(float damage, Vector2 posEnemie)
    {
        player.GetDamage(damage, posEnemie);        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            if (!player.invunerable && !player.attackState)
            {
                switch (collision.gameObject.tag)
                {
                    case "Undead":
                        GetDamage(100f, collision.gameObject.transform.position);
                        break;
                    case "Slime":
                        GetDamage(100f, collision.gameObject.transform.position);
                        break;

                }
            }
        }
    }
  
    public void SetGravity()
    {
        player.SetGravity();
    }
   private void RepositionIsGroundPosition()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Vector2 size = collider.size;
        Vector3 centerPoint = new Vector3(collider.offset.x, collider.offset.y, 0f);

        float top = centerPoint.y + (size.y / 2f);
        float btm = centerPoint.y - (size.y / 2f);
        float left = centerPoint.x - (size.x / 2f);
        float right = centerPoint.x + (size.x / 2f);

        checkGround.transA.localPosition = new Vector3(left, btm, 0);
        checkGround.transB.localPosition = new Vector3(right, btm, 0);

        isWallLeft.localPosition = new Vector3(left, top, 0);
        isWallRight.localPosition = new Vector3(right, top, 0);
        positionRaycast.localPosition = new Vector3(0, btm+0.1f, 0);

        localDownPosition = new Vector3(0, btm, 0);

    }
}
