using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonTriggerArea : MonoBehaviour
{
    [SerializeField] private GameObject thisDemon;

    private bool soundReady = true;
    private float soundCooldown = 3.0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && thisDemon != null)
        {
            thisDemon.GetComponent<DemonBehaviour>().isChasing = true;
            if (soundReady)
            {
                soundReady = false;
                PlayerAudio.PlaySound(thisDemon.GetComponent<DemonBehaviour>().demonChaseSound, 1.0f);
                Invoke("SoundCooldown", soundCooldown);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && thisDemon != null)
        {
            thisDemon.GetComponent<DemonBehaviour>().isChasing = false;
        }
    }

    private void SoundCooldown()
    {
        soundReady = true;
    }
}
