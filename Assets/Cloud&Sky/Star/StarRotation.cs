using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRotation : MonoBehaviour
{
    public bool rotation;
    public float speed = 0f;
    float Yto = 0f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (rotation)
        {
            Yto += speed * Time.deltaTime;
            transform.localRotation = Quaternion.Euler(0f, Yto, 0f);
        }
    }
}
