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
    [SerializeField] private HoopsScore hoopsScore;
    [SerializeField] private int machineNum = 0;


    // Start is called before the first frame update
    void Start()
    {
        hoopsScore = m_Struct.scoreDetector.GetComponent<HoopsScore>();
    }
    public void SetGate(bool isOpen) {
        m_Struct.gate.gameObject.SetActive(isOpen);
    }

    public void BallReset() {
        for (int i = 0; i < m_Struct.balls.childCount; i++)
        {
            GameObject ball = m_Struct.balls.GetChild(i).gameObject;
            ball.transform.position = m_Struct.spawnPts.GetChild(0).position;       
        }
    }
    public int GetScore() {
        return hoopsScore.score;
    }

    public void ResetScore() {
        hoopsScore.ResetScore();
    }

}
