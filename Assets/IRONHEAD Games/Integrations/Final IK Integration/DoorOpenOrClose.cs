using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenOrClose : MonoBehaviour
{
    private Animation animation;
    private void Start()
    {
        animation = gameObject.GetComponent<Animation>();
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject)
        {
            animation.Play("DoorOpen");
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.gameObject)
        {
            animation.Play("DoorClose");
        }
    }
}
