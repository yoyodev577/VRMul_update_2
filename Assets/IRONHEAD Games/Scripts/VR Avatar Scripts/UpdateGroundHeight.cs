using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateGroundHeight : MonoBehaviour
{
    public Transform xrRig, model;
    private bool isLocalPlayer = false;
    // Update is called once per frame
    private Vector3 offset;
    private void Awake()
    {
        PhotonView phoView = gameObject.GetComponent<PhotonView>();
        if (phoView.IsMine) isLocalPlayer = true;
        offset = new Vector3(model.position.x, xrRig.position.y, model.position.z + 0.5f);

    }
    void Update()
    {
        if (isLocalPlayer){
            model.position = new Vector3(model.position.x, xrRig.position.y-0.2f, model.position.z);
            // xrRig.position = new Vector3(model.position.x, xrRig.position.y, model.position.z + 0.5f);
            // xrRig.position = offset;
            } 
    }
}