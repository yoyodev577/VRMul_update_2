using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using System.Text.RegularExpressions;

public class TableButton : MonoBehaviour
{
    /// <summary>
    /// This script manages all buttons for the hoops machine.
    /// It uses names of child gameObjects in hoops machine for reference.
    /// Do not change any names of the child gameObjects under hoops machine or buttons,
    /// or you will have to refactor the script below.
    /// Written by Viva on 11/07/2023
    /// </summary>

    #region Variable declaration
    //Variables for scene referencing
    public UnityEvent onPressed, onReleased;
    private PhotonView _view;

    //Variables for button audio
    public AudioSource sound;
   
    //Variables for button movement
    public bool isPressed;
    private Vector3 startPos;
    private ConfigurableJoint joint;

    //Variables for flashing buttons
    private MeshRenderer buttonRenderer;
    private int buttonMaterialIndex;
    [SerializeField]
    private List<Material> buttonMaterials;

    //Variables for detecting button collision (can change default value to alter button behaviour)
    [SerializeField] private float threshold = .1f;
    [SerializeField] private float deadZone = .025f;

    [SerializeField] private bool IsButtonCoroutine = false;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    public virtual void Start()
    {
        _view = GetComponent<PhotonView>();
        sound = this.transform.GetChild(0).GetComponent<AudioSource>();
        joint = GetComponent<ConfigurableJoint>();
        buttonRenderer = this.transform.GetChild(0).GetComponent<MeshRenderer>();
        if (buttonRenderer && buttonMaterials.Count > 0)
        {
            buttonRenderer.material = buttonMaterials[buttonMaterialIndex];
        }
        startPos = transform.localPosition;

        onPressed.AddListener(() => FlashButton(isPressed));
        onPressed.AddListener(()=>sound.Play());
        
    }
    // Update is called on every frame
    public virtual void Update()
    {
        if(!IsButtonCoroutine)
        {
            StartCoroutine(ButtonCoroutine());
        }

        Debug.Log(this.gameObject.name + " IsPressed : " + isPressed);

    }
    #endregion

    #region Button Transform Detection
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
    #endregion

    // recording the action of pressing button within 0.5 seconds
    IEnumerator ButtonCoroutine() {
        IsButtonCoroutine = true;
        if (GetValue() + threshold >= 1f) {
            isPressed = !isPressed;
            onPressed.Invoke();
        }
        if (PhotonNetwork.IsConnected)
            _view.RPC("FlashButton",RpcTarget.AllBuffered,isPressed);

        FlashButton(isPressed);
        yield return new WaitForSeconds(0.5f);
        IsButtonCoroutine = false;
    
    }

    [PunRPC]
    public void FlashButton(bool isPressed) {

        if (buttonMaterials.Count < 1) return;

        if (isPressed)
            buttonRenderer.material = buttonMaterials[1];
        else
            buttonRenderer.material = buttonMaterials[0];
    }

}