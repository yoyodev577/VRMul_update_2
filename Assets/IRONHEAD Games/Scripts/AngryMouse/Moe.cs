using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Moe : MonoBehaviour
{
    public PhotonTransformView transformView;
    public float minHeight, maxHeight;
    public Vector3 startPos;
    public bool isPop = false;
    public bool isHit = false;
    public float speed = 3f;

    public GameObject panelObj;
    public TextMeshProUGUI textMeshProUGUI;
    public string currentAns = "";
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        panelObj.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isPop)
        {
            Pop();
        }
        else {
            Hide();
        }
    }



    public void SetPop( bool pop) {
        isPop = pop;
    }

    public void SetHitStatus(bool _isHit) {
        isHit = _isHit;
    }
    public void SetCurrentAns(string s) {
        currentAns = s;
    }

    public void Pop() { 
        Vector3 targetPos =  new Vector3(startPos.x, maxHeight, startPos.z);
        //Debug.Log(targetPos);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, speed);
        panelObj.SetActive(true);
        textMeshProUGUI.text = currentAns;
    }

    public void Hide() {
        Vector3 targetPos = new Vector3(startPos.x, minHeight, startPos.z);
       // Debug.Log(targetPos);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, speed);

        panelObj.SetActive(false);
    }

    public void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "hammer") {
            isHit = true;
            isPop = false;
        }
    }
}
