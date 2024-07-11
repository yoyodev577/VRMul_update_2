using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using Photon.Pun;
using System.Linq;
using static RootMotion.FinalIK.GrounderQuadruped;
public class GunGameManager : MonoBehaviour
{
    public static GunGameManager instance;
    private PhotonView View;

    [SerializeField] private AudioClip countSound;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<Shoot> _playerShoots;
    [SerializeField] private List<PlayerButton> _playerButtons;
    [SerializeField] private GameState _gameState = GameState.Default;
    [SerializeField] private int score_0, score_1 = 0;

    public bool isPlayersReady = false;
    public bool isReadyToStart = false;
    public bool isGameStart = false;
    public bool isGameEnd = false;
    public bool isUpdateScore = false;
    public bool isRoundStart = false;
    public bool isRoundEnd = false;
    public bool isReset = false;

    public bool isGameCoroutine= false;
    public bool isResetCoroutine = false;

    public float currentSec = 0f;
    public float timerSec = 3f;
    public bool IsReadyTimerCoroutine = false;

    [SerializeField] private List<Question> questions;
    [SerializeField] public Question currentQuestion;
    [SerializeField] private int currentIndex = 0;


    //reference of player1
    public GunGameButton player1;
    private  bool playerReady1; 
    public GameObject gunPlayer1;
    public GameObject notiPlayer1;
    public GameObject notiPlayer1_2;

    //referenece of player2
    public GunGameButton player2;
    private  bool playerReady2; 
    public GameObject gunPlayer2;
    public GameObject notiPlayer2;
    public GameObject notiPlayer2_2;




    // game control variable
    public TMP_Text CountDonwText;
    public GameObject CountDownCanva;
    public bool gunGameStart;
    public bool gunGrabOK; // to lock the gun before end of the count down
    public GunGameButton reset;
    public TMP_Text QuestionBoardText;
    public TMP_Text ResultText;
    public TMP_Text ShootingScore1;
    public TMP_Text ShootingScore2;

    public TMP_Text uiBoard;


    private int play1Score =0;
    private int play2Score =0;

    private string resultText;

    // Start is called before the first frame update
    void Start()
    {   
        View = this.gameObject.GetComponent<PhotonView>();
        audioSource = this.gameObject.GetComponent<AudioSource>();
        _playerButtons = FindObjectsOfType<PlayerButton>().ToList();
        _playerShoots = FindObjectsOfType<Shoot>().ToList();
        gunGrabOK = false;
        gunGameStart = false;
        InitQuestions();

        InitGame();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsConnected)
        View.RPC("PhotonUpdate", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void PhotonUpdate() {

        if (isPlayersReady && isReadyToStart)
        {
            StartTimer();
        }

        if (!isGameStart && isReadyToStart && !isGameEnd)
        {
            StartGame();
        }

        if (isGameStart && !isGameEnd)
        {
            for (int i = 0; i < _playerShoots.Count; i++)
            {
                if (_playerShoots[i].isUpdatedScore)
                {
                    isRoundEnd = true;
                }
            }

            if (isRoundEnd && currentIndex == questions.Count - 1)
            {
                EndGame();
            }


            if (isRoundEnd)
            {
                if (!isGameCoroutine)
                {
                    StartCoroutine(SetGameCoroutine());
                }
            }
        }

        if (isGameEnd)
        {
            //ShowResult
            ShowResult();
            Debug.Log("--Show Result---");
        }

    }

    void InitGame()
    {
        _gameState = GameState.Default;
        UpdateBoardText("Press Ready to start the game.");
    }

    void InitQuestions()
    {
        questions.Add(new Question("Question 1: The ans is A :)"
            , "A"
            ));
        questions.Add(new Question("Question 2: The ans is B :)"
         , "B"
         ));
        questions.Add(new Question("Question 3: The ans is C :)"
         , "C"
         ));
        questions.Add(new Question("Question 4: The ans is D :)"
         , "D"
         ));

    }

    public void WaitForPlayersReady()
    {
        if (PhotonNetwork.IsConnected)
            View.RPC("PhotonWaitForPlayersReady", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PhotonWaitForPlayersReady()
    {
        Debug.Log("---Waiting For Players Ready---");
        foreach (PlayerButton button in _playerButtons)
        {
            if (button.isPressed)
            {
                isPlayersReady = true;
                _gameState = GameState.PlayersReady;
            }
            else {
                isPlayersReady = false;
                _gameState = GameState.Default;
            }
        }
    }

    //Function for Start Button
    public void ReadyToStart()
    {
        if (PhotonNetwork.IsConnected)
            View.RPC("PhotonReadyToStart", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PhotonReadyToStart()
    {
        Debug.Log("---Game Ready To Start---");
        if (isPlayersReady && !isReadyToStart)
        {
            if (_playerButtons[0].isPressed && _playerButtons[1].isPressed)
            {
                isReadyToStart = true;
                _gameState = GameState.ReadyToStart;
            }
        }
    }
    

    public void StartGame()
    {
        if (PhotonNetwork.IsConnected)
            View.RPC("PhotonStartGame", RpcTarget.AllBuffered);

    }
    [PunRPC]
    private void PhotonStartGame()
    {
        Debug.Log("---Start the game---");

        _gameState = GameState.StartGame;
        isGameStart = true;
        gunGrabOK = true;

        foreach(Shoot shoot in _playerShoots)
        {
            shoot.ShowRedPoint();
        }
    }


    public void EndGame()
    {
        if (PhotonNetwork.IsConnected)
            View.RPC("PhotonEndGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void PhotonEndGame()
    {

        _gameState = GameState.EndGame;
        isGameEnd = true;
    }

    public void ResetGame() {
        if (PhotonNetwork.IsConnected)
            View.RPC("PhotonResetGame", RpcTarget.AllBuffered);

    }


    [PunRPC]
    private void PhotonResetGame()
    {
        _gameState = GameState.ResetGame;
        isGameEnd = false;
        isGameStart = false;
        isPlayersReady = false;
        isReadyToStart = false;
        isUpdateScore = false;
        isGameCoroutine = false;
        IsReadyTimerCoroutine = false;
        isRoundEnd = false;

        currentIndex = 0;
        resultText = string.Empty;

        for(int i = 0; i < _playerShoots.Count; i++)
        {
            _playerShoots[i].OnReset();
        }

        for(int i = 0; i < _playerButtons.Count;i++)
        {
            _playerButtons[i].ResetButton();
        }

        isReset = true;

        if (!isResetCoroutine)
            StartCoroutine(ResetCoroutine());

        InitGame();
    }



    public void StartTimer()
    {
        if (PhotonNetwork.IsConnected)
            View.RPC("PhotonStartTimer", RpcTarget.AllBuffered);

    }

    [PunRPC]
    private void PhotonStartTimer()
    {
        if (!IsReadyTimerCoroutine)
            StartCoroutine(SetReadyTimerCoroutine(timerSec));
    }

    private void UpdateBoardText(string text)
    {
        uiBoard.text = text;

    }

    public bool IsRoundEnd() { 
        if(_playerShoots.Count == 0 || !isRoundStart || isRoundEnd) return false;

        foreach(Shoot shoot in _playerShoots)
        {
            if (shoot.isUpdatedScore)
            {
                return true;
            }
            else {
                return false;
            }
        }
        return false;
    }

    public void ShowResult() {

        Debug.Log("---Update the shooting score---");
        if (_playerShoots.Count > 0)
        {
            if (_playerShoots[0].score > _playerShoots[1].score)
            {
                resultText = "Game Ends.\nPlayer" +_playerShoots[0].playerNum+ " wins";
            }
            else if (_playerShoots[0].score == _playerShoots[1].score)
            {
                resultText = "Game Ends.The game is fair";
            }
            else if (_playerShoots[0].score < _playerShoots[1].score)
            {
                resultText = "Game Ends.\nPlayer"+ _playerShoots[1].playerNum + " wins";
            }


        }
        UpdateBoardText(resultText);
    }



    IEnumerator SetGameCoroutine()
    {
        Debug.Log("---Start Game Coroutine");
        isGameCoroutine = true;
        if (currentIndex < questions.Count)
        {
            if (isRoundEnd)
            {
                Debug.Log("---Next Round---");
                for (int i = 0; i < _playerShoots.Count; i++)
                {
                    _playerShoots[i].isUpdatedScore = false;
                    _playerShoots[i].DisableAllPanels();
                }
                if(currentIndex < questions.Count - 1)
                currentIndex++;
                isRoundEnd = false;
            }

            Debug.Log("---Start Question---" + currentIndex);

            currentQuestion = questions[currentIndex];
            UpdateBoardText(currentQuestion.questionText);

            yield return new WaitForSeconds(5);
        }
        isGameCoroutine = false;
    }


    IEnumerator SetReadyTimerCoroutine(float seconds)
    {
        IsReadyTimerCoroutine = true;
        currentSec = seconds;
        UpdateBoardText(currentSec.ToString());

        while (currentSec >= 0)
        {
            audioSource.PlayOneShot(countSound);
            UpdateBoardText(currentSec.ToString());
            yield return new WaitForSeconds(1f);
            currentSec -= 1;
        }

        if (currentSec <= 0)
        {
            audioSource.Stop();
            isReadyToStart = false;
            isGameStart = true;
            UpdateBoardText("Game Starts");
        }
       
        IsReadyTimerCoroutine = false;
        yield return StartCoroutine(SetGameCoroutine());
    }





   /* [PunRPC]
    public void PhotonUpdate(){
         //lock the position of the gun
        if (!gunGrabOK){
            ResetTheGun();
        }
        playerReady1 = player1.ReadyValue;
        playerReady2 = player2.ReadyValue;
        if (playerReady1 && playerReady2){
            //CountDownForStartGunGame();
        }
        
        // reset the game
        if (reset.NeedReset){
            reset.NeedReset = false;
            ResetTheGun();
            ResetGame2();
            
            
        }
    }*/

    public void ResetGame2(){
        notiPlayer1.SetActive(false);
        notiPlayer2_2.SetActive(false);
        notiPlayer2.SetActive(false);
        notiPlayer1_2.SetActive(false);
        ShootingScore1.text = "0";
        ShootingScore2.text = "0";
        QuestionBoardText.text ="Press ready button to start the game.";
        player1.ReadyValue =false;
        player2.ReadyValue =false;
        player1.transform.GetChild(0).GetComponent<MeshRenderer>().material = player1.buttonLight[0];
        player2.transform.GetChild(0).GetComponent<MeshRenderer>().material = player2.buttonLight[0];
        gunGameStart = false;
        // View.RPC("PhotonResetGame", RpcTarget.AllBuffered);
    }


    
    public void ResetTheGun(){
        gunPlayer1.gameObject.transform.position = new Vector3(80.620277f,1.130567f,25.880285f);
        gunPlayer1.gameObject.transform.rotation = new Quaternion(0,0,0,0);
        gunPlayer2.gameObject.transform.position = new Vector3(75.880279f,1.130567f,25.880285f);
        gunPlayer2.gameObject.transform.rotation = new Quaternion(0,0,0,0);
   
        // View.RPC("PhotonResetTheGun", RpcTarget.AllBuffered);

    }
/*
    public void CountDownForStartGunGame(){
        if(!gunGameStart){
            gunGameStart = true;
            CountDownCanva.SetActive(true);
            StartCoroutine(coroutineForCountDown());
        }
        // View.RPC("PhotonCountDownForStartGunGame", RpcTarget.AllBuffered);
    }*/
/*
    IEnumerator coroutineForCountDown(){
        for (int i = 3; i>=0; i--){
            CountDonwText.text = i.ToString();
            audioSource.PlayOneShot(countSound);
            yield return new WaitForSeconds(1f);
        }
        CountDownCanva.SetActive(false);
        gunGrabOK = true;
        ResultText.text = "";
        // gunPlayer1.gameObject.SetActive(true);
        // gunPlayer2.gameObject.SetActive(true);
        //the game started 
        for (int i = 0; i<=3;i++){
            yield return StartCoroutine(StartQuestion(i));
        }
        play1Score = int.Parse(ShootingScore1.text);
        play2Score = int.Parse(ShootingScore2.text);

        if(play1Score >play2Score){
            QuestionBoardText.text = "";
            ResultText.text ="Player1 wins!";
        }else if(play2Score>play1Score){
            QuestionBoardText.text = "";
            ResultText.text ="Player2 wins!";
        }else{
            QuestionBoardText.text = "";
            ResultText.text ="Player2 wins!";
        }
        yield return new WaitForSeconds(5f);
        gunGrabOK = false;
        ResetGame();

    }*/


    string answer = null;
    bool roundEnd = true;
    int roundWinner = -1; // 0 = player1 1 = player2
    bool updateWinner = false;
    public void checkAnswer1(string playerAns){
        View.RPC("PhotonCheckAnswer1", RpcTarget.AllBuffered,playerAns);
    }
    [PunRPC]
    public void PhotonCheckAnswer1(string playerAns2){
        if(playerAns2 == answer){
            roundEnd = true;
            if(updateWinner){
                updateWinner = false;
                roundWinner = 0;
                ShootingScore1.text = (int.Parse(ShootingScore1.text)+1).ToString();
            }
            
        }

    }

    public void checkAnswer2(string playerAns){
        View.RPC("PhotonCheckAnswer2", RpcTarget.AllBuffered,playerAns);
    }
    [PunRPC]
    public void PhotonCheckAnswer2(string playerAns){
        if(playerAns == answer){
            roundEnd = true;
            if(updateWinner){
                updateWinner = false;
                roundWinner = 1;
                ShootingScore2.text = (int.Parse(ShootingScore2.text)+1).ToString();
            }
        }
    }
   
    IEnumerator StartQuestion(int index){
        notiPlayer1.SetActive(false);
        notiPlayer2_2.SetActive(false);
        notiPlayer2.SetActive(false);
        notiPlayer1_2.SetActive(false);
        //QuestionBoardText.text = Question[index];
        //answer = Answer[index];
        roundEnd = false;
        updateWinner = true;
        while(!roundEnd){
             yield return StartCoroutine(waitForCorrectAns());
        }
        if(roundWinner == 0){
            audioSource.PlayOneShot(correctSound);
            notiPlayer1.SetActive(true);
            notiPlayer2_2.SetActive(true);
            QuestionBoardText.text = "Player1 got the correct answer.";
        }else if(roundWinner == 1){
            audioSource.PlayOneShot(correctSound);
            notiPlayer2.SetActive(true);
            notiPlayer1_2.SetActive(true);
            QuestionBoardText.text = "Player2 got the correct answer.";
        }
        // QuestionBoardText.text = roundWinner.ToString();

        yield return new WaitForSeconds(2f);
        
    }

    IEnumerator waitForCorrectAns(){
        yield return null;
    }


    IEnumerator ResetCoroutine()
    {
        isResetCoroutine = true;
        yield return new WaitForSeconds(2f);
        isReset = false;
        isResetCoroutine = false;
    }
}
