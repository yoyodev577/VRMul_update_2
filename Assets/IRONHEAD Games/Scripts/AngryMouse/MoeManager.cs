using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class MoeManager : MonoBehaviour
{
    public List<Moe> moes;
    public List<Moe> temp;
    public List<Moe> popList = new List<Moe>();
    public string[] answerList = { "A", "B", "C", "D" };
    public bool isEnabled = false;
    public int maxMoes = 4;

    // Start is called before the first frame update
    void Start()
    {
        HideAllMoes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            RandomPickMoes();
            PopMoes();
        }else  if(Input.GetKeyDown(KeyCode.Q))
        {
            HideAllMoes();
        }

    }

    public void RandomPickMoes()
    {
        popList.Clear();
        temp.Clear();

        temp.AddRange(moes);

        while (popList.Count < maxMoes)
        {
            int r = Random.Range(0, temp.Count);
            if (!popList.Contains(temp[r]))
            {
                Debug.Log("Pop " + temp[r].name);
                popList.Add(temp[r]);
                temp.RemoveAt(r);
            }

        }
    }

    public void PopMoes() {
        if (popList.Count == 0) return;

        for(int i = 0; i < popList.Count; i++)
        {
            // first one = A, second one = B
            popList[i].SetCurrentAns(answerList[i]);
            popList[i].SetPop(true);
        }

    }

    public void HideAllMoes() 
    { 
        for(int i = 0; i < moes.Count; i++)
        {
            moes[i].SetPop(false);
            moes[i].SetHitStatus(false);
        }
    
    }

}
