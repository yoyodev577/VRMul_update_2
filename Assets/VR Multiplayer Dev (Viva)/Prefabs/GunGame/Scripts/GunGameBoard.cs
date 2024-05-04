using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GunGameBoard : MonoBehaviour
{
    private bool goRight = true;
    private Vector3 originalPosition;
    // Start is called before the first frame update

    PhotonView View;
    void Start()
    {
        View = this.gameObject.GetComponent<PhotonView>();
        originalPosition = this.gameObject.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (goRight){
            this.gameObject.transform.Translate(Vector3.right * Time.deltaTime * 2f);
        }else{
            this.gameObject.transform.Translate(Vector3.left * Time.deltaTime * 2f);
        }
    }

    void OnTriggerEnter(Collider other){

        if (!PhotonNetwork.IsConnected) return;

        // PhotonView wallObject = other.gameObject.GetComponent<PhotonView>();
        // View.RPC("PhotonOnTriggerEnter",RpcTarget.AllBuffered,wallObject.ViewID);
        if(other.gameObject.GetComponent<RightWall>() != null){
            // this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            // goRight = false;
            View.RPC("PhotonChangeDirection",RpcTarget.AllBuffered,false);
        }else if(other.gameObject.GetComponent<LeftWall>() != null){
            // goRight = true;
            View.RPC("PhotonChangeDirection",RpcTarget.AllBuffered,true);
        }

        if(other.gameObject.GetComponent<GroundWall>() != null){
            // this.gameObject.GetComponent<Rigidbody>().useGravity = false;
            // for(int i = 0; i< this.gameObject.transform.childCount; i++){
            //     this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            // }
            // this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // this.gameObject.transform.position = originalPosition;
            // StartCoroutine(TimeForGenerateBoard());

            View.RPC("PhotonDropBoard",RpcTarget.AllBuffered);
        }
        
    }
    [PunRPC]
    public void PhotonChangeDirection(bool value){
        goRight = value;
    }
    [PunRPC]
    public void PhotonDropBoard(){
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        for(int i = 0; i< this.gameObject.transform.childCount; i++){
            this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        this.gameObject.transform.position = originalPosition;
        StartCoroutine(TimeForGenerateBoard());
    }

    // [PunRPC]
    // public void PhotonOnTriggerEnter(int wallObjectID){
    //     PhotonView wallObject = PhotonView.Find(wallObjectID);

    //     if(wallObject.gameObject.GetComponent<RightWall>() != null){
    //         // this.gameObject.transform.GetChild(0).gameObject.SetActive(false);
    //         goRight = false;
    //     }else if(wallObject.gameObject.GetComponent<LeftWall>() != null){
    //         goRight = true;
    //     }

    //     if(wallObject.gameObject.GetComponent<GroundWall>() != null){
    //         this.gameObject.GetComponent<Rigidbody>().useGravity = false;
    //         for(int i = 0; i< this.gameObject.transform.childCount; i++){
    //             this.gameObject.transform.GetChild(i).gameObject.SetActive(false);
    //         }
    //         this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //         this.gameObject.transform.position = originalPosition;
    //         StartCoroutine(TimeForGenerateBoard());

    //     }
    // }

    IEnumerator TimeForGenerateBoard(){
        yield return new WaitForSeconds(1);
        for(int i = 0; i< this.gameObject.transform.childCount; i++){
                this.gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }


    public void ResetBoardPosition(){
        this.gameObject.transform.position = originalPosition;
    }

}
