using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using IRONHEADGames;


public class HomeSceneHnadAnimation : MonoBehaviour
{

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
        
    
        if(WhichHand == "Left"){
            animL.SetTrigger("Fist");
            
            if(obj.ReadValue<float>() - currentPressed_trigger >0){
                animL.SetFloat("trigger",0.6f);

            }else{
                animL.SetFloat("trigger",0.4f);

            }
            currentPressed_trigger=obj.ReadValue<float>();
          
            
        }else{
           
            if(obj.ReadValue<float>() - currentPressed_triggerR >0){
                animR.SetFloat("trigger",0.6f);
                
            }else{
                animR.SetFloat("trigger",0.4f);
                
            }
            currentPressed_triggerR=obj.ReadValue<float>();
        }
       
    }
   

    private void GripPressed(InputAction.CallbackContext obj)
    {
        
        if(WhichHand == "Left"){
            animL.SetTrigger("Fist");
     
            if(obj.ReadValue<float>() - currentPressed >0){
                animL.SetFloat("grab",0.6f);
               
            }else{
                animL.SetFloat("grab",0.4f);
                
            }
            currentPressed=obj.ReadValue<float>();
          
            
        }else{
     
            if(obj.ReadValue<float>() - currentPressedR >0){
                animR.SetFloat("grab",0.6f);
                
            }else{
                animR.SetFloat("grab",0.4f);
               
            }
            currentPressedR=obj.ReadValue<float>();
        }
       
    }
}
