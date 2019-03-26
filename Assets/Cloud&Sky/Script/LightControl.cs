using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    Light myLight;
    public Color HighColor;
    public Color HorizonColor;
    public float rotationX;
    public Material MatMoon;

    private void OnEnable()
    {
        myLight = GetComponent<Light>();
    }

    void Update()
    {
        rotationX = transform.eulerAngles.x;
        rotationX = ((rotationX > 180) ? rotationX - 360 : rotationX) / 90f;
        rotationX = Mathf.Clamp01(rotationX * 4.75f);

        myLight.color = Color.Lerp(HorizonColor, HighColor, rotationX);
        MatMoon.SetColor("_MoonColor", myLight.color);
    }
}
