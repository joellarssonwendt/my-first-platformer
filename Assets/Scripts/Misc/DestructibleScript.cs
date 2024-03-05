using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleScript : MonoBehaviour
{
    // Cache
    private SpriteRenderer rend;
    private Shader shaderGUItext;
    private Shader defaultSpriteShader;

    // Variables
    [SerializeField] private ParticleSystem parts1, parts2, parts3, parts4;
    [SerializeField] private AudioClip hitSound, deathSound;
    [SerializeField] private int health = 10;

    private Color originalColor;

    private void Start()
    {
        originalColor = GetComponent<SpriteRenderer>().color;
        rend = gameObject.GetComponent<SpriteRenderer>();
        shaderGUItext = Shader.Find("GUI/Text Shader");
        defaultSpriteShader = Shader.Find("Sprites/Default");
    }

    public void TakeDamage(int damage)
    {
        SpriteWhite();
        health -= damage;

        if (health <= 0)
        {
            PlayerAudio.PlaySound(deathSound, 1.0f);
            Instantiate(parts1, transform.position, Quaternion.identity);
            Instantiate(parts2, transform.position, Quaternion.identity);
            Instantiate(parts3, transform.position, Quaternion.identity);
            Instantiate(parts4, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            PlayerAudio.PlaySound(hitSound, 0.7f);
        }

        Invoke("SpriteNormal", 0.5f);
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
