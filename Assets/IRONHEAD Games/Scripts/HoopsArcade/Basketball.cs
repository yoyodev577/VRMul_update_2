using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basketball : MonoBehaviour
{
    [SerializeField] private Vector3 startPos;
    Rigidbody rb;
    PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        view = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>(); 
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor") {
            if (PhotonNetwork.IsConnected)
                view.RPC("ResetPosition", RpcTarget.All);

            ResetPosition();
        }
    }

    [PunRPC]
    public void ResetPosition()
    {
        rb.isKinematic = true;
        transform.position = startPos;
        rb.isKinematic = false;
    }
}
