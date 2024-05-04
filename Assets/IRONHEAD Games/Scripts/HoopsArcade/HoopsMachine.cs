using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HoopsMachineStruct
{
    public Transform balls;
    public Transform spawnPts;
    public Transform gate;
    public Transform scoreDetector;
    public Transform scoreboard;
}

public class HoopsMachine : MonoBehaviour
{
    public HoopsMachineStruct m_Struct;

    // Start is called before the first frame update
    void Start()
    {
        m_Struct.balls = GameObject.Find("Basketballs").transform;
        m_Struct.spawnPts = GameObject.Find("Spawnpoints").transform;
        m_Struct.gate = GameObject.Find("Gate").transform;
        m_Struct.scoreDetector = GameObject.Find("ScoreDetector").transform;
        m_Struct.scoreboard = GameObject.Find("ScoreBoard").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetGate(bool isOpen) {
        m_Struct.gate.gameObject.SetActive(isOpen);
    }

    public void BallReset() {
        for (int i = 0; i < m_Struct.balls.childCount; i++)
        {
            GameObject ball = m_Struct.balls.GetChild(i).gameObject;
            ball.transform.position = m_Struct.spawnPts.GetChild(i).position;
           
        }

    }

}
