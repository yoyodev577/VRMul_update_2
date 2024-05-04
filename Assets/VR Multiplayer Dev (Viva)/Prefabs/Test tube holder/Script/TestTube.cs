using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestTube : MonoBehaviour
{
    [SerializeField]
    private GameObject cap, body;
    private Transform coverPos;
    public bool grabbed  = false; //player grabbing the testtube or not
    // Start is called before the first frame update
    PhotonView View;
    void Start()
    {
        View = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void grab(){
        View.RPC("PhotonGrab", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void PhotonGrab(){
        grabbed = true;
    }
}
