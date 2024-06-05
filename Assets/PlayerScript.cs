using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviourPun
{
    PhotonManager PM;
    PhotonView PV;

    private void Start()
    {
        PV = photonView;
        PM = PhotonManager.instance;
    }
}
