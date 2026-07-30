using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;

    private CinemachineImpulseSource cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Has more one than Screen Shake:" + Instance + ", " + transform);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intensity = 1f)
    {
        cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
