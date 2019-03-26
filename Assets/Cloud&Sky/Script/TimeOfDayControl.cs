using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDayControl : MonoBehaviour
{
    public bool autoTime = false;
    public float timeSpeed = 0.45f;
    [Range(0, 24)] public float timeOfDay;
    public Light sunLight;
    public Transform sunTransform;
    public Light moonLight;
    public Transform moonTransform;
    public float sunXto;
    public float Yto;
    [Range(0, 2)] public float moonHaloIntensity = 1.2f;

    public Material matSky;
    public Material matCloud;
    public Material mat00;
    public Material mat04;
    public Material mat06;
    public Material mat07;
    public Material mat09;
    public Material mat12;
    public Material mat15;
    public Material mat17;
    public Material mat18;
    public Material mat20;

    //public Color sunColor00;
    //public Color sunColor04;
    public Color sunColor06;
    public Color sunColor07;
    public Color sunColor09;
    public Color sunColor12;
    public Color sunColor15;
    public Color sunColor17;

    public Color sunColor18;
    //public Color sunColor20;

    private Color _PartialRayleighInScattering;
    private Color _PartialMieInScattering;
    private Color _NightSkyColBase;
    private Color _NightSkyColDelta;

    private Color _CloudColor;
    private Color _CloudDarkColor;
    private float _TransmitExp;
    private float _TransmitIntensity;
    private float _TransmitEdge;

    private float _TransmitBase;

    private void OnEnable()
    {
        sunTransform = sunLight.GetComponent<Transform>();
        SetSkyParams();
    }

    void Start()
    {
        sunTransform = sunLight.GetComponent<Transform>();
        SetSkyParams();
    }

    void Update()
    {
        if (autoTime)
        {
            timeOfDay += timeSpeed * Time.deltaTime;
            if (timeOfDay >= 24)
                timeOfDay = 0;
        }

        sunXto = Mathf.Lerp(-90f, 270f, timeOfDay / 24f);
        sunTransform.localRotation = Quaternion.Euler(sunXto, Yto, 0f);

        float moonXto = -90f;
        if (timeOfDay >= 19)
        {
            moonXto = Mathf.Lerp(-15f, 90f, (timeOfDay - 19) / 5);
        }
        if (timeOfDay <= 5)
        {
            moonXto = Mathf.Lerp(90f, 195f, (timeOfDay) / 5);
        }
        moonTransform.localRotation = Quaternion.Euler(moonXto, 0, 0f);

        SetSkyParams();
    }

    //直接进行线性插值即可
    void SetSkyParams()
    {
        if (timeOfDay >= 0 && timeOfDay < 4)
        {
            float lerp0_4 = (timeOfDay) / 4f;
            _PartialRayleighInScattering = Color.Lerp(mat00.GetColor("_PartialRayleighInScattering"),
                mat04.GetColor("_PartialRayleighInScattering"), lerp0_4);
            _PartialMieInScattering = Color.Lerp(mat00.GetColor("_PartialMieInScattering"),
                mat04.GetColor("_PartialMieInScattering"), lerp0_4);
            _NightSkyColBase = Color.Lerp(mat00.GetColor("_NightSkyColBase"), mat04.GetColor("_NightSkyColBase"),
                lerp0_4);
            _NightSkyColDelta = Color.Lerp(mat00.GetColor("_NightSkyColDelta"), mat04.GetColor("_NightSkyColDelta"),
                lerp0_4);

            _CloudColor = Color.Lerp(mat00.GetColor("_CloudColor"), mat04.GetColor("_CloudColor"), lerp0_4);
            _CloudDarkColor = Color.Lerp(mat00.GetColor("_CloudDarkColor"), mat04.GetColor("_CloudDarkColor"), lerp0_4);

            _TransmitExp = Mathf.Lerp(mat00.GetFloat("_TransmitExp"), mat04.GetFloat("_TransmitExp"), lerp0_4);
            _TransmitIntensity = Mathf.Lerp(mat00.GetFloat("_TransmitIntensity"), mat04.GetFloat("_TransmitIntensity"),
                lerp0_4);
            _TransmitEdge = Mathf.Lerp(mat00.GetFloat("_TransmitEdge"), mat04.GetFloat("_TransmitEdge"), lerp0_4);
            _TransmitBase = Mathf.Lerp(mat00.GetFloat("_TransmitBase"), mat04.GetFloat("_TransmitBase"), lerp0_4);
        }

        if (timeOfDay >= 4 && timeOfDay < 6)
        {
            float lerp4_6 = (timeOfDay - 4f) / 2f;
            _PartialRayleighInScattering = Color.Lerp(mat04.GetColor("_PartialRayleighInScattering"),
                mat06.GetColor("_PartialRayleighInScattering"), lerp4_6);
            _PartialMieInScattering = Color.Lerp(mat04.GetColor("_PartialMieInScattering"),
                mat06.GetColor("_PartialMieInScattering"), lerp4_6);
            _NightSkyColBase = Color.Lerp(mat04.GetColor("_NightSkyColBase"), mat06.GetColor("_NightSkyColBase"),
                lerp4_6);
            _NightSkyColDelta = Color.Lerp(mat04.GetColor("_NightSkyColDelta"), mat06.GetColor("_NightSkyColDelta"),
                lerp4_6);

            _CloudColor = Color.Lerp(mat04.GetColor("_CloudColor"), mat06.GetColor("_CloudColor"), lerp4_6);
            _CloudDarkColor = Color.Lerp(mat04.GetColor("_CloudDarkColor"), mat06.GetColor("_CloudDarkColor"), lerp4_6);

            _TransmitExp = Mathf.Lerp(mat04.GetFloat("_TransmitExp"), mat06.GetFloat("_TransmitExp"), lerp4_6);
            _TransmitIntensity = Mathf.Lerp(mat04.GetFloat("_TransmitIntensity"), mat06.GetFloat("_TransmitIntensity"),
                lerp4_6);
            _TransmitEdge = Mathf.Lerp(mat04.GetFloat("_TransmitEdge"), mat06.GetFloat("_TransmitEdge"), lerp4_6);
            _TransmitBase = Mathf.Lerp(mat04.GetFloat("_TransmitBase"), mat06.GetFloat("_TransmitBase"), lerp4_6);
        }

        if (timeOfDay <= 5) //timeOfDay > 4 &&
        {
            float lerp45_5 = (timeOfDay - 4.5f) * 2;
            moonLight.intensity = Mathf.Lerp(0.55f, 0f, lerp45_5);
            matSky.SetFloat("_SunHaloIntensity", Mathf.Lerp(moonHaloIntensity, 0.36f, lerp45_5));
            sunLight.intensity = 0f;
        }

        if (timeOfDay > 5 && timeOfDay <= 19)
        {
            moonLight.intensity = 0f;
            matSky.SetFloat("_SunHaloIntensity", 0.36f);
        }

        if (timeOfDay > 5) //&& timeOfDay <= 6
        {
            float lerp5_6 = (timeOfDay - 5);
            sunLight.intensity = Mathf.Lerp(0f, 1.5f, lerp5_6);
        }

        if (timeOfDay >= 6 && timeOfDay < 7)
        {
            float lerp6_7 = (timeOfDay - 6);
            _PartialRayleighInScattering = Color.Lerp(mat06.GetColor("_PartialRayleighInScattering"),
                mat07.GetColor("_PartialRayleighInScattering"), lerp6_7);
            _PartialMieInScattering = Color.Lerp(mat06.GetColor("_PartialMieInScattering"),
                mat07.GetColor("_PartialMieInScattering"), lerp6_7);
            _NightSkyColBase = Color.Lerp(mat06.GetColor("_NightSkyColBase"), mat07.GetColor("_NightSkyColBase"),
                lerp6_7);
            _NightSkyColDelta = Color.Lerp(mat06.GetColor("_NightSkyColDelta"), mat07.GetColor("_NightSkyColDelta"),
                lerp6_7);

            _CloudColor = Color.Lerp(mat06.GetColor("_CloudColor"), mat07.GetColor("_CloudColor"), lerp6_7);
            _CloudDarkColor = Color.Lerp(mat06.GetColor("_CloudDarkColor"), mat07.GetColor("_CloudDarkColor"), lerp6_7);

            _TransmitExp = Mathf.Lerp(mat06.GetFloat("_TransmitExp"), mat07.GetFloat("_TransmitExp"), lerp6_7);
            _TransmitIntensity = Mathf.Lerp(mat06.GetFloat("_TransmitIntensity"), mat07.GetFloat("_TransmitIntensity"),
                lerp6_7);
            _TransmitEdge = Mathf.Lerp(mat06.GetFloat("_TransmitEdge"), mat07.GetFloat("_TransmitEdge"), lerp6_7);
            _TransmitBase = Mathf.Lerp(mat06.GetFloat("_TransmitBase"), mat07.GetFloat("_TransmitBase"), lerp6_7);

            sunLight.color = Color.Lerp(sunColor06, sunColor07, lerp6_7);
        }

        if (timeOfDay >= 7 && timeOfDay < 9)
        {
            float lerp7_9 = (timeOfDay - 7) / 2f;
            _PartialRayleighInScattering = Color.Lerp(mat07.GetColor("_PartialRayleighInScattering"),
                mat09.GetColor("_PartialRayleighInScattering"), lerp7_9);
            _PartialMieInScattering = Color.Lerp(mat07.GetColor("_PartialMieInScattering"),
                mat09.GetColor("_PartialMieInScattering"), lerp7_9);
            _NightSkyColBase = Color.Lerp(mat07.GetColor("_NightSkyColBase"), mat09.GetColor("_NightSkyColBase"),
                lerp7_9);
            _NightSkyColDelta = Color.Lerp(mat07.GetColor("_NightSkyColDelta"), mat09.GetColor("_NightSkyColDelta"),
                lerp7_9);

            _CloudColor = Color.Lerp(mat07.GetColor("_CloudColor"), mat09.GetColor("_CloudColor"), lerp7_9);
            _CloudDarkColor = Color.Lerp(mat07.GetColor("_CloudDarkColor"), mat09.GetColor("_CloudDarkColor"), lerp7_9);

            _TransmitExp = Mathf.Lerp(mat07.GetFloat("_TransmitExp"), mat09.GetFloat("_TransmitExp"), lerp7_9);
            _TransmitIntensity = Mathf.Lerp(mat07.GetFloat("_TransmitIntensity"), mat09.GetFloat("_TransmitIntensity"),
                lerp7_9);
            _TransmitEdge = Mathf.Lerp(mat07.GetFloat("_TransmitEdge"), mat09.GetFloat("_TransmitEdge"), lerp7_9);
            _TransmitBase = Mathf.Lerp(mat07.GetFloat("_TransmitBase"), mat09.GetFloat("_TransmitBase"), lerp7_9);

            sunLight.color = Color.Lerp(sunColor07, sunColor09, lerp7_9);
        }

        if (timeOfDay >= 9 && timeOfDay < 12)
        {
            float lerp9_12 = (timeOfDay - 9) / 3f;
            _PartialRayleighInScattering = Color.Lerp(mat09.GetColor("_PartialRayleighInScattering"),
                mat12.GetColor("_PartialRayleighInScattering"), lerp9_12);
            _PartialMieInScattering = Color.Lerp(mat09.GetColor("_PartialMieInScattering"),
                mat12.GetColor("_PartialMieInScattering"), lerp9_12);
            _NightSkyColBase = Color.Lerp(mat09.GetColor("_NightSkyColBase"), mat12.GetColor("_NightSkyColBase"),
                lerp9_12);
            _NightSkyColDelta = Color.Lerp(mat09.GetColor("_NightSkyColDelta"), mat12.GetColor("_NightSkyColDelta"),
                lerp9_12);

            _CloudColor = Color.Lerp(mat09.GetColor("_CloudColor"), mat12.GetColor("_CloudColor"), lerp9_12);
            _CloudDarkColor = Color.Lerp(mat09.GetColor("_CloudDarkColor"), mat12.GetColor("_CloudDarkColor"),
                lerp9_12);

            _TransmitExp = Mathf.Lerp(mat09.GetFloat("_TransmitExp"), mat12.GetFloat("_TransmitExp"), lerp9_12);
            _TransmitIntensity = Mathf.Lerp(mat09.GetFloat("_TransmitIntensity"), mat12.GetFloat("_TransmitIntensity"),
                lerp9_12);
            _TransmitEdge = Mathf.Lerp(mat09.GetFloat("_TransmitEdge"), mat12.GetFloat("_TransmitEdge"), lerp9_12);
            _TransmitBase = Mathf.Lerp(mat09.GetFloat("_TransmitBase"), mat12.GetFloat("_TransmitBase"), lerp9_12);

            sunLight.color = Color.Lerp(sunColor09, sunColor12, lerp9_12);
        }

        if (timeOfDay >= 12 && timeOfDay < 15)
        {
            float lerp12_15 = (timeOfDay - 12) / 3f;
            _PartialRayleighInScattering = Color.Lerp(mat12.GetColor("_PartialRayleighInScattering"),
                mat15.GetColor("_PartialRayleighInScattering"), lerp12_15);
            _PartialMieInScattering = Color.Lerp(mat12.GetColor("_PartialMieInScattering"),
                mat15.GetColor("_PartialMieInScattering"), lerp12_15);
            _NightSkyColBase = Color.Lerp(mat12.GetColor("_NightSkyColBase"), mat15.GetColor("_NightSkyColBase"),
                lerp12_15);
            _NightSkyColDelta = Color.Lerp(mat12.GetColor("_NightSkyColDelta"), mat15.GetColor("_NightSkyColDelta"),
                lerp12_15);

            _CloudColor = Color.Lerp(mat12.GetColor("_CloudColor"), mat15.GetColor("_CloudColor"), lerp12_15);
            _CloudDarkColor = Color.Lerp(mat12.GetColor("_CloudDarkColor"), mat15.GetColor("_CloudDarkColor"),
                lerp12_15);

            _TransmitExp = Mathf.Lerp(mat12.GetFloat("_TransmitExp"), mat15.GetFloat("_TransmitExp"), lerp12_15);
            _TransmitIntensity = Mathf.Lerp(mat12.GetFloat("_TransmitIntensity"), mat15.GetFloat("_TransmitIntensity"),
                lerp12_15);
            _TransmitEdge = Mathf.Lerp(mat12.GetFloat("_TransmitEdge"), mat15.GetFloat("_TransmitEdge"), lerp12_15);
            _TransmitBase = Mathf.Lerp(mat12.GetFloat("_TransmitBase"), mat15.GetFloat("_TransmitBase"), lerp12_15);

            sunLight.color = Color.Lerp(sunColor12, sunColor15, lerp12_15);
        }

        if (timeOfDay >= 15 && timeOfDay < 17)
        {
            float lerp15_17 = (timeOfDay - 15) / 2f;
            _PartialRayleighInScattering = Color.Lerp(mat15.GetColor("_PartialRayleighInScattering"),
                mat17.GetColor("_PartialRayleighInScattering"), lerp15_17);
            _PartialMieInScattering = Color.Lerp(mat15.GetColor("_PartialMieInScattering"),
                mat17.GetColor("_PartialMieInScattering"), lerp15_17);
            _NightSkyColBase = Color.Lerp(mat15.GetColor("_NightSkyColBase"), mat17.GetColor("_NightSkyColBase"),
                lerp15_17);
            _NightSkyColDelta = Color.Lerp(mat15.GetColor("_NightSkyColDelta"), mat17.GetColor("_NightSkyColDelta"),
                lerp15_17);

            _CloudColor = Color.Lerp(mat15.GetColor("_CloudColor"), mat17.GetColor("_CloudColor"), lerp15_17);
            _CloudDarkColor = Color.Lerp(mat15.GetColor("_CloudDarkColor"), mat17.GetColor("_CloudDarkColor"),
                lerp15_17);

            _TransmitExp = Mathf.Lerp(mat15.GetFloat("_TransmitExp"), mat17.GetFloat("_TransmitExp"), lerp15_17);
            _TransmitIntensity = Mathf.Lerp(mat15.GetFloat("_TransmitIntensity"), mat17.GetFloat("_TransmitIntensity"),
                lerp15_17);
            _TransmitEdge = Mathf.Lerp(mat15.GetFloat("_TransmitEdge"), mat17.GetFloat("_TransmitEdge"), lerp15_17);
            _TransmitBase = Mathf.Lerp(mat15.GetFloat("_TransmitBase"), mat17.GetFloat("_TransmitBase"), lerp15_17);

            sunLight.color = Color.Lerp(sunColor15, sunColor17, lerp15_17);
        }

        if (timeOfDay >= 17 && timeOfDay < 18)
        {
            float lerp17_18 = (timeOfDay - 17);
            _PartialRayleighInScattering = Color.Lerp(mat17.GetColor("_PartialRayleighInScattering"),
                mat18.GetColor("_PartialRayleighInScattering"), lerp17_18);
            _PartialMieInScattering = Color.Lerp(mat17.GetColor("_PartialMieInScattering"),
                mat18.GetColor("_PartialMieInScattering"), lerp17_18);
            _NightSkyColBase = Color.Lerp(mat17.GetColor("_NightSkyColBase"), mat18.GetColor("_NightSkyColBase"),
                lerp17_18);
            _NightSkyColDelta = Color.Lerp(mat17.GetColor("_NightSkyColDelta"), mat18.GetColor("_NightSkyColDelta"),
                lerp17_18);

            _CloudColor = Color.Lerp(mat17.GetColor("_CloudColor"), mat18.GetColor("_CloudColor"), lerp17_18);
            _CloudDarkColor = Color.Lerp(mat17.GetColor("_CloudDarkColor"), mat18.GetColor("_CloudDarkColor"),
                lerp17_18);

            _TransmitExp = Mathf.Lerp(mat17.GetFloat("_TransmitExp"), mat18.GetFloat("_TransmitExp"), lerp17_18);
            _TransmitIntensity = Mathf.Lerp(mat17.GetFloat("_TransmitIntensity"), mat18.GetFloat("_TransmitIntensity"),
                lerp17_18);
            _TransmitEdge = Mathf.Lerp(mat17.GetFloat("_TransmitEdge"), mat18.GetFloat("_TransmitEdge"), lerp17_18);
            _TransmitBase = Mathf.Lerp(mat17.GetFloat("_TransmitBase"), mat18.GetFloat("_TransmitBase"), lerp17_18);

            sunLight.color = Color.Lerp(sunColor17, sunColor18, lerp17_18);
        }

        if (timeOfDay > 18) // && timeOfDay <= 19
        {
            float lerp18_19 = (timeOfDay - 18);
            sunLight.intensity = Mathf.Lerp(1.5f, 0f, lerp18_19);
        }

        if (timeOfDay > 19) //&& timeOfDay <= 20
        {
            float lerp19_195 = (timeOfDay - 19) * 2;
            moonLight.intensity = Mathf.Lerp(0f, 0.55f, lerp19_195);
            matSky.SetFloat("_SunHaloIntensity", Mathf.Lerp(0.36f, moonHaloIntensity, lerp19_195));
            sunLight.intensity = 0f;
        }

        if (timeOfDay >= 18 && timeOfDay < 20)
        {
            float lerp18_20 = (timeOfDay - 18) / 2f;

            _PartialRayleighInScattering = Color.Lerp(mat18.GetColor("_PartialRayleighInScattering"),
                mat20.GetColor("_PartialRayleighInScattering"), lerp18_20);
            _PartialMieInScattering = Color.Lerp(mat18.GetColor("_PartialMieInScattering"),
                mat20.GetColor("_PartialMieInScattering"), lerp18_20);
            _NightSkyColBase = Color.Lerp(mat18.GetColor("_NightSkyColBase"), mat20.GetColor("_NightSkyColBase"),
                lerp18_20);
            _NightSkyColDelta = Color.Lerp(mat18.GetColor("_NightSkyColDelta"), mat20.GetColor("_NightSkyColDelta"),
                lerp18_20);

            _CloudColor = Color.Lerp(mat18.GetColor("_CloudColor"), mat20.GetColor("_CloudColor"), lerp18_20);
            _CloudDarkColor = Color.Lerp(mat18.GetColor("_CloudDarkColor"), mat20.GetColor("_CloudDarkColor"),
                lerp18_20);

            _TransmitExp = Mathf.Lerp(mat18.GetFloat("_TransmitExp"), mat20.GetFloat("_TransmitExp"), lerp18_20);
            _TransmitIntensity = Mathf.Lerp(mat18.GetFloat("_TransmitIntensity"), mat20.GetFloat("_TransmitIntensity"),
                lerp18_20);
            _TransmitEdge = Mathf.Lerp(mat18.GetFloat("_TransmitEdge"), mat20.GetFloat("_TransmitEdge"), lerp18_20);
            _TransmitBase = Mathf.Lerp(mat18.GetFloat("_TransmitBase"), mat20.GetFloat("_TransmitBase"), lerp18_20);
        }



        if (timeOfDay >= 20 && timeOfDay < 24)
        {
            float lerp20_24 = (timeOfDay - 20) / 4f;

            _PartialRayleighInScattering = Color.Lerp(mat20.GetColor("_PartialRayleighInScattering"),
                mat00.GetColor("_PartialRayleighInScattering"), lerp20_24);
            _PartialMieInScattering = Color.Lerp(mat20.GetColor("_PartialMieInScattering"),
                mat00.GetColor("_PartialMieInScattering"), lerp20_24);
            _NightSkyColBase = Color.Lerp(mat20.GetColor("_NightSkyColBase"), mat00.GetColor("_NightSkyColBase"),
                lerp20_24);
            _NightSkyColDelta = Color.Lerp(mat20.GetColor("_NightSkyColDelta"), mat00.GetColor("_NightSkyColDelta"),
                lerp20_24);

            _CloudColor = Color.Lerp(mat20.GetColor("_CloudColor"), mat00.GetColor("_CloudColor"), lerp20_24);
            _CloudDarkColor = Color.Lerp(mat20.GetColor("_CloudDarkColor"), mat00.GetColor("_CloudDarkColor"),
                lerp20_24);

            _TransmitExp = Mathf.Lerp(mat20.GetFloat("_TransmitExp"), mat00.GetFloat("_TransmitExp"), lerp20_24);
            _TransmitIntensity = Mathf.Lerp(mat20.GetFloat("_TransmitIntensity"), mat00.GetFloat("_TransmitIntensity"),
                lerp20_24);
            _TransmitEdge = Mathf.Lerp(mat20.GetFloat("_TransmitEdge"), mat00.GetFloat("_TransmitEdge"), lerp20_24);
            _TransmitBase = Mathf.Lerp(mat20.GetFloat("_TransmitBase"), mat00.GetFloat("_TransmitBase"), lerp20_24);

        }

        Vector3 sunVector = sunTransform.rotation * Vector3.forward;
        matSky.SetColor("_SunLightDir", new Vector4(sunVector.x, sunVector.y, sunVector.z, 0f));

        matSky.SetColor("_PartialRayleighInScattering", _PartialRayleighInScattering);
        matSky.SetColor("_PartialMieInScattering", _PartialMieInScattering);
        matSky.SetColor("_NightSkyColBase", _NightSkyColBase);
        matSky.SetColor("_NightSkyColDelta", _NightSkyColDelta);
        matSky.SetColor("_CloudColor", _CloudColor);
        matSky.SetColor("_CloudDarkColor", _CloudDarkColor);
        matCloud.SetColor("_CloudColor", _CloudColor);
        matCloud.SetColor("_CloudDarkColor", _CloudDarkColor);
        matCloud.SetFloat("_TransmitExp", _TransmitExp);
        matCloud.SetFloat("_TransmitIntensity", _TransmitIntensity);
        matCloud.SetFloat("_TransmitEdge", _TransmitEdge);
        matCloud.SetFloat("_TransmitBase", _TransmitBase);

        RenderSettings.ambientLight = _NightSkyColBase;
    }

}