using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenInput : MonoBehaviour
{
    [SerializeField] GameObject fader;

    void Update()
    {
        if (Input.anyKey)
        {
            fader.GetComponent<Animator>().SetTrigger("FadeOut");
            Invoke("LoadLevel", 1.5f);
        }
    }

    void LoadLevel()
    {
        SceneManager.LoadScene(1);
    }
}
