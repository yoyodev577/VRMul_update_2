using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using System.Text.RegularExpressions;
using Photon.Pun;
using UnityEngine.InputSystem.Controls;


public enum GameState
{
    Default,
    PlayersReady,
    ReadyToStart,
    StartGame,
    EndGame,
    ResetGame
}

public class HoopsGameManager : MonoBehaviour
{
    public static HoopsGameManager _instance;
    private PhotonView _view;
    [SerializeField] private List<HoopsMachine> _machines;
    [SerializeField] private List<PlayerButton> _playerButtons;

    //Questions
    public List<Question> questions;
    [SerializeField] private int currentIndex = 0;
    public Question currentQuestion;

    //Game State
    [SerializeField] private GameState _gameState = GameState.Default;
    public bool isPlayersReady = false;
    public bool IsReadyToStart = false;
    public bool IsGameStart = false;
    public bool IsGameEnd = false;
    public bool IsReset = false;
    public bool IsResetCoroutine = false;

    public bool isPlayer1Win = false;

    /// <summary>
    /// Count Down Timer
    /// </summary>
    public float currentSec = 0f;
    public float timerSec = 3f;
    public bool IsReadyTimerCoroutine = false;

    // Board Panel
    public TMP_Text questionBoard;

    public static List<string> hoopsBasketballTags = new List<string>
    {
        "Basketball",
        "A",
        "B",
        "C",
        "D"
    };

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _audioClip;
    void Start()
    {
        _instance = this;
        _view = GetComponent<PhotonView>();
        _playerButtons = FindObjectsOfType<PlayerButton>().ToList();
       _audioSource = GetComponent<AudioSource>();
        InitQuestions();
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        // when players get ready, the timer starts.
        if (isPlayersReady && IsReadyToStart) {
            StartTimer();
        }

        // after the timer, it start the machines and show questions.
        if (IsGameStart) {
            HoopsStart();
            ShowQuestion();
        }

        // when it is reset,then init the game
        if (IsReset && !IsReadyToStart)
        {
            InitGame();
        }
    }
    void InitGame() {
        _gameState = GameState.Default;
        UpdateBoardText("Press Ready to start the game.");
    }



    private void InitQuestions()
    {
        questions.Add(new Question(
            "What are not major disinfectants for lab disinfection?\n" 
            +"A. Hypochlorites\n" +"B. Formaldehyde\n" +"C. Xylene"
            , "C"));


        questions.Add(new Question(
            "Which one is not belong to common behavior that can be exposed to bloodborne pathogens?\n" 
            +"A. Splashes to blood\n" +"B. Contact of eyes\n" 
            +"C. Bites and knife wounds\n" +"D. Shake hands"
            , "D"));


        questions.Add(new Question("Which communicable disease is NOT found to be common?\n" +
                "A. Cold\n" +
                "B. Hepatitis A\n" +
                "C. Chickenpox\n" +
                "D. Ebola virus disease\n" +
                "E. Pink Eye"
                ,"D"));
    }


    //Function for Ready Button
    public void HoopsPlayerReady() {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonHoopsPlayerReady", RpcTarget.AllBuffered);

        //PhotonHoopsPlayerReady();
    }

    [PunRPC]
    private void PhotonHoopsPlayerReady() {
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

    //Function for Start Button
    public void HoopsReadyToStart()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonHoopsReadyToStart", RpcTarget.AllBuffered);

        //PhotonHoopsReadyToStart();
    }

    [PunRPC]
    private void PhotonHoopsReadyToStart()
    {
        Debug.Log("---Game Ready To Start---");
        if (isPlayersReady && !IsReadyToStart) {
            if (_playerButtons[0].isPressed && _playerButtons[1].isPressed)
            {
                IsReadyToStart = true;
                _gameState = GameState.ReadyToStart;
            }
        }
    }


    public void HoopsStart()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonHoopsStart", RpcTarget.AllBuffered);

    }

    [PunRPC]
    private void PhotonHoopsStart()
    {
        Debug.Log("---Start the game---");

        _gameState = GameState.StartGame;

        foreach (HoopsMachine m in _machines) {
            m.m_Struct.gate.gameObject.SetActive(false);
        }
    }
    public void HoopsReset()
    {
        if(PhotonNetwork.IsConnected)
        _view.RPC("PhotonHoopsReset", RpcTarget.AllBuffered);

    }

    [PunRPC]
    private void PhotonHoopsReset()
    {
        Debug.Log("---Game Reset---");
        foreach (HoopsMachine m in _machines)
        {
            m.BallReset();
            m.SetGate(true);
            m.ResetScore();
        }
        currentIndex = 0;
        currentQuestion = null;

        isPlayersReady = false;
        IsReadyToStart = false;
        IsGameStart = false;
        IsGameEnd = false;
        isPlayer1Win = false;

        IsReset = true;

        if (!IsResetCoroutine)
            StartCoroutine(ResetCoroutine());

        UpdateBoardText("Press Ready to start the game.");
    }


    public void StartTimer() {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonStartTimer", RpcTarget.AllBuffered);

    }

    [PunRPC]
    private void PhotonStartTimer()
    {
        if(!IsReadyTimerCoroutine)
        StartCoroutine(SetReadyTimerCoroutine(timerSec));
    }


    private void ShowResult()
    {
        isPlayer1Win = _machines[0].GetScore() > _machines[1].GetScore() ? true : false;

        questionBoard.text = "The game has ended.\n" +
                        "Player 1 Score:" + _machines[0].GetScore() +"\n" +
                        "Player 2 Score: " + _machines[1].GetScore() +"\n";


        if (isPlayer1Win)
            questionBoard.text += "Player 1 Wins";
        else
            questionBoard.text += "Player 2 Wins";

    }

    private void UpdateBoardText(string text) { 
        questionBoard.text= text;
    
    }
    public void UpdateQuestionBoard(int playerNumber)
    {
        questionBoard.text = "Player " + playerNumber + 
            " has got it correct. The correct answer is " + questions[currentIndex].answerText;

        StartCoroutine(SetQuestionBoardCoroutine());
    }

    private void ShowQuestion()
    {
        currentQuestion = questions[currentIndex];
        questionBoard.text = "Question " + currentIndex + ":\n" + questions[currentIndex].questionText;
    }

    IEnumerator SetQuestionBoardCoroutine()
    {
        yield return new WaitForSeconds(2f);
        currentIndex += 1;
        if (currentIndex > questions.Count)
        {
            IsGameEnd = true;
            ShowResult();
        }
        else
        {
            ShowQuestion();
        }
    }

    // Enable Ready Timer
    IEnumerator SetReadyTimerCoroutine(float seconds) {

        IsReadyTimerCoroutine = true;
        currentSec = seconds;
        UpdateBoardText(currentSec.ToString());

        while (currentSec >= 0)
        {
            _audioSource.PlayOneShot(_audioClip);
            UpdateBoardText(currentSec.ToString());
            yield return new WaitForSeconds(1f);
            currentSec -= 1;
        }

        if (currentSec <= 0)
        {
            _audioSource.Stop();
            IsReadyToStart = false;
            IsGameStart = true;
        }
        yield return null;
        IsReadyTimerCoroutine = false;

    }

    IEnumerator ResetCoroutine() {
        IsResetCoroutine = true;
        yield return new WaitForSeconds(2f);
        IsReset = false;
        IsResetCoroutine = false;
    }

}
