using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandPhysicsIgnore : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private List<string> scenesWithNoColl;
    [SerializeField] private Scene currentScene;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();
        //collider = GetComponent<Collider>();
        collider.isTrigger = true;

        for (int i = 0; i< scenesWithNoColl.Count; i++)
        {
            if (currentScene.name != scenesWithNoColl[i])
                collider.isTrigger = false;

        }
        Debug.Log("Current Scene :" + currentScene.name);
        
    }
    void Update()
    {
        Physics.IgnoreLayerCollision(10, 18);
    }
}
