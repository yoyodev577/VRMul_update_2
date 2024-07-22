using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;
using AngryMouse;

public class MoeManager : MonoBehaviour
{
    public PhotonView view;
    public GameManager manager;
    public List<Moe> moes;
    public List<Moe> temp;
    public List<Moe> popList = new List<Moe>();
    public string[] answerList = { "A", "B", "C", "D" };
    public bool isEnabled = false;
    public int maxMoes = 4;

    public bool isCoroutine = false;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        HideAllMoes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SetEngine(true);
        }

    }

    public void SetEngine(bool _isEnabled) {

        isEnabled = _isEnabled;

        if( _isEnabled &&!isCoroutine)
        {
            StartCoroutine(MoeCoroutine());
        }
        else if( !_isEnabled && isCoroutine )
        {
            StopAllCoroutines();
        }

    }
    [PunRPC]
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

    [PunRPC]
    public void PopMoes() {
        if (popList.Count == 0) return;

        for(int i = 0; i < popList.Count; i++)
        {
            // first one = A, second one = B
            popList[i].SetCurrentAns(answerList[i]);
            popList[i].SetPop(true);
        }

    }

    [PunRPC]
    public void HideAllMoes() 
    { 
        for(int i = 0; i < moes.Count; i++)
        {
            moes[i].SetPop(false);
            moes[i].SetHitStatus(false);
        }
    
    }

    public IEnumerator MoeCoroutine() {
        isCoroutine = true;
        while (isEnabled)
        {
            RandomPickMoes();
            //view.RPC("RandomPickMoes", RpcTarget.AllBuffered);
            yield return new WaitForFixedUpdate();

            PopMoes();
            //view.RPC("PopMoes", RpcTarget.AllBuffered);

            yield return new WaitForSeconds(3);
            HideAllMoes();
            //view.RPC("HideAllMoes", RpcTarget.AllBuffered);
        }
        isCoroutine = false;

    }

}
