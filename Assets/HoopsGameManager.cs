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
    ReadyToStart,
    StartGame,
    EndGame,
    ResetGame
}

public class HoopsGameManager : MonoBehaviour
{
    public static HoopsGameManager _instance;
    [SerializeField] private GameState _gameState = GameState.Default;
    private PhotonView _view;
    [SerializeField] private List<HoopsMachine> _machines;
    [SerializeField] private List<PlayerButton> _playerButtons;


    public List<Question> questions;
    [SerializeField] private int currentIndex = 0;

    [HideInInspector] public int[] allScores;
    public static bool ScoreResetBool2 = false;

    public bool IsReadyToStart = false;
    public bool IsGameStart = false;
    public bool IsGameEnd = false;

    public float currentSec = 0f;
    public float timerSec = 3f;
    public bool isCountDown = false;
    public bool IsReadyTimerCoroutine = false;

    public TMP_Text questionBoard;

    public static List<string> hoopsBasketballTags = new List<string>
    {
        "Basketball",
        "A",
        "B",
        "C",
        "D"
    };
    void Start()
    {
        _instance = this;
        _view = GetComponent<PhotonView>();
        _playerButtons = FindObjectsOfType<PlayerButton>().ToList();
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {

        if (!IsReadyToStart) { 
            foreach(PlayerButton button in _playerButtons)
            {
                if (button.isPressed)
                {
                    IsReadyToStart = true; break;
                }
            }

        }

        if (IsReadyToStart && !IsReadyTimerCoroutine) {
            //StartCoroutine(SetReadyTimer(timerSec));
        }

        if (IsGameStart) {
            //HoopsStart();
           // ShowQuestion();
            
        }

  /*      if(TableButton.ScoreResetBool){
            ResetScoreAndQuestion();
            
            
        }
        if (TableButton.gameStarted && currentIndex == 0)
        {
            currentIndex += 1;
            ShowQuestion();
        }*/
    }
    void InitGame() {
        QuestionBankInitiation();
        ResetScoreAndQuestion();
        allScores = new int[2];

        IsReadyToStart = false;
        IsGameStart = false;
        IsGameEnd = false;
        _gameState = GameState.Default;
    }

    private void QuestionBankInitiation()
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


    public void HoopsReady()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonHoopsReady", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void PhotonHoopsReady()
    {
        _gameState = GameState.ReadyToStart;
        //playerReady[playerNumber - 1] = !playerReady[playerNumber - 1];
    }


    public void HoopsStart()
    {
        if (PhotonNetwork.IsConnected)
            _view.RPC("PhotonHoopsStart", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void PhotonHoopsStart()
    {
        _gameState = GameState.StartGame;

        foreach (HoopsMachine m in _machines) {
            m.m_Struct.gate.gameObject.SetActive(false);
        }
        IsGameStart = true;
        /*if (CheckAllPlayersReady(playerReady))          //All players ready, starts all hoops machines
        {

            for (int i = 1; i <= machineCount; i++)
            {
                //MachineParts[i][HoopsMachineParts.Gate].gameObject.SetActive(false);
            }
            gameStarted = true;
            ScoreReset();
        }
        //Resets static booleans
        for (int i = 0; i < playerReady.Length; i++)
        {
            playerReady[i] = false;
        }
        allPlayersReady = false;*/
    }
    public void HoopsReset()
    {
        _view.RPC("PhotonHoopsReset", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void PhotonHoopsReset()
    {
        /*if (playerNumber == 0)
        {
            for (int i = 0; i < machineCount; i++)
            {
                //BallReset(i + 1, true);
                playerReady[i] = false;
            }
            ScoreReset();
            gameStarted = false;
            StartCoroutine(StartButtonFlash());
        }
        else
        {
            //BallReset(playerNumber, false);
        }*/
    }
    private void ScoreReset()
    {
        //ScoreResetBool = true;
    }

    public void ResetGame()
    {
        ResetScoreAndQuestion();
    }


    private void ResetScoreAndQuestion()
    {
        for (int i = 0; i < allScores.Length; i++)
        {
            allScores[i] = 0;
            // DebugUIManager.instance.ShowDebugUIMessage(allScores[i].ToString());
        }
        currentIndex = 0;
        UpdateText("Press Ready to start the game.");
        ScoreResetBool2 = true;
    }

    private void ShowResult()
    {
        int playerNumberOffset = 1;
        questionBoard.text =
            "The game has ended.\n" +
            "Player 1 Score: " + allScores[0] + "\n" +
            "Player 2 Score: " + allScores[1] + "\n" +
            "Player " + ((Array.IndexOf(allScores, allScores.Max())) + playerNumberOffset).ToString() + " wins!";

    }

    private bool CheckAllPlayersReady(bool[] checklist)
    {
        bool allTrue = false;
        foreach (bool ready in checklist)
        {
            if (ready)
            {
                allTrue = true;
            }
            else
            {
                allTrue = false;
                break;
            }
        }
        return allTrue;
    }
    public void UpdateText(string text) { 
        questionBoard.text= text;
    
    }

    public void QuestionBoardCorrect(int playerNumber)
    {
        questionBoard.text = "Player " + playerNumber + " has got it correct. The correct answer is " + questions[currentIndex].answerText;
        StartCoroutine(UpdateTextBoard());
    }

    private void ShowQuestion()
    {
        questionBoard.text = "Question " + currentIndex + ":\n" + questions[currentIndex].questionText;
    }


    IEnumerator UpdateTextBoard()
    {
        yield return new WaitForSeconds(2f);
        currentIndex += 1;
        if (currentIndex > questions.Count)
        {
            ShowResult();
        }
        else
        {
            ShowQuestion();
        }
    }

    IEnumerator SetReadyTimer(float seconds) {
        IsReadyTimerCoroutine = true;
        currentSec = seconds;

        while (currentSec >= 0)
        {
            UpdateText(currentSec.ToString());
            yield return new WaitForSeconds(1f);
            currentSec -= 1;
        }

        if (currentSec <= 0)
        {
            yield return null;
            IsReadyTimerCoroutine = false;
            IsReadyToStart = false;
            IsGameStart = true;
        }


    }

}
