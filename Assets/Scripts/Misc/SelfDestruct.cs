using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    private void Awake()
    {
        Invoke("DestroySelf", 2.0f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
