using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

namespace AngryMouse
{
    public class GameManager : MonoBehaviour
    {
        public PhotonView view;
        public MoeManager moeManager;

        //Questions
        public List<Question> questions;
        [SerializeField] private int currentIndex = 0;
        public Question currentQuestion;
        public string answer = "";

        [SerializeField] private List<PlayerButton> _playerButtons;
        public bool isPlayersReady = false;
        public bool IsReadyToStart = false;
        public bool IsGameStart = false;
        public bool IsGameEnd = false;
        public bool IsReset = false;
        public bool IsResetCoroutine = false;

        public float currentSec = 0f;
        public float timerSec = 3f;
        public bool IsReadyTimerCoroutine = false;

        public TMP_Text questionBoard;


        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();
            moeManager = FindObjectOfType<MoeManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void InitQuestions()
        {
            questions.Add(new Question(
            "What are not major disinfectants for lab disinfection?\n"
            + "A. Hypochlorites\n" + "B. Formaldehyde\n" + "C. Xylene"
            , "C"));


            questions.Add(new Question(
                "Which one is not belong to common behavior that can be exposed to bloodborne pathogens?\n"
                + "A. Splashes to blood\n" + "B. Contact of eyes\n"
                + "C. Bites and knife wounds\n" + "D. Shake hands"
                , "D"));


            questions.Add(new Question("Which communicable disease is NOT found to be common?\n" +
                    "A. Cold\n" +
                    "B. Hepatitis A\n" +
                    "C. Chickenpox\n" +
                    "D. Ebola virus disease\n" +
                    "E. Pink Eye"
                    , "D"));
        }

        private void ShowQuestion()
        {
            currentQuestion = questions[currentIndex];
            questionBoard.text = "Question " + currentIndex + ":\n" + questions[currentIndex].questionText;
        }

        public void HoopsPlayerReady()
        {
            if (PhotonNetwork.IsConnected)
                view.RPC("PhotonHoopsPlayerReady", RpcTarget.AllBuffered);

        }

        [PunRPC]
        private void PhotonHoopsPlayerReady()
        {
            Debug.Log("---Waiting For Players Ready---");
            foreach (PlayerButton button in _playerButtons)
            {
                if (button.isPressed)
                {
                    isPlayersReady = true;
                }
            }
        }
        public void HoopsReadyToStart()
        {
            if (PhotonNetwork.IsConnected)
                view.RPC("PhotonHoopsReadyToStart", RpcTarget.AllBuffered);

            //PhotonHoopsReadyToStart();
        }

        [PunRPC]
        private void PhotonHoopsReadyToStart()
        {
            Debug.Log("---Game Ready To Start---");
            if (isPlayersReady && !IsReadyToStart)
            {
                if (_playerButtons[0].isPressed && _playerButtons[1].isPressed)
                {
                    IsReadyToStart = true;
                }
            }
        }

        public void HoopsStart()
        {
            if (PhotonNetwork.IsConnected)
                view.RPC("PhotonHoopsStart", RpcTarget.AllBuffered);

        }

        [PunRPC]
        private void PhotonHoopsStart()
        {
            Debug.Log("---Start the game---");


        }


        IEnumerator SetQuestionBoardCoroutine()
        {
            yield return new WaitForSeconds(2f);
            currentIndex += 1;
            if (currentIndex > questions.Count)
            {
                IsGameEnd = true;
               // ShowResult();
            }
            else
            {
                ShowQuestion();
            }
        }
    }
}
