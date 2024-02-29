using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;

public class PlayerController : MonoBehaviour
{
    // Cache
    public static PlayerController instance;
    public Animator anim;
    private Rigidbody2D rb2d;

    // Variables
    public bool hasSword = false;
    public bool iFrames = false;
    public bool canMove = true;
    public bool facingRight = true;
    public bool isAttacking = false;
    public bool knockedBack = false;
    public bool isAirborne = false;

    [SerializeField] private Transform leftFoot, rightFoot;
    [SerializeField] private LayerMask groundLayer, enemyLayer, hazardLayer, redLayer, greenLayer, blueLayer;
    [SerializeField] private GameObject attackPoint;

    private int attackPower = 10;
    private float moveSpeed = 250.0f;
    private float jumpForce = 500.0f;
    private float dodgeRollForce = 6f;
    private float dodgeRollDuration = 0.8f;
    private float dodgeRollCooldown = 0.3f;
    private float attackRadius = 0.6f;
    private float normalGravity;
    private float xVelocity;
    private bool isGrounded;
    private bool canDodgeRoll = true;
    private bool isDodgeRolling = false;

    // Methods
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        Physics2D.IgnoreLayerCollision(3, 9);
        Physics2D.IgnoreLayerCollision(3, 10);
        Physics2D.IgnoreLayerCollision(3, 11);

        normalGravity = rb2d.gravityScale;
    }

    void Update()
    {
        if (isDodgeRolling) return;

        if (canMove)
        {
            xVelocity = Input.GetAxis("Horizontal");

            if (Input.GetButtonDown("Jump") && CheckIfGrounded())
            {
                rb2d.gravityScale = normalGravity;
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
                rb2d.AddForce(new Vector2(0, jumpForce));
                GetComponent<PlayerAudio>().JumpSound();
            }
        }

        if (!canMove && !knockedBack && !isAirborne)
        {
            xVelocity = 0.0f;
        }

        StartAttack();
        StartDodgeRoll();

        anim.SetFloat("xVelocity", Mathf.Abs(rb2d.velocity.x));
        anim.SetFloat("yVelocity", rb2d.velocity.y);
        anim.SetBool("isGrounded", CheckIfGrounded());
    }

    private void FixedUpdate()
    {
        if (isDodgeRolling) return;

        if (!knockedBack && rb2d.bodyType == RigidbodyType2D.Dynamic)
        {
            rb2d.velocity = new Vector2(xVelocity * moveSpeed * Time.deltaTime, rb2d.velocity.y);

            if ((rb2d.velocity.x < 0 && facingRight) || (rb2d.velocity.x > 0 && !facingRight))
            {
                facingRight = !facingRight;
                transform.Rotate(new Vector3(0, 180, 0));
            }
        }
    }

    private bool CheckIfGrounded()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(leftFoot.position, Vector2.down, 0.15f, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightFoot.position, Vector2.down, 0.15f, groundLayer);

        //Debug.DrawRay(leftFoot.position, Vector2.down * 0.15f, Color.yellow, 0.15f);
        //Debug.DrawRay(rightFoot.position, Vector2.down * 0.15f, Color.yellow, 0.15f);

        if (leftHit.collider != null && leftHit.collider.CompareTag("Ground") || rightHit.collider != null && rightHit.collider.CompareTag("Ground"))
        {
            return true;
        }

        return (CheckColoredLeftFoot().collider != null && CheckColoredLeftFoot().collider.CompareTag("Ground") || CheckColoredRightFoot().collider != null && CheckColoredRightFoot().collider.CompareTag("Ground"));
    }

    private RaycastHit2D CheckColoredLeftFoot()
    {
        bool isRed = GetComponent<PlayerPickup>().isRed;
        bool isGreen = GetComponent<PlayerPickup>().isGreen;
        bool isBlue = GetComponent<PlayerPickup>().isBlue;

        if (isRed)
        {
            return Physics2D.Raycast(leftFoot.position, Vector2.down, 0.15f, redLayer);
        }

        if (isGreen)
        {
            return Physics2D.Raycast(leftFoot.position, Vector2.down, 0.15f, greenLayer);
        }

        if (isBlue)
        {
            return Physics2D.Raycast(leftFoot.position, Vector2.down, 0.15f, blueLayer);
        }

        return Physics2D.Raycast(leftFoot.position, Vector2.down, 0.15f, groundLayer);
    }

    private RaycastHit2D CheckColoredRightFoot()
    {
        bool isRed = GetComponent<PlayerPickup>().isRed;
        bool isGreen = GetComponent<PlayerPickup>().isGreen;
        bool isBlue = GetComponent<PlayerPickup>().isBlue;

        if (isRed)
        {
            return Physics2D.Raycast(rightFoot.position, Vector2.down, 0.15f, redLayer);
        }

        if (isGreen)
        {
            return Physics2D.Raycast(rightFoot.position, Vector2.down, 0.15f, greenLayer);
        }

        if (isBlue)
        {
            return Physics2D.Raycast(rightFoot.position, Vector2.down, 0.15f, blueLayer);
        }

        return Physics2D.Raycast(rightFoot.position, Vector2.down, 0.15f, groundLayer);
    }

    private void StartAttack()
    {
        if (Input.GetButtonDown("Fire1") && !isAttacking && hasSword)
        {
            isAttacking = true;
        }
    }

    private void StartDodgeRoll()
    {
        if (Input.GetButtonDown("Fire2") && canDodgeRoll && canMove && CheckIfGrounded())
        {
            StartCoroutine(DodgeRoll());
        }
    }

    private IEnumerator DodgeRoll()
    {
        canDodgeRoll = false;
        isDodgeRolling = true;
        canMove = false;
        anim.Play("Player_Roll");
        GetComponent<PlayerAudio>().DodgeSound();

        iFrames = true;
        Physics2D.IgnoreLayerCollision(3, 7);

        if (facingRight)
        {
            rb2d.velocity = new Vector2(transform.localScale.x * dodgeRollForce, rb2d.velocity.y);
        }
        else
        {
            rb2d.velocity = new Vector2(transform.localScale.x * -dodgeRollForce, rb2d.velocity.y);
        }
        
        yield return new WaitForSeconds(dodgeRollDuration * 0.6f);
        iFrames = false;
        Physics2D.IgnoreLayerCollision(3, 7, false);
        yield return new WaitForSeconds(dodgeRollDuration * 0.4f);
        isDodgeRolling = false;
        rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        yield return new WaitForSeconds(0.1f);
        canMove = true;
        yield return new WaitForSeconds(dodgeRollCooldown);
        canDodgeRoll = true;
    }

    public float GetJumpForce()
    {
        return jumpForce;
    }

    public void GetKnockedBack(Transform enemy, float force)
    {
        rb2d.gravityScale = normalGravity;
        canMove = false;
        knockedBack = true;
        rb2d.velocity = new Vector2(0, 0);

        if (transform.position.x > enemy.position.x)
        {
            rb2d.AddForce(new Vector2(force, force));
        }
        else
        {
            rb2d.AddForce(new Vector2(-force, force));
        }

        anim.Play("Player_Hurt");
        Invoke("CanMoveAgain", 0.3f);
    }

    public void CanMoveAgain()
    {
        canMove = true;
        knockedBack = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isAirborne = false;
        }
    }

    private void HitReg()
    {
        Collider2D[] attackArea = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, enemyLayer);
        Collider2D[] breakArea = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRadius, hazardLayer);

        for (int i = 0; i < attackArea.Length; i++)
        {
            if (attackArea[i].gameObject.CompareTag("Enemy"))
            {
                if (attackArea[i] != attackArea[i].gameObject.GetComponent<EnemyScript>().triggerCollider)
                {
                    attackArea[i].gameObject.GetComponent<EnemyScript>().TakeDamage(attackPower);
                }
            }
        }

        for (int i = 0; i < breakArea.Length; i++)
        {

            if (breakArea[i].gameObject.CompareTag("Destructible"))
            {
                breakArea[i].gameObject.GetComponent<DestructibleScript>().TakeDamage(attackPower);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRadius);

    }
}
