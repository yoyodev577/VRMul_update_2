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
        public bool isReadyTimerCoroutine = false;
        public bool isReadyToStart = false;
        public bool isGameStart = false;
        public bool isGameEnd = false;
        public bool isReset = false;

        public float currentSec = 0;

        public List<ExitDetector> detectors;

        public GameObject videoPanel, boardPanel;
        public TMP_Text boardText;

        public AudioSource bgmSource, sfxSource;
        public AudioClip countDownClip;
        // Start is called before the first frame update
        void Start()
        {
            spawnManager = FindObjectOfType<SpawnManager>();
            videoPlayer = FindObjectOfType<VideoPlayer>();
            videoPanel.SetActive(true);
            boardPanel.SetActive(false);

            if(!isVideoPlayed)
            {
                if(!isVideoCoroutine)
                    StartCoroutine(VideoCoroutine());

            }


        }

        // Update is called once per frame
        void Update()
        {
            if (isVideoPlayed && !videoPlayer.isPlaying) { 
                if(!isReadyToStart)
                {
                    videoPanel.SetActive(false);
                    boardPanel.SetActive(true);
                    isReadyToStart = true;
                    if (!isReadyTimerCoroutine) {
                        StartCoroutine(SetReadyTimerCoroutine(5));
                    }
                }
            
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
            yield return new WaitForSeconds(3);
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
