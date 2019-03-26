using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LowCloudRotate : MonoBehaviour
{
    public bool rotating;
    public float speed = 0f;
    private float Yto = 0f;
    void Start()
    {
        
    }

    void Update()
    {
        if (rotating)
        {
            Yto += speed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0f, Yto, 0f);
        }
    }
}
