using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[ExecuteInEditMode]
public class LocalAngleReferencer : MonoBehaviour
{
    [SerializeField]
    float eulerAngX;
    [SerializeField]
    float eulerAngY;
    [SerializeField]
    float eulerAngZ;

    void Update()
    {
        eulerAngX = transform.localEulerAngles.x;
        eulerAngY = transform.localEulerAngles.y;
        eulerAngZ = transform.localEulerAngles.z;
        float [] eulerAngles = new float[3]
        {
            eulerAngX,
            eulerAngY,
            eulerAngZ
        };
        /*
        for (int i = 0; i<eulerAngles.Length; i++)
        {
            if(eulerAngles[i] > 180)
            {
                eulerAngles[i] -= 360;
            }
        }
        */
        eulerAngX = eulerAngles[0];
        eulerAngY = eulerAngles[1];
        eulerAngZ = eulerAngles[2];
    }
}
