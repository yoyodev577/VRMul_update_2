using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class NewHand : MonoBehaviour
{
    public float speed;
    Animator animator;
    private float gripTarget;
    private float gripCurrent;

    // [SerializeField] InputAction gripInputAction;
    // [SerializeField] InputAction triggerInputAction;
    // private void Awake()
    // {
    //     gripInputAction.performed += GripPressed;
    //     triggerInputAction.performed += GripPressed; 
    // }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        AnimateHand();
    }
    // private void GripPressed(InputAction.CallbackContext obj)
    // {
    //     animator.SetFloat("grip", obj.ReadValue<float>());
    //     //Debug.Log("Grip Pressed " + obj.ReadValue<float>());
    // }
    internal void SetGrip(float v){
        // DebugUIManager.instance.ShowDebugUIMessage("click");
        gripTarget =v;
        // DebugUIManager.instance.ShowDebugUIMessage(v.ToString());
    }

    void AnimateHand(){
        if(gripCurrent != gripTarget){
            gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime*speed);
            animator.SetFloat("grip",gripCurrent);
        }
        animator.SetFloat("grip",0.5f);
    }
}
