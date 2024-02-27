using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform cameraObject;

    void LateUpdate()
    {
        transform.LookAt(transform.position + cameraObject.forward);
    }
}
