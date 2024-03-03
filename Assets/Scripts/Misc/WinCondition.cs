using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    [SerializeField] GameObject padlock, player, fader;

    private Animator doorAnim, padlockAnim;
    private Rigidbody2D padlockBody;

    private bool isLocked;

    void Start()
    {
        doorAnim = GetComponent<Animator>();
        padlockAnim = padlock.GetComponent<Animator>();
        padlockBody = padlock.GetComponent<Rigidbody2D>();

        padlockBody.bodyType = RigidbodyType2D.Static;

        isLocked = true;
    }

    public bool CheckIfLocked()
    {
        return isLocked;
    }

    public void UnlockDoor()
    {
        player.GetComponent<PlayerController>().StopDodgeRoll();
        player.GetComponent<PlayerController>().canMove = false;
        player.GetComponent<PlayerController>().GiveIFrames(2.0f);
        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        player.GetComponent<Animator>().Play("Player_Idle");
        player.GetComponent<PlayerAudio>().UnlockSound();
        Invoke("ThrowPadlock", 0.5f);
    }

    private void ThrowPadlock()
    {
        isLocked = false;
        padlockAnim.Play("Padlock_Unlocked");
        padlockBody.bodyType = RigidbodyType2D.Dynamic;
        padlockBody.AddForce(new Vector2(0, 300.0f));

        Invoke("OpenDoor", 1.0f);
    }

    private void OpenDoor()
    {
        Destroy(padlock);
        doorAnim.Play("Door_Open");
        fader.GetComponent<Animator>().SetTrigger("FadeOut");
        player.GetComponent<PlayerAudio>().WinSound();
        Invoke("NextScene", 1.5f);
    }

    private void NextScene()
    {
        player.GetComponent<PlayerController>().canMove = true;
        player.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
