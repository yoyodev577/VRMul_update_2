using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrateSize : MonoBehaviour
{
    public Transform upperArmBoneLeft, lowerArmBoneLeft;
    public Transform upperArmBoneRight, lowerArmBoneRight;
    public float scalePercentage = .05f;
    private float scaleHeight, scaleArms;

    public void GrowHeight()
    {
        scaleHeight = this.transform.localScale.y + scalePercentage;
        //this.gameObject.transform.localScale
    }
}
