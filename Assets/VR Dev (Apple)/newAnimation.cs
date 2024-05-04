using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
[RequireComponent(typeof(ActionBasedController))]
public class newAnimation : MonoBehaviour
{
    ActionBasedController controller;
    public NewHand hand;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();
    }

    // Update is called once per frame
    void Update()
    {
        // float a = controller.activateAction.action.ReadValue<float>();
        hand.SetGrip(controller.activateAction.action.ReadValue<float>());
        // DebugUIManager.instance.ShowDebugUIMessage(a.ToString());
    }

}
