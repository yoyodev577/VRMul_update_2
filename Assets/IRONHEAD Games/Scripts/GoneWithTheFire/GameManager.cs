using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using TMPro;

namespace GoneWithTheFire
{
    public class GameManager : MonoBehaviour
    {
        private PhotonView view;
        private VideoPlayer videoPlayer;
        private SpawnManager spawnManager;

        public bool isVideoPlayed = false;
        public bool isVideoCoroutine = false;
        public bool isAnimationPlayed = false;
        public bool isAnimationCoroutine = false;
        public bool isReadyTimerCoroutine = false;
        public bool isReadyToStart = false;
        public bool isGameStart = false;
        public bool isGameEnd = false;
        public bool isReset = false;

        public float currentSec = 0;

        public List<ExitDetector> detectors;

        public GameObject videoPanel, boardPanel;
        public TMP_Text boardText;

        public GameObject bottleObj;
        public Animator bottleAnimator;
        public GameObject fireObj;


        public AudioSource bgmSource, sfxSource;
        public AudioClip countDownClip, fallClip, fireClip;
        // Start is called before the first frame update
        void Start()
        {
            view = GetComponent<PhotonView>();
            spawnManager = FindObjectOfType<SpawnManager>();
            videoPlayer = FindObjectOfType<VideoPlayer>();
            videoPanel.SetActive(true);
            boardPanel.SetActive(false);
            //videoPlayer.Play();

        }

        // Update is called once per frame
        void Update()
        {
           // PhotonUpdate();
            if (PhotonNetwork.IsConnected)
                view.RPC("PhotonUpdate", RpcTarget.AllBuffered);

        }

        [PunRPC]
        public void PhotonUpdate() {

            if (isVideoPlayed && !videoPlayer.isPlaying)
            {
                if (!isReadyToStart)
                {
                    videoPanel.SetActive(false);
                    boardPanel.SetActive(true);

                    view.RPC("PhotonPlayAnimation", RpcTarget.All);

                    if (isAnimationPlayed)
                    {
                        isReadyToStart = true;
                        if (!isReadyTimerCoroutine)
                        {
                            StartCoroutine(SetReadyTimerCoroutine(5));
                        }
                    }
                }

            }
        }

        public void StartGame()
        {
           //PhotonStartGame();

            if (PhotonNetwork.IsConnected)
                view.RPC("PhotonStartGame", RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void PhotonPlayAnimation() {

            bottleObj.SetActive(true);
            bottleAnimator = bottleObj.GetComponent<Animator>();
            bottleAnimator.SetTrigger("Fall");
            if(!sfxSource.isPlaying)
            sfxSource.PlayOneShot(fallClip);
            if (bottleAnimator.GetAnimatorTransitionInfo(0).normalizedTime >0.5f) 
            {
                sfxSource.PlayOneShot(fireClip);
                fireObj.SetActive(true);
                isAnimationPlayed = true;
            }

        
        }

        [PunRPC]
        private void PhotonStartGame()
        {
            if (!isVideoPlayed)
            {
                if (!isVideoCoroutine)
                    StartCoroutine(VideoCoroutine());

            }

        }



        public void EndGame() {

            if (PhotonNetwork.IsConnected)
                view.RPC("PhotonEndGame", RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void PhotonEndGame()
        {
            isGameEnd = true;
        }

        public void ResetGame()
        {
            if (PhotonNetwork.IsConnected)
                view.RPC("PhotonResetGame", RpcTarget.AllBuffered);

        }


        [PunRPC]
        private void PhotonResetGame()
        {
            isAnimationPlayed = false;
            isReadyTimerCoroutine = false;
            isVideoCoroutine = false;
            isReadyToStart = false;
            isVideoPlayed = false;
            isGameStart = false;
            isGameEnd = false;
            currentSec = 0;

            videoPanel.SetActive(true);
            boardPanel.SetActive(false);
        }


        void UpdateBoardText(string txt)
        {
            boardText.text = txt;
        }

        IEnumerator VideoCoroutine() {
            isVideoCoroutine = true;
            yield return new WaitForSeconds(1);
            videoPlayer.Play();
            yield return new WaitUntil(()=>!videoPlayer.isPlaying);
            isVideoPlayed = true;
            isVideoCoroutine = false;
               
        }

        IEnumerator SetReadyTimerCoroutine(float seconds)
        {
            isReadyTimerCoroutine = true;
            currentSec = seconds;
            UpdateBoardText(currentSec.ToString());

            while (currentSec >= 0)
            {
                sfxSource.PlayOneShot(countDownClip);
                UpdateBoardText(currentSec.ToString());
                yield return new WaitForSeconds(1f);
                currentSec -= 1;
            }

            if (currentSec <= 0)
            {
                sfxSource.Stop();
                isReadyToStart = true;
                isGameStart = true;
                UpdateBoardText("Game Starts");
            }

            isReadyTimerCoroutine = false;
            yield return null;
        }
    }
}
