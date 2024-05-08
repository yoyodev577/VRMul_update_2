using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class HoopsScore : MonoBehaviour
{
    #region Variable declaration
    //Vaiables for score detection 
    [HideInInspector]
    public int score;
    [SerializeField]
    private int playerNumber;
    private bool answerCorrect = false;
    private bool updatingScore = false;

    //Variables for updating scores; for score board only
    public TMP_Text infoBoard;      //drag itself (Text (TMP)) into the field
    public HoopsGameManager gameManager;

    PhotonView View;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        View = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
    /*    if(HoopsGameManager.ScoreResetBool2){
            gameManager.allScores[0] = 0;
            gameManager.allScores[1] = 0;
            // DebugUIManager.instance.ShowDebugUIMessage(gameManager.allScores[1].ToString());
            infoBoard.text = "Current Score: " + 0;
            
            StartCoroutine(WaitForResetScore());
            score = 0;
            
        }
        gameManager.allScores[playerNumber - 1] = score;*/
    }

    IEnumerator WaitForResetScore(){
        yield return new WaitForSeconds(1f);
        HoopsGameManager.ScoreResetBool2 = false;
        //TableButton.ScoreResetBool = false;
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject ball = other.gameObject;
        PhotonView ballPhotonView = ball.GetComponent<PhotonView>();
        View.RPC("PhotonTriggerExit",RpcTarget.AllBuffered, ballPhotonView.ViewID);
    }

    [PunRPC]
    public void PhotonTriggerExit(int ballViewId){
        PhotonView ballPhotonView  = PhotonView.Find(ballViewId);
        GameObject ball = ballPhotonView.gameObject;
/*        foreach (string tagToTest in HoopsGameManager.hoopsBasketballTags)
        {
            if (ball.CompareTag(tagToTest))
            {
                if (ball.CompareTag(HoopsGameManager.hoopsAnswerList[HoopsGameManager.hoopsQuestionNumber - 1]))
                {
                    answerCorrect = true;
                }
                else
                {
                    answerCorrect = false;
                }
                updatingScore = true;
                StartCoroutine(InfoBoardProgress(answerCorrect));
            }
        }*/
    }

    

    IEnumerator InfoBoardProgress(bool correct)
    {
        
        if (correct == answerCorrect && correct)
        {
            score += 3;
            infoBoard.text = "Correct!";
            gameManager.QuestionBoardCorrect(playerNumber);
        }
        else if (correct == answerCorrect && !correct)
        {
            score += 1;
            infoBoard.text = "Incorrect!";
        }
        answerCorrect = false;
        yield return new WaitForSeconds(1.5f);
        infoBoard.text = "Current Score: " + score;
        
    }
}
