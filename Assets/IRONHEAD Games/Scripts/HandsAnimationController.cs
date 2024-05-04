using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using IRONHEADGames;
using Photon.Pun;
public class HandsAnimationController : MonoBehaviour
{
    // [SerializeField] Animator LeftHandAnimator;
    // [SerializeField] Animator RightHandAnimator;
    PhotonView View;
    [SerializeField] InputAction gripInputAction;
    [SerializeField] InputAction triggerInputAction;
    [SerializeField] string WhichHand = "";

    [SerializeField] Animator animL;
    [SerializeField] Animator animR;
    int Idle = Animator.StringToHash("Idle");
    int GrabSmall = Animator.StringToHash("GrabSmall");
    int GrabLarge = Animator.StringToHash("GrabLarge");
    
    float currentPressed =0 ;
    float currentPressedR =0 ;
    float currentPressed_trigger=0 ;
    float currentPressed_triggerR=0 ;
    void Start()
    {
        View = GetComponent<PhotonView>();
    }
    private void Awake()
    {
        gripInputAction.performed += GripPressed;
        triggerInputAction.performed += TriggerPressed; 
    }

    private void OnEnable()
    {
        gripInputAction.Enable();
        triggerInputAction.Enable();     
    }

    private void OnDisable()
    {
        gripInputAction.Disable();
        triggerInputAction.Disable();
    }
   
    private void TriggerPressed(InputAction.CallbackContext obj)
    {
        // HandAnimator.SetFloat("Trigger_"+WhichHand, obj.ReadValue<float>());
        // DebugUIManager.instance.ShowDebugUIMessage("pressed");
        

        // DebugUIManager.instance.ShowDebugUIMessage(obj.ReadValue<float>().ToString());
        if(WhichHand == "Left"){
            animL.SetTrigger("Fist");
            // LeftHandAnimator.SetFloat("grip",obj.ReadValue<float>());
            // if(obj.ReadValue<float>()>0.5){
            //     animL.SetTrigger(GrabSmall);
            // }else{
            //     animL.SetTrigger(GrabLarge);
            // }
            if(obj.ReadValue<float>() - currentPressed_trigger >0){
                // animL.SetFloat("trigger",0.6f);
                View.RPC("PhotonSetFloatL", RpcTarget.AllBuffered,"trigger",0.6f);
            }else{
                // animL.SetFloat("trigger",0.4f);
                View.RPC("PhotonSetFloatL", RpcTarget.AllBuffered,"trigger",0.4f);
            }
            currentPressed_trigger=obj.ReadValue<float>();
            // animL.SetFloat("grab",obj.ReadValue<float>());
            
        }else{
            // RightHandAnimator.SetFloat("grip",obj.ReadValue<float>());
            if(obj.ReadValue<float>() - currentPressed_triggerR >0){
                // animR.SetFloat("trigger",0.6f);
                View.RPC("PhotonSetFloatR", RpcTarget.AllBuffered,"trigger",0.6f);
            }else{
                // animR.SetFloat("trigger",0.4f);
                View.RPC("PhotonSetFloatR", RpcTarget.AllBuffered,"trigger",0.4f);
            }
            currentPressed_triggerR=obj.ReadValue<float>();
        }
        // Debug.Log("Trigger Pressed " + obj.ReadValue<float>());
    }
    [PunRPC]
    public void PhotonSetFloatR(string name, float value){
        animR.SetFloat(name,value);
    }
    [PunRPC]
    public void PhotonSetFloatL(string name, float value){
        animL.SetFloat(name,value);
    }

    private void GripPressed(InputAction.CallbackContext obj)
    {
        // DebugUIManager.instance.ShowDebugUIMessage(obj.ReadValue<float>().ToString());
        if(WhichHand == "Left"){
            animL.SetTrigger("Fist");
            // LeftHandAnimator.SetFloat("grip",obj.ReadValue<float>());
            // if(obj.ReadValue<float>()>0.5){
            //     animL.SetTrigger(GrabSmall);
            // }else{
            //     animL.SetTrigger(GrabLarge);
            // }
            if(obj.ReadValue<float>() - currentPressed >0){
                // animL.SetFloat("grab",0.6f);
                View.RPC("PhotonSetFloatL", RpcTarget.AllBuffered,"grab",0.6f);
            }else{
                // animL.SetFloat("grab",0.4f);
                View.RPC("PhotonSetFloatL", RpcTarget.AllBuffered,"grab",0.4f);
            }
            currentPressed=obj.ReadValue<float>();
            // animL.SetFloat("grab",obj.ReadValue<float>());
            
        }else{
            // RightHandAnimator.SetFloat("grip",obj.ReadValue<float>());
            if(obj.ReadValue<float>() - currentPressedR >0){
                // animR.SetFloat("grab",0.6f);
                View.RPC("PhotonSetFloatR", RpcTarget.AllBuffered,"grab",0.6f);
            }else{
                // animR.SetFloat("grab",0.4f);
                View.RPC("PhotonSetFloatR", RpcTarget.AllBuffered,"grab",0.4f);
            }
            currentPressedR=obj.ReadValue<float>();
        }
        // HandAnimator.SetFloat("Grip_"+WhichHand, obj.ReadValue<float>());
        //Debug.Log("Grip Pressed " + obj.ReadValue<float>());

        // if(WhichHand == "Left"){
        //     LeftHandAnimator.SetFloat("grip",obj.ReadValue<float>());
        // }else{
        //     RightHandAnimator.SetFloat("grip",obj.ReadValue<float>());
        // }
    }
    
}
