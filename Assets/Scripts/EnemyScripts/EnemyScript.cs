using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    // Cache
    private Animator anim;
    private Rigidbody2D rb2d;
    private SpriteRenderer rend;
    private Shader shaderGUItext;
    private Shader defaultSpriteShader;

    // Variables
    public Collider2D triggerCollider;
    public float moveSpeed = 50.0f;
    public bool enemyCanMove = true;
    public int maxHealth;

    [SerializeField] private bool isInvulnerable = false;
    [SerializeField] private AudioClip enemyHitSound, enemyDeathSound;
    [SerializeField] GameObject player, enemyDeathFire, smallHeart;

    private int currentHealth;
    private Color originalColor;

    // Methods
    private void Start()
    {
        currentHealth = maxHealth;
        originalColor = GetComponent<SpriteRenderer>().color;

        rend = gameObject.GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        defaultSpriteShader = Shader.Find("Sprites/Default");
    }

    public void ChangeDirection()
    {
        transform.Rotate(new Vector3(0, 180, 0));
        moveSpeed = -moveSpeed;
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return;

        enemyCanMove = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        SpriteWhite();
        currentHealth -= damage;

        if(currentHealth <= 0)
        {
            PlayerAudio.PlaySound(enemyDeathSound, 1.5f);
            Instantiate(enemyDeathFire, transform.position, Quaternion.identity);

            if (player.GetComponent<PlayerHealth>().GetCurrentHealth() < player.GetComponent<PlayerHealth>().GetMaxHealth())
            {
                if (Random.value > 0.5f)
                {
                    Instantiate(smallHeart, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                }
            }

            Destroy(gameObject);
        }
        else
        {
            PlayerAudio.PlaySound(enemyHitSound, 1.0f);
        }

        Invoke("EnemyCanMoveAgain", 0.5f);
    }

    public void EnemyCanMoveAgain()
    {
        SpriteNormal();
        enemyCanMove = true;
    }
    private void SpriteWhite()
    {
        rend.material.shader = shaderGUItext;
        rend.color = Color.white;
    }

    private void SpriteNormal()
    {
        rend.material.shader = defaultSpriteShader;
        rend.color = Color.white;
    }
}
