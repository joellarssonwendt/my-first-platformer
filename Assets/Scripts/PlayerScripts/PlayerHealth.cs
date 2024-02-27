using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // Variables
    [SerializeField] private Slider healthBar;
    [SerializeField] private GameObject playerDead;

    private int maxHealth = 100;
    private int currentHealth = 100;

    // Methods
    void Start()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    void Update()
    {
        if (currentHealth == maxHealth && healthBar.gameObject.activeSelf == true)
        {
            healthBar.gameObject.SetActive(false);
        }

        if (currentHealth != maxHealth && healthBar.gameObject.activeSelf == false)
        {
            healthBar.gameObject.SetActive(true);
        }
    }

    public void AddMaxHealth(int n)
    {
        maxHealth += n;
        healthBar.maxValue = maxHealth;
    }

    public void SetMaxHealth(int n)
    {
        maxHealth = n;
        healthBar.maxValue = maxHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void AddCurrentHealth(int n)
    {
        currentHealth += n;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        healthBar.value = currentHealth;
    }

    public void SetCurrentHealth(int n)
    {
        currentHealth = n;
        healthBar.value = currentHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.value = currentHealth;
        GetComponent<PlayerAudio>().HurtSound();

        if (currentHealth <= 0)
        {
            PlayerDeath();
        }
    }

    public void PlayerDeath()
    {
        Instantiate(playerDead, transform.position, Quaternion.identity);
        this.gameObject.SetActive(false);
        Invoke("PlayerRespawn", 2.0f);
    }

    private void PlayerRespawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
