using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Shoot : MonoBehaviour
{
    [SerializeField] public int playerNum;
    private Vector3 startPos;

    public float fireRate = 0.25f; // rate of the fire shooting 
    public float fireDistance = 50f; // distance that the fire can reach
    public float hitForce = 100f; //???
    public Transform gunFront ;

    private WaitForSeconds Duration = new WaitForSeconds(0.07f); 
    private LineRenderer laserLine; // the laser line
    private float nextFire; // hold the time between current fire and next fire

    public GameObject redPoint;
    private RaycastHit hit;
    private bool checkHold;

    public AudioSource src;
    public GameObject effect;

    public GameObject[] GunGameBoard; //0 = A, 1 = B, 2 = C, 3 = D
    public string currentAns = "";
    // public AudioClip shootSound;
    public bool check = false;

    public string nameOfParticalEffect;
  
    PhotonView View;

    public TMP_Text ShootingScoreText;
    public GameObject correctPanel;
    public GameObject wrongPanel;

    public GameObject gameManager;

    private int currentBoardNumber = -1; // store the current board number which is just shot by the user

    private HandsAnimationController handsAnimationController;


    private NetworkedGrabbing networkedGrabbing;
    public int score = 0;
    public bool isUpdatedScore;
    public GunGameManager gunGameManager;

    // Start is called before the first frame update
    void Start()
    {
        networkedGrabbing  = GetComponent<NetworkedGrabbing>();
        // index = 0;  
        redPoint.SetActive(false);
        laserLine = GetComponent<LineRenderer>();
        src = GetComponent<AudioSource>();
        src.mute = true;
        effect = GameObject.Find(nameOfParticalEffect);
        View = GetComponent<PhotonView>();
        startPos = transform.position;
        gunGameManager = FindObjectOfType<GunGameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void ShowRedPoint(){
        redPoint.SetActive(true);
    }

    public void HideRedPoint(){
        redPoint.SetActive(false);
    }

    

    public void ShootBullet(){

        if (PhotonNetwork.IsConnected)
        View.RPC("PhotonShootBullet", RpcTarget.AllBuffered);

    }
    [PunRPC]
    public void PhotonShootBullet(){
         if(Time.time > nextFire){ //over the time that cannot shoot
            nextFire = Time.time + fireRate; // next time that can shoot 
            StartCoroutine(ShotEffect());
        }
        // set the laserline postion to the front of the gun
        laserLine.SetPosition(0,gunFront.position); 
        
        if(Physics.Raycast(gunFront.transform.position,gunFront.transform.forward,out hit, fireDistance)){
            laserLine.SetPosition(1, hit.point);

            for (int i =0; i< GunGameBoard.Length; i++){
                if(hit.collider.gameObject == GunGameBoard[i]){
                    hit.collider.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    currentBoardNumber =i;
                    currentAns = GunGameBoard[i].tag;
                    Debug.Log("--Current Ans" + currentAns);

                    if (currentAns == gunGameManager.currentQuestion.answerText && !isUpdatedScore)
                    {
                        score++;
                       // ShowNoticePanel(true);
                    }
                    else {
                        score += 0;
                        //ShowNoticePanel(false);
                    }
                    isUpdatedScore = true;
                    ShootingScoreText.text = score.ToString();

                    StartCoroutine(BoardEffect());
                    break;
                }
                
            }

        }else{
            laserLine.SetPosition(1, gunFront.transform.position + (gunFront.transform.forward * fireDistance));
        }
    }
    public void ShowNoticePanel(bool isCorrect) {
        if (isCorrect)
        {
            //correctPanel.SetActive(true);
            //wrongPanel.SetActive(false);
        }
        else {
            //correctPanel.SetActive(false);
            //wrongPanel.SetActive(true);
        }
    }

    public void DisableAllPanels() {
        correctPanel.SetActive(false);
        wrongPanel.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Hand" && networkedGrabbing.isBeingHeld) {

          /*  if (handsAnimationController.currentPressed > 0 || handsAnimationController.currentPressedR > 0)
            {*/
                Debug.Log("---Shooting---");
                //start shooting
                if (Physics.Raycast(gunFront.transform.position, gunFront.transform.forward, out hit, fireDistance))
                {
                    redPoint.transform.position = hit.point;
                    ShootBullet();

                }
                else
                {
                    redPoint.transform.position = gunFront.transform.position + (gunFront.transform.forward * fireDistance);
                }
            //}
        }
    }
   /*     public void ShowBoardName1(){

            View.RPC("PhotonShowBoardName1", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void PhotonShowBoardName1(){
            if (currentBoardNumber == 0){
                gameManager.GetComponent<GunGameManager>().checkAnswer1("A");
            }else if (currentBoardNumber == 1){
                gameManager.GetComponent<GunGameManager>().checkAnswer1("B");
            }else if (currentBoardNumber == 2){
                gameManager.GetComponent<GunGameManager>().checkAnswer1("C");
            }else if (currentBoardNumber == 3){
                gameManager.GetComponent<GunGameManager>().checkAnswer1("D");
            }else{
                gameManager.GetComponent<GunGameManager>().checkAnswer1(null);
            }
        }

        public void ShowBoardName2(){
             View.RPC("PhotonShowBoardName2", RpcTarget.AllBuffered);
        }

        [PunRPC]
        public void PhotonShowBoardName2(){
            if (currentBoardNumber == 0){
                gameManager.GetComponent<GunGameManager>().checkAnswer2("A");
            }else if (currentBoardNumber == 1){
                gameManager.GetComponent<GunGameManager>().checkAnswer2("B");
            }else if (currentBoardNumber == 2){
                gameManager.GetComponent<GunGameManager>().checkAnswer2("C");
            }else if (currentBoardNumber == 3){
                gameManager.GetComponent<GunGameManager>().checkAnswer2("D");
            }else{
                gameManager.GetComponent<GunGameManager>().checkAnswer2(null);
            }
        }
*/

    
    IEnumerator ShotEffect(){
        // play the sound
        
        src.mute = false;
        src.Play();
        laserLine.enabled = true;
        yield return Duration; // wait for the line disappear
        laserLine.enabled = false;

        effect.transform.GetComponent<ParticleSystem>().Emit(1);
        effect.transform.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.75f);
        effect.transform.GetComponent<ParticleSystem>().Stop();
       

    }

    IEnumerator BoardEffect(){
        yield return new WaitForSeconds(1);

    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            if (PhotonNetwork.IsConnected)
                View.RPC("ResetPosition", RpcTarget.All);

        }
    }

    [PunRPC]
    public void ResetPosition()
    {
        transform.position = startPos;
    }


    public void OnReset() {

        score = 0;
        isUpdatedScore = false;
        HideRedPoint();
        ShootingScoreText.text = score.ToString();


        correctPanel.SetActive(false);
        wrongPanel.SetActive(false);

        if (PhotonNetwork.IsConnected)
            View.RPC("ResetPosition", RpcTarget.All);

    }

}
