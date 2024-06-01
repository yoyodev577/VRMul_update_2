using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopsBound : MonoBehaviour
{
    public Transform[] SpawnPoints;
    

    private void OnTriggerExit(Collider other)
    {
        int spawnListSize = SpawnPoints.Length;
        foreach (string tagToTest in HoopsGameManager.hoopsBasketballTags)
        {
            if (other.CompareTag(tagToTest))
            {
                other.gameObject.transform.position = SpawnPoints[Random.Range(0, spawnListSize)].position;
                other.attachedRigidbody.velocity = Vector3.zero;
                other.attachedRigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
