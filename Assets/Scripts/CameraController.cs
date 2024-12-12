using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform pcTransform;
    public Vector3 offset;

    void LateUpdate()
    {
        if(pcTransform != null)
        {
            transform.position = pcTransform.position + offset;

            if(pcTransform.localScale.x > 0)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }
}
