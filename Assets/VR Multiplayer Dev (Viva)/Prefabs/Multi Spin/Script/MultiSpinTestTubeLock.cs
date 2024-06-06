using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Photon.Pun;
using System.Text.RegularExpressions;

public class MultiSpinTestTubeLock : MonoBehaviour
{
    [SerializeField]
    private Transform spinner;
    public bool isOccupied = false;
    
    PhotonView View;
    
    // Start is called before the first frame update
    void Start()
    {
        View = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<TestTube>() != null && !isOccupied)
        {
            if(other.GetComponent<TestTube>().grabbed == true){
                GameObject testTube = other.gameObject;
                PhotonView testTubePhotonView = testTube.GetComponent<PhotonView>();
                View.RPC("PhotonTriggerEnter", RpcTarget.AllBuffered,testTubePhotonView.ViewID);
                
            }
            
        }
        
    }

    [PunRPC]
    public void PhotonTriggerEnter(int testTubeID){
        DebugUIManager.instance.ShowDebugUIMessage("Enter");
        PhotonView testTubePhotonView = PhotonView.Find(testTubeID);
        GameObject testTube = testTubePhotonView.gameObject;
        ConstraintSource constraintSource = new ConstraintSource();
        constraintSource.sourceTransform = this.gameObject.transform;
        constraintSource.weight = 1;
        ParentConstraint constraint = testTube.AddComponent<ParentConstraint>();
        constraint.AddSource(constraintSource);
        constraint.constraintActive = true;
        isOccupied = true;
        testTube.GetComponent<TestTube>().grabbed = false;
        
    }

    void OnTriggerExit(Collider other)
    {
        GameObject testTube;
        PhotonView testTubePhotonView;
        if (other.gameObject.GetComponent<TestTube>() != null)
        {
            if(other.GetComponent<TestTube>().grabbed == true){
                testTube = other.gameObject;
                testTubePhotonView = testTube.GetComponent<PhotonView>();
                View.RPC("PhotonOnTriggerExit", RpcTarget.AllBuffered,testTubePhotonView.ViewID);
            }
            
        }
        
    }
    [PunRPC]
    public void PhotonOnTriggerExit(int testTubeID){
        DebugUIManager.instance.ShowDebugUIMessage("Exit");
        PhotonView testTubePhotonView = PhotonView.Find(testTubeID);
        GameObject testTube = testTubePhotonView.gameObject;
        ParentConstraint constraint = testTube.GetComponent<ParentConstraint>();
        Destroy(constraint);
        isOccupied = false;
        testTube.GetComponent<TestTube>().grabbed = false;
    }

    public void OnReset() { 
        if(PhotonNetwork.IsConnected)
        {
            View.RPC("PhotonReset", RpcTarget.All);
        }
    }

    [PunRPC]
    private void PhotonReset()
    {
        isOccupied = false;
    }

}
