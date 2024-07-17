using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerBaseInfo : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private PlayerNetworkSetup networkSetup;
    public string nameStr = "";

    // Start is called before the first frame update
    void Start()
    {
        nameStr = "auto";

        if (photonView != null && photonView.Owner.NickName != null)
            nameStr = photonView.Owner.NickName;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
