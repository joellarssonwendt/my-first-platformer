using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPrompt : MonoBehaviour
{
    [SerializeField] GameObject player;

    void Start()
    {
        Hide();
    }

    void Update()
    {
        if (player.GetComponent<PlayerController>().isAttacking)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && player.GetComponent<PlayerController>().hasSword)
        {
            Show();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Hide();
        }
    }

    private void Show()
    {
        GetComponent<Renderer>().enabled = true;
    }

    private void Hide()
    {
        GetComponent<Renderer>().enabled = false;
    }
}
