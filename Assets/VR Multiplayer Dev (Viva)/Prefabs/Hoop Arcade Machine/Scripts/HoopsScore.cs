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

    private bool isCorrect = false;
    private bool isScored = false;
    private bool isCoroutine = false;

    public TMP_Text infoBoard;

    PhotonView View;
    #endregion

    void Start()
    {
        score = 0;
        View = GetComponent<PhotonView>();
    }
    private void OnTriggerExit(Collider other)
    {
        GameObject ball = other.gameObject;
        PhotonView ballPhotonView = ball.GetComponent<PhotonView>();
        View.RPC("PhotonTriggerExit",RpcTarget.AllBuffered, ballPhotonView.ViewID);
    }

    [PunRPC]
    public void PhotonTriggerExit(int ballViewId)
    {
        PhotonView ballPhotonView = PhotonView.Find(ballViewId);
        GameObject ball = ballPhotonView.gameObject;

        Question q = HoopsGameManager._instance.currentQuestion;

        if (q == null) return;
        // check the answer

        if (ball.CompareTag(q.answerText))
        {
            isCorrect = true;
        }
        else
        {
            isCorrect = false;
        }
        isScored = true;

        if(!isCoroutine)
        StartCoroutine(InfoBoardProgress(isCorrect));

    }

    public void ResetScore()
    {
        score = 0;
        infoBoard.text = "Current Score: " + score;

    }

    IEnumerator InfoBoardProgress(bool correct)
    {
        isCoroutine = true;
        if (correct == isCorrect && correct)
        {
            score += 3;
            infoBoard.text = "Correct!";
            HoopsGameManager._instance.UpdateQuestionBoard(playerNumber);
        }
        else if (correct == isCorrect && !correct)
        {
           // score += 1;
            infoBoard.text = "Incorrect!";
        }
        isCorrect = false;
        yield return new WaitForSeconds(1.5f);
        infoBoard.text = "Current Score: " + score;
        isCoroutine = false;
        
    }


}
