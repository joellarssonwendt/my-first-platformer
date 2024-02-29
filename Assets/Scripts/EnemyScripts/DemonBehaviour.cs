using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonBehaviour : MonoBehaviour
{
    public bool isChasing = false;
    public AudioClip demonChaseSound;

    [SerializeField] private Transform startingPoint;
    [SerializeField] private BoxCollider2D triggerArea;
    [SerializeField] private AudioClip[] demonAttackSounds;
    [SerializeField] private int damage = 25;
    [SerializeField] private float knockBack = 250.0f;

    private Animator anim;
    private float speed, originalSpeed;
    private GameObject player;
    private EnemyScript enemyScript;
    private bool soundReady = true;
    private float soundCooldown = 3.0f;

    void Start()
    {
        enemyScript = GetComponent<EnemyScript>();
        anim = GetComponent<Animator>();
        originalSpeed = GetComponent<EnemyScript>().moveSpeed;
        speed = originalSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            ResetPosition();
        }
        FacePlayer();
    }

    private void ChasePlayer()
    {
        if (enemyScript.enemyCanMove == false) return;

        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        if(Vector2.Distance(transform.position, player.transform.position) < 3.0f)
        {
            speed = 5;
            anim.speed = 3;

            if (soundReady)
            {
                soundReady = false;
                PlayerAudio.PlaySound(demonAttackSounds, 0.75f);
                Invoke("SoundCooldown", soundCooldown);
            }

            Invoke("ResetSpeed", 0.5f);
        }
    }

    private void FacePlayer()
    {
        if (transform.position.x < player.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void ResetPosition()
    {
        if (enemyScript.enemyCanMove == false) return;
        transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, speed * Time.deltaTime);
    }

    private void ResetSpeed()
    {
        speed = originalSpeed;
        anim.speed = 1;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            player.GetComponent<PlayerHealth>().TakeDamage(damage);
            player.GetComponent<PlayerController>().GetKnockedBack(transform, knockBack);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<PlayerController>().knockedBack == false)
        {
            enemyScript.enemyCanMove = false;

            other.GetComponent<Rigidbody2D>().velocity = new Vector2(other.GetComponent<Rigidbody2D>().velocity.x, 0);
            other.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, other.GetComponent<PlayerController>().GetJumpForce() * 0.75f));

            enemyScript.TakeDamage(enemyScript.maxHealth);
            
        }
    }

    private void CanMoveAgain()
    {
        enemyScript.enemyCanMove = true;
    }

    private void SoundCooldown()
    {
        soundReady = true;
    }
}
