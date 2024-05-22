using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
public class MiniGameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    [SerializeField] private string[] RoomName = {"basketball","Multispin","GunGame","GoneWithTheFire"};
    [SerializeField] private string[] sceneArray = {"HoopsArcade_update","Multispin","ShootingGame","GoneWithTheFire"};

    public TextMeshProUGUI[] sceneUserNumber;
    private int currentScene = 0;
    private bool IsJoined = false;

    [SerializeField] private List<RoomInfo> currentRoomList; //create a empty roomlist
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public void onClickCreateRoom(string btnName){
        // DebugUIManager.instance.ShowDebugUIMessage(GameObject.Find(btnName).GetComponent<SceneButton>().scene_number.ToString());
        if(!IsJoined && GameObject.Find(btnName).GetComponent<SceneButton>().click == 0){
            GameObject.Find(btnName).GetComponent<SceneButton>().click = 1;
            currentScene = GameObject.Find(btnName).GetComponent<SceneButton>().scene_number -1;
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 8;
            PhotonNetwork.CreateRoom(RoomName[currentScene],roomOptions,null);
            Debug.Log("Create Room for "+ RoomName[currentScene]);
            IsJoined = true;
            
        }else{
            // DebugUIManager.instance.ShowDebugUIMessage("Loading...");
            currentScene = GameObject.Find(btnName).GetComponent<SceneButton>().scene_number -1;
            PhotonNetwork.JoinRoom(RoomName[currentScene]);
            Debug.Log("Join Room " + RoomName[currentScene]);
            IsJoined = true;
        }
    }

    public override void OnJoinedRoom(){
        SceneLoader.instance.LoadScene(sceneArray[currentScene], true);
        Debug.Log("Join Room " + RoomName[currentScene]);

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        currentRoomList = new List<RoomInfo>(roomList);
        // foreach(RoomInfo roomInfo in roomList){
        //     for(int i =0; i<3; i++){
        //         if (roomInfo.Name.Contains(RoomName[i])){
        //             sceneUserNumber[i].text = roomInfo.PlayerCount+"/ 8";
        //             break;
        //         }
        //     }
        // }
    }

    void Update(){

/*        foreach(RoomInfo roomInfo in currentRoomList){
            for(int i =0; i<4; i++){
                if (roomInfo.Name.Contains(RoomName[i])){
                    sceneUserNumber[i].text = roomInfo.PlayerCount+"/ 8";
                    break;
                }
            }
        }*/
        
    }

    
    
}
