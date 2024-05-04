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

    //Variables for button audio
    private AudioSource sound;

    //Variables for button movement
    private bool isPressed;
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

    //Variables for setting machine/player number in inspector, used by Start/Reset ball buttons
    //Please set this as 1-index, on Ready and Reset Ball Buttons
    //On Reset Score and Start Button, please set it to 0
    public int playerNumber;

    public static bool ScoreResetBool; //false

    PhotonView View;

    //Static variables for checking player ready status and game status
    public static bool[] playerReady = new bool[2];
    public static bool gameStarted = false;
    private static bool allPlayersReady = false;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        View = GetComponent<PhotonView>();
        sound = this.transform.GetChild(0).GetComponent<AudioSource>();
        joint = GetComponent<ConfigurableJoint>();
        buttonRenderer = this.transform.GetChild(0).GetComponent<MeshRenderer>();
        if (buttonRenderer && buttonMaterials.Count > 0)
        {
            buttonRenderer.material = buttonMaterials[buttonMaterialIndex];
        }
        startPos = transform.localPosition;
        StartCoroutine(StartButtonFlash());
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
        if (this.GetComponentInParent<ReadyButton>() != null)
        {
            ButtonLightSwitch(playerReady[playerNumber-1]);
        }
        if (!gameStarted)
        {
            HoopsGameManager._instance.ResetGame();
        }

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

    /// <summary>
    /// This region stores all events activated by the button press.
    /// Drag the gameObject that contains the script itself (the "press" child in corresponding buttons)
    /// into the onPressed/onReleased events to trigger Hooops Methods.
    /// </summary>
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

    /// <summary>
    /// Used by all Buttons. 
    /// Toggles between materials of the button.
    /// </summary>
    /// <param name="pressed">
    /// Whether the button is pressed or released. Set boolean to true in onPressed and false in onReleased.
    /// </param>
    public void ButtonLightSwitch(bool pressed)
    {
        if (PhotonNetwork.IsConnected)
            View.RPC("PhotonButtonLightSwitch", RpcTarget.AllBuffered,pressed);
    }
    [PunRPC]
    public void PhotonButtonLightSwitch(bool pressed){
        buttonRenderer.material = buttonMaterials[Convert.ToInt32(pressed)];
    }
    #endregion

    IEnumerator StartButtonFlash()
    {
        while (true)        //loops indefinitely
        {
            if (this.GetComponentInParent<StartButton>() != null)                           //only performs for Start Button
            {
                if (allPlayersReady && !gameStarted)                                //Condition: all players ready but game not started 
                {
                    buttonMaterialIndex = (buttonMaterialIndex + 1) % buttonMaterials.Count;
                    buttonRenderer.material = buttonMaterials[buttonMaterialIndex];
                }                                                                   //true: switches light
                else
                {
                    buttonRenderer.material = buttonMaterials[0];
                }                                                                   //false: turns off light
            }
            yield return new WaitForSeconds(0.5f);                              //wait for some time before executing again
        }
    }

}