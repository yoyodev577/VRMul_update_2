using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MultispinGameManager : MonoBehaviour
{
    public static MultispinGameManager instance;
    private PhotonView _view;

    [SerializeField] private List<PlayerButton> _playerButtons;
    [SerializeField] private List<MultiSpin> _multiSpins;

    [SerializeField] 
    private GameState _gameState = GameState.Default;
    public bool isPlayersReady = false;
    public bool IsReadyToStart = false;
    public bool IsGameStart = false;
    public bool IsGameEnd = false;
    public bool IsReset = false;
    public bool IsResetCoroutine = false;

    public float currentSec = 0f;
    public float timerSec = 3f;
    public bool IsReadyTimerCoroutine = false;

    public TMP_Text uiBoard;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        _view = GetComponent<PhotonView>();
        _playerButtons = FindObjectsOfType<PlayerButton>().ToList();
        _multiSpins = FindObjectsOfType<MultiSpin>().ToList();
        _audioSource = GetComponent<AudioSource>();
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        // when players get ready, the timer starts.
        if (isPlayersReady && IsReadyToStart && !IsReadyTimerCoroutine)
        {
            StartCoroutine(SetReadyTimerCoroutine(timerSec));
        }
        if (IsGameStart)
        {
            StartGame();
        }


    }

    void InitGame() {

        UpdateBoardText("Press Ready to start the game.");
    }

    public void WaitForPlayersReady()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonWaitForPlayersReady", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PhotonWaitForPlayersReady() {
        Debug.Log("---Waiting For Players Ready---");
        foreach (PlayerButton button in _playerButtons)
        {
            if (button.isPressed)
            {
                isPlayersReady = true;
                _gameState = GameState.PlayersReady;
            }
        }
    }

    public void ReadyToStart()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonReadyToStart", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PhotonReadyToStart()
    {
        Debug.Log("---Game Ready To Start---");
        if (isPlayersReady && !IsReadyToStart)
        {
            if (_playerButtons[0].isPressed && _playerButtons[1].isPressed)
            {
                IsReadyToStart = true;
                _gameState = GameState.ReadyToStart;
            }
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonStartGame", RpcTarget.AllBuffered);

    }

    [PunRPC]
    private void PhotonStartGame()
    {
        Debug.Log("---Start the game---");
        UpdateBoardText("Game Starts");

        _gameState = GameState.StartGame;
        IsGameStart = true;

    }

    public void EndGame()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonEndGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PhotonEndGame() {


        _gameState = GameState.EndGame;
        IsGameEnd = true;
        ShowResult();
    }



    public void ResetGame()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonResetGame", RpcTarget.AllBuffered);

    }

    [PunRPC]
    private void PhotonResetGame()
    {
        Debug.Log("---Game Reset---");
        if(!IsResetCoroutine)
            StartCoroutine(ResetCoroutine());
    }

    private void ShowResult() {
        string text = "";

        if (_multiSpins[0].isBalanced && !_multiSpins[1].isBalanced)
        {
            text = "The game has ended.\nPlayer :" + _multiSpins[0].playerNum + " wins";

        }
        else if (!_multiSpins[0].isBalanced && _multiSpins[1].isBalanced) {

            text = "The game has ended.\nPlayer :" + _multiSpins[1].playerNum + " wins";
        }
        else if (_multiSpins[0].isBalanced && _multiSpins[1].isBalanced)
        {
            text = "The game has ended.Both players win!";
        }
        else if (!_multiSpins[0].isBalanced && !_multiSpins[1].isBalanced)
        {
            text = "The game has ended.Both players lose :(!";
        }


        UpdateBoardText(text);
    }


    private void UpdateBoardText(string text)
    {
        uiBoard.text = text;

    }

    IEnumerator SetReadyTimerCoroutine(float seconds)
    {
        IsReadyTimerCoroutine = true;
        currentSec = seconds;

        while (currentSec >= 0)
        {
            UpdateBoardText(currentSec.ToString());
            yield return new WaitForSeconds(1f);
            currentSec -= 1;
            _audioSource.PlayOneShot(_audioClip);
        }

        if (currentSec <= 0)
        {
            yield return null;
            IsReadyTimerCoroutine = false;
            IsReadyToStart = false;
            IsGameStart = true;
        }


    }

    IEnumerator ResetCoroutine()
    {
        IsResetCoroutine = true;
        isPlayersReady = false;
        IsReadyToStart = false;
        IsGameStart = false;
        IsGameEnd = false;
        IsReadyTimerCoroutine = false;
        foreach(MultiSpin m in _multiSpins)
        {
            m.ResetMultiSpin(); 
        }

        InitGame();
        yield return new WaitForSeconds(1f);
        IsReset = false;
        IsResetCoroutine = false;
    }

}
