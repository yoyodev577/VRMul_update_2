using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TestTube : MonoBehaviour
{
    private Vector3 startPos;
    [SerializeField]
    private GameObject cap, body;
    public bool grabbed  = false; //player grabbing the testtube or not
    // Start is called before the first frame update
    PhotonView View;
    Rigidbody rb;
    void Start()
    {
        View = GetComponent<PhotonView>();
        startPos= transform.position;
        rb = GetComponent<Rigidbody>();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            PhotonOnCollision();

            // View.RPC("PhotonOnTriggerEnter", RpcTarget.AllBuffered);
            Debug.Log(collision.gameObject.name + "Collides");

        }
    }

    [PunRPC]
    public void PhotonOnCollision()
    {
        transform.position = startPos;
    
    }
}
