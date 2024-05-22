// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Photon.Pun;
// using Photon.Realtime;
// using ExitGames.Client.Photon;
// using UnityEngine.SceneManagement;
// using UnityEngine.XR.Interaction.Toolkit;
// public class SpawnManager : MonoBehaviourPunCallbacks
// {
//     public Transform[] spawnPositions;
//     public GameObject[] spawnPrefabList;

//     #region Unity Methods
//     // Start is called before the first frame update
//     void Start()
//     {   
//         SpawnPlayer();
//         StartCoroutine(EnableTeleportArea());
//     }
//     #endregion


//     #region Private Methods
//     IEnumerator EnableTeleportArea()
//     {
//         yield return new WaitForSeconds(2f);
//         FindObjectOfType<TeleportationArea>(true).GetComponent<TeleportationArea>().enabled = true;
//     }
//     private void SpawnPlayer()
//     {
//         int randomSpawnPoint = Random.Range(0, spawnPositions.Length - 1);
//         Vector3 randomInstantiatePosition = spawnPositions[randomSpawnPoint].position;

//         if (MultiplayerVRConstants.USE_FINALIK)
//         {
//             string prefabName = spawnPrefabList[AvatarSelectionManager.selectedAvatarIndex].name;
//             PhotonNetwork.Instantiate(prefabName, randomInstantiatePosition, Quaternion.identity, 0);

//         }else if (MultiplayerVRConstants.USE_FINALIK_UMA2)
//         {
//             PhotonNetwork.Instantiate("NetworkedVRPlayerPrefab_FinalIK_UMA2", randomInstantiatePosition, Quaternion.identity, 0);
//         }
//         else
//         {
//             PhotonNetwork.Instantiate("NetworkedVRPlayerPrefab", randomInstantiatePosition, Quaternion.identity, 0);
//         }
//     }
//     #endregion
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
public class SpawnManager : MonoBehaviourPunCallbacks
{
    public Transform spawnPosition;

    #region Unity Methods
    // Start is called before the first frame update
    void Start()
    {   
        SpawnPlayer();
    }
    #endregion
    


    #region Private Methods
    private void SpawnPlayer()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;

        Vector3 randomInstantiatePosition = spawnPosition.position;
        PhotonNetwork.Instantiate("NetworkedVRPlayerPrefab", randomInstantiatePosition, Quaternion.identity, 0);
    }
    #endregion
}
