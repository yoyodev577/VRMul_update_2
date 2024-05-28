using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using Photon.Pun;
using System.Linq;
public class GunGameManager : MonoBehaviour
{
    public static GunGameManager instance;
    private PhotonView View;

    [SerializeField] private AudioClip countSound;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioSource source;

    [SerializeField] private List<PlayerButton> _playerButtons;
    [SerializeField] private GameState _gameState = GameState.Default;
    public bool isPlayersReady = false;
    public bool IsReadyToStart = false;
    public bool IsGameStart = false;
    public bool IsGameEnd = false;
    public bool IsReset = false;
    public bool IsResetCoroutine = false;

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
    

    private int play1Score =0;
    private int play2Score =0;

    private string[] Question = {
        "Question 1: The ans is A :)",
        "Question 2: The ans is B",
        "Question 3: The ans is C",
        "Question 4: The ans is D",
    };

    private string[] Answer = 
    {
        "A",
        "B",
        "C",
        "D",

    };


    // Start is called before the first frame update
    void Start()
    {   
        View = this.gameObject.GetComponent<PhotonView>();
        source = this.gameObject.GetComponent<AudioSource>();
        _playerButtons = FindObjectsOfType<PlayerButton>().ToList();
        gunGrabOK = false;
        gunGameStart = false;
        
    }

    // Update is called once per frame
    void Update()
    {   
        if(PhotonNetwork.IsConnected)
        View.RPC("PhotonUpdate", RpcTarget.AllBuffered);
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
        }
    }


    [PunRPC]
    public void PhotonUpdate(){
         //lock the position of the gun
        if (!gunGrabOK){
            ResetTheGun();
        }
        playerReady1 = player1.ReadyValue;
        playerReady2 = player2.ReadyValue;
        if (playerReady1 && playerReady2){
            CountDownForStartGunGame();
        }
        
        // reset the game
        if (reset.NeedReset){
            reset.NeedReset = false;
            ResetTheGun();
            resetGame();
            
            
        }
    }

    public void resetGame(){
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

    public void CountDownForStartGunGame(){
        if(!gunGameStart){
            gunGameStart = true;
            CountDownCanva.SetActive(true);
            StartCoroutine(coroutineForCountDown());
        }
        // View.RPC("PhotonCountDownForStartGunGame", RpcTarget.AllBuffered);
    }
    // [PunRPC]
    // public void PhotonCountDownForStartGunGame(){
    //     // if(!gunGameStart){
    //     //     gunGameStart = true;
    //     //     CountDownCanva.SetActive(true);
    //     //     StartCoroutine(coroutineForCountDown());
    //     // }
    // }

    IEnumerator coroutineForCountDown(){
        for (int i = 3; i>=0; i--){
            CountDonwText.text = i.ToString();
            source.PlayOneShot(countSound);
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
        resetGame();



    }


    string answer = null;
    bool roundEnd = true;
    int roundWinner = -1; // 0 = player1 1 = player2
    bool updateWinner = false;
    public void checkAnswer1(string playerAns){
        View.RPC("PhotonCheckAnswer1", RpcTarget.AllBuffered,playerAns);
    }
    [PunRPC]
    public void PhotonCheckAnswer1(string playerAns){
        if(playerAns == answer){
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
        QuestionBoardText.text = Question[index];
        answer = Answer[index];
        roundEnd = false;
        updateWinner = true;
        while(!roundEnd){
             yield return StartCoroutine(waitForCorrectAns());
        }
        if(roundWinner == 0){
            source.PlayOneShot(correctSound);
            notiPlayer1.SetActive(true);
            notiPlayer2_2.SetActive(true);
            QuestionBoardText.text = "Player1 got the correct answer.";
        }else if(roundWinner == 1){
            source.PlayOneShot(correctSound);
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
  
}
