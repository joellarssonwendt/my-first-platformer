using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonBehaviour : MonoBehaviour
{
    // Cache
    private Animator anim;
    private Rigidbody2D rb2d;
    private AudioSource audioSource;
    private CapsuleCollider2D capsuleCollider;
    private BoxCollider2D boxCollider;
    private EnemyScript enemyScript;

    // Variables
    [SerializeField] private AudioClip deathSound, riseSound;
    [SerializeField] private int damage = 30;
    [SerializeField] private float knockBack = 250.0f;

    // Methods
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        enemyScript = GetComponent<EnemyScript>();
    }

    private void FixedUpdate()
    {
        if (enemyScript.enemyCanMove)
        {
            rb2d.velocity = new Vector2(gameObject.GetComponent<EnemyScript>().moveSpeed * Time.deltaTime, rb2d.velocity.y);
            anim.SetFloat("xVelocity", Mathf.Abs(rb2d.velocity.x));
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("EnemyBlock") || other.gameObject.CompareTag("Enemy"))
        {
            GetComponent<EnemyScript>().ChangeDirection();
        }

        if (other.gameObject.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            if (player.GetComponent<PlayerController>().iFrames) return;
            player.GetComponent<PlayerHealth>().TakeDamage(damage);
            player.GetComponent<PlayerController>().GetKnockedBack(transform, knockBack);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PlayerController>().knockedBack == false)
        {
            enemyScript.enemyCanMove = false;
            anim.Play("Skeleton_Death");
            audioSource.PlayOneShot(deathSound, 1.0f);

            other.GetComponent<Rigidbody2D>().velocity = new Vector2(other.GetComponent<Rigidbody2D>().velocity.x, 0);
            other.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, other.GetComponent<PlayerController>().GetJumpForce() * 0.75f));

            rb2d.bodyType = RigidbodyType2D.Static;
            capsuleCollider.enabled = false;
            boxCollider.enabled = false;
            Invoke("Rise", 10.0f);
        }
    }

    private void Rise()
    {
        anim.Play("Skeleton_Rise");
        AudioSource.PlayClipAtPoint(riseSound, transform.position, 3.0f);
        capsuleCollider.enabled = true;
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        Invoke("CanMoveAgain", 0.5f);
    }

    private void CanMoveAgain()
    {
        enemyScript.enemyCanMove = true;
        boxCollider.enabled = true;
    }
}
