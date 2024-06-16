using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Photon.Pun;
using System.Text.RegularExpressions;

public class MultiSpinTestTubeLock : MonoBehaviour
{
    [SerializeField] private MultiSpin multiSpin;
    [SerializeField]
    private Transform spinner;
    public TestTube occupiedTestTube;
    public bool isOccupied = false;
    
    PhotonView View;
    
    // Start is called before the first frame update
    void Start()
    {
        View = GetComponent<PhotonView>();
        multiSpin = GetComponentInParent<MultiSpin>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!MultispinGameManager.instance.IsGameStart) return;

        if (multiSpin.isSpinning || multiSpin.isSpinningFinished || !multiSpin.isLidOpened) return;


        if(other.gameObject.GetComponent<TestTube>() != null && !isOccupied)
        {
            if(other.GetComponent<TestTube>().grabbed == true && !multiSpin.isSpinning){
                GameObject testTube = other.gameObject;
                occupiedTestTube = testTube.GetComponent<TestTube>(); 
                PhotonView testTubePhotonView = occupiedTestTube.GetComponent<PhotonView>();
                multiSpin.SetLockedTestTube(occupiedTestTube, true);
                View.RPC("PhotonTriggerEnter", RpcTarget.AllBuffered,testTubePhotonView.ViewID);
                
            }
            
        }
        
    }

    [PunRPC]
    public void PhotonTriggerEnter(int testTubeID){

        Debug.Log("---TestTube Lock Trigger --");
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
                multiSpin.SetLockedTestTube(occupiedTestTube, false);
                View.RPC("PhotonOnTriggerExit", RpcTarget.AllBuffered,testTubePhotonView.ViewID);
                occupiedTestTube = null;
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
            View.RPC("PhotonReset", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void PhotonReset()
    {
        isOccupied = false;
        occupiedTestTube = null;
    }

}
