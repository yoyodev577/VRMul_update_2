using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolderPhysicsIgnore : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        Physics.IgnoreLayerCollision(15, 17);
        Physics.IgnoreLayerCollision(16, 17);
    }
}
