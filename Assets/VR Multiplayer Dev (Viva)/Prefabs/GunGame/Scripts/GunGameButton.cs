using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using System.Text.RegularExpressions;

public class GunGameButton : MonoBehaviour
{
   
    public UnityEvent onPressed, onReleased;
    private bool isPressed;
    [SerializeField] private float threshold = .1f;
    [SerializeField] private float deadZone = .025f;
    private Vector3 startPos;
    private ConfigurableJoint joint;
    PhotonView View;
    private AudioSource sound;
    public Material[] buttonLight;

    public bool ReadyValue;

    public bool NeedReset; // false = no need/ true = need
    
    // Start is called before the first frame update
    void Start()
    {
        View = this.gameObject.GetComponent<PhotonView>();
        sound = this.transform.GetChild(0).GetComponent<AudioSource>();
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();
        ReadyValue =false;
        NeedReset = false;
        
    }
    // Update is called on every frame
    void Update()
    {
        if (!isPressed && GetValue() + threshold >= 1)
        {
            Pressed();
        }
        if (isPressed && GetValue() - threshold <= 0)
        {
            Released();
        }


        
    }


    //ref.:https://www.youtube.com/watch?v=HFNzVMi5MSQ
    private float GetValue()
    {
        var value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
        if (Math.Abs(value) < deadZone)
        {
            value = 0;
        }
        return Math.Clamp(value, -1f, 1f);
    }


    #region Invoking Events
    private void Pressed()
    {
        isPressed = true;
        onPressed.Invoke();
        sound.Play();
    }

    private void Released()
    {
        isPressed = false;
        onReleased.Invoke();
    }
    #endregion


    public void PlayerReady(){
        // if(!ReadyValue){
        //     this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = buttonLight[1];
        //     ReadyValue = true;
        // }else{
        //     this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = buttonLight[0];
        //     ReadyValue = false;
        // }

        View.RPC("PhotonPlayerReady", RpcTarget.AllBuffered);

    }

    [PunRPC]
    public void PhotonPlayerReady(){
        if(!ReadyValue){
            this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = buttonLight[1];
            ReadyValue = true;
        }else{
            this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = buttonLight[0];
            ReadyValue = false;
        }
    }


    public void Reset(){
        // this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = buttonLight[1];
        // NeedReset = true;
        View.RPC("PhotonReset",RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void PhotonReset(){
        this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = buttonLight[1];
        NeedReset = true;
    }

    public void Reseted(){
        View.RPC("PhotonReseted",RpcTarget.AllBuffered);
    }


    [PunRPC]
    public void PhotonReseted(){
        this.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = buttonLight[0];
        
    }

}