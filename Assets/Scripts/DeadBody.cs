using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DeadBody : MonoBehaviour
{
    public PlayerManager player;

    [SerializeField] private MeshRenderer graphics;
    
    public void Initialize(int info)
    {
        player = PhotonNetwork.GetPhotonView(info).GetComponent<PlayerManager>();
        graphics.material.color = player.colour;
        Debug.Log("New Dead Body! " + player.name);
    }
}
