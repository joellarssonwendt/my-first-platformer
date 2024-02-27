using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesBehaviour : MonoBehaviour
{
    // Cache
    private BoxCollider2D boxCollider;
    private EnemyScript enemyScript;

    // Variables
    [SerializeField] private int damage = 25;
    [SerializeField] private float knockBack = 250.0f;

    // Methods
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        enemyScript = GetComponent<EnemyScript>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameObject player = other.gameObject;
            if (player.GetComponent<PlayerController>().iFrames) return;
            player.GetComponent<PlayerHealth>().TakeDamage(damage);
            player.GetComponent<PlayerController>().GetKnockedBack(transform, knockBack);
        }
    }
}
