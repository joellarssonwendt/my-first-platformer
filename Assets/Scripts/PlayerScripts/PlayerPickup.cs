using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;

public class PlayerPickup : MonoBehaviour
{
    // Constants 
    const float defaultAlpha = 0.1f;

    // Variables
    public bool isRed, isBlue, isGreen;

    [SerializeField] GameObject redTilemap, greenTilemap, blueTilemap, playerTransFX;
    [SerializeField] Color redColor, greenColor, blueColor;

    private int smallHeartHealth = 15;
    private int goldCoins = 0;
    private bool hasKey = false;

    // Methods
    private void Start()
    {
        ChangeAlpha(redTilemap, defaultAlpha);
        ChangeAlpha(greenTilemap, defaultAlpha);
        ChangeAlpha(blueTilemap, defaultAlpha);
        UpdateColorInteractions();
    }

    // Item Pickup trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Win condition
        if (other.gameObject.CompareTag("GoalDoor"))
        {
            if (other.gameObject.GetComponent<WinCondition>().CheckIfLocked() && hasKey)
            {
                hasKey = false;
                other.gameObject.GetComponent<WinCondition>().UnlockDoor();
            }
        }

        if (other.gameObject.CompareTag("Key"))
        {
            other.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            other.gameObject.GetComponent<Animator>().Play("Key_Get");

            hasKey = true;

            GetComponent<PlayerAudio>().PickUpSounds();
            GetComponent<PlayerAudio>().KeyGetSound();
            StartCoroutine(DestroyObject(other.gameObject));
        }

        // Color Torch Transformations
        if (other.gameObject.CompareTag("RedColorTorch") && !isRed)
        {
            PlayerTransform("red");
            isRed = true;
            isGreen = false;
            isBlue = false;
            Invoke("TransformExit", 2.01f);
        }

        if (other.gameObject.CompareTag("GreenColorTorch") && !isGreen)
        {
            PlayerTransform("green");
            isRed = false;
            isGreen = true;
            isBlue = false;
            Invoke("TransformExit", 2.01f);
        }

        if (other.gameObject.CompareTag("BlueColorTorch") && !isBlue)
        {
            PlayerTransform("blue");
            isRed = false;
            isGreen = false;
            isBlue = true;
            Invoke("TransformExit", 2.01f);
        }

        // Items & Collectibles
        if (other.gameObject.CompareTag("SwordItem"))
        {
            other.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            other.gameObject.GetComponent<Animator>().Play("SwordItem_Get");

            gameObject.GetComponent<PlayerController>().hasSword = true;

            GetComponent<PlayerAudio>().SwordItemSound();
            StartCoroutine(DestroyObject(other.gameObject));
        }

        if (other.gameObject.CompareTag("SmallHeart"))
        {
            other.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            other.gameObject.GetComponent<Animator>().Play("GoldCoin_Get");

            GetComponent<PlayerHealth>().AddCurrentHealth(smallHeartHealth);

            GetComponent<PlayerAudio>().LifeOrbSound();
            StartCoroutine(DestroyObject(other.gameObject));
        }

        if (other.gameObject.CompareTag("GoldCoin"))
        {
            other.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            other.gameObject.GetComponent<Animator>().Play("GoldCoin_Get");

            goldCoins++;

            GetComponent<PlayerAudio>().PickUpSounds();
            StartCoroutine(DestroyObject(other.gameObject));
        }
    }

    // Helper Methods
    private void UpdateColorInteractions()
    {
        Physics2D.IgnoreLayerCollision(3, 9);
        Physics2D.IgnoreLayerCollision(3, 10);
        Physics2D.IgnoreLayerCollision(3, 11);
        Physics2D.IgnoreLayerCollision(7, 9);
        Physics2D.IgnoreLayerCollision(7, 10);
        Physics2D.IgnoreLayerCollision(7, 11);

        ChangeAlpha(redTilemap, defaultAlpha);
        ChangeAlpha(greenTilemap, defaultAlpha);
        ChangeAlpha(blueTilemap, defaultAlpha);

        if (isRed)
        {
            Physics2D.IgnoreLayerCollision(3, 9, false);
            Physics2D.IgnoreLayerCollision(7, 9, false);
            ChangeAlpha(redTilemap, 1.0f);
        }

        if (isGreen)
        {
            Physics2D.IgnoreLayerCollision(3, 10, false);
            Physics2D.IgnoreLayerCollision(7, 10, false);
            ChangeAlpha(greenTilemap, 1.0f);
        }

        if (isBlue)
        {
            Physics2D.IgnoreLayerCollision(3, 11, false);
            Physics2D.IgnoreLayerCollision(7, 11, false);
            ChangeAlpha(blueTilemap, 1.0f);
        }

    }

    public static void ChangeAlpha(GameObject obj, float alphaValue)
    {
        Material material = obj.GetComponent<Renderer>().material;
        Color oldColor = material.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaValue);
        material.SetColor("_Color", newColor);
    }

    private void PlayerTransform(string s)
    {
        GetComponent<PlayerController>().canMove = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        GetComponent<Animator>().Play("Player_Transform");
        GameObject fx = Instantiate(playerTransFX, transform.position, Quaternion.identity);

        if(s.Equals("red"))
        {
            fx.gameObject.GetComponent<SpriteRenderer>().color = redColor;
        }

        if (s.Equals("green"))
        {
            fx.gameObject.GetComponent<SpriteRenderer>().color = greenColor;
        }

        if (s.Equals("blue"))
        {
            fx.gameObject.GetComponent<SpriteRenderer>().color = blueColor;
        }

        GetComponent<PlayerAudio>().TransformSound();
    }

    private void TransformExit()
    {
        UpdateColorInteractions();
        if (isRed) gameObject.GetComponent<SpriteRenderer>().color = redColor;
        if (isGreen) gameObject.GetComponent<SpriteRenderer>().color = greenColor;
        if (isBlue) gameObject.GetComponent<SpriteRenderer>().color = blueColor;
        GetComponent<PlayerAudio>().TransformExitSound();
        Invoke("CanMoveAgain", 1.02f);
    }

    public void CanMoveAgain()
    {
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameObject.GetComponent<PlayerController>().canMove = true;
    }

    private IEnumerator DestroyObject(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        Destroy(obj);
    }
}
