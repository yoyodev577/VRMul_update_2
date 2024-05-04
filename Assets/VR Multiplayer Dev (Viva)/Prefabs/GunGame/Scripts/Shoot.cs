using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Shoot : MonoBehaviour
{
    
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
    // public AudioClip shootSound;
    public bool check = false;

    public string nameOfParticalEffect;
  
    PhotonView View;

    public TMP_Text ShootingScore;

    public GameObject gameManager;

    private int currentBoardNumber = -1; // store the current board number which is just shot by the user
    

    // Start is called before the first frame update
    void Start()
    {
        // index = 0;  
        redPoint.SetActive(false);
        laserLine = GetComponent<LineRenderer>();
        src = GetComponent<AudioSource>();
        src.mute = true;
        effect = GameObject.Find(nameOfParticalEffect);
        View = GetComponent<PhotonView>();

    }

    // Update is called once per frame
    void Update()
    {
       
        if(redPoint.activeSelf == true){
            if(Physics.Raycast(gunFront.transform.position,gunFront.transform.forward, out hit, fireDistance)){
            redPoint.transform.position = hit.point;

            }else{
                redPoint.transform.position  = gunFront.transform.position + (gunFront.transform.forward * fireDistance);
            }
        }


        
        
    }

    public void showRedPoint(){
        redPoint.SetActive(true);
    }

    public void hideRedPoint(){
        redPoint.SetActive(false);
    }

    

    public void shootBullet(){
        // if(Time.time > nextFire){ //over the time that cannot shoot
        //     nextFire = Time.time + fireRate; // next time that can shoot 
        //     StartCoroutine(ShotEffect());
        // }    


        // laserLine.SetPosition(0,gunFront.position); // set the laserline postion to the front of the gun
        
        // if(Physics.Raycast(gunFront.transform.position,gunFront.transform.forward,out hit, fireDistance)){
        //     laserLine.SetPosition(1, hit.point);

        //     for (int i =0; i<=3; i++){
        //         if(hit.collider.gameObject == GunGameBoard[i]){
        //             hit.collider.gameObject.GetComponent<Rigidbody>().useGravity = true;
        //             currentBoardNumber =i;
        //             StartCoroutine(BoardEffect());
        //             break;
        //         }
                
        //     }

        // }else{
        //     laserLine.SetPosition(1, gunFront.transform.position + (gunFront.transform.forward * fireDistance));
        // }
        View.RPC("PhotonShootBullet", RpcTarget.AllBuffered);

    }
    [PunRPC]
    public void PhotonShootBullet(){
         if(Time.time > nextFire){ //over the time that cannot shoot
            nextFire = Time.time + fireRate; // next time that can shoot 
            StartCoroutine(ShotEffect());
        }    


        laserLine.SetPosition(0,gunFront.position); // set the laserline postion to the front of the gun
        
        if(Physics.Raycast(gunFront.transform.position,gunFront.transform.forward,out hit, fireDistance)){
            laserLine.SetPosition(1, hit.point);

            for (int i =0; i<=3; i++){
                if(hit.collider.gameObject == GunGameBoard[i]){
                    hit.collider.gameObject.GetComponent<Rigidbody>().useGravity = true;
                    currentBoardNumber =i;
                    StartCoroutine(BoardEffect());
                    break;
                }
                
            }

        }else{
            laserLine.SetPosition(1, gunFront.transform.position + (gunFront.transform.forward * fireDistance));
        }
    }

    public void ShowBoardName1(){

        // if (currentBoardNumber == 0){
        //     gameManager.GetComponent<GunGameManager>().checkAnswer1("A");
        // }else if (currentBoardNumber == 1){
        //     gameManager.GetComponent<GunGameManager>().checkAnswer1("B");
        // }else if (currentBoardNumber == 2){
        //     gameManager.GetComponent<GunGameManager>().checkAnswer1("C");
        // }else if (currentBoardNumber == 3){
        //     gameManager.GetComponent<GunGameManager>().checkAnswer1("D");
        // }else{
        //     gameManager.GetComponent<GunGameManager>().checkAnswer1(null);
        // }
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

        // if (currentBoardNumber == 0){
        //     gameManager.GetComponent<GunGameManager>().checkAnswer2("A");
        // }else if (currentBoardNumber == 1){
        //     gameManager.GetComponent<GunGameManager>().checkAnswer2("B");
        // }else if (currentBoardNumber == 2){
        //     gameManager.GetComponent<GunGameManager>().checkAnswer2("C");
        // }else if (currentBoardNumber == 3){
        //     gameManager.GetComponent<GunGameManager>().checkAnswer2("D");
        // }else{
        //     gameManager.GetComponent<GunGameManager>().checkAnswer2(null);
        // }
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

    
}
