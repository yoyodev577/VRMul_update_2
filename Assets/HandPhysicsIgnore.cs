using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysicsIgnore : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        Physics.IgnoreLayerCollision(10, 18);
    }
}
