using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SabotageManager : MonoBehaviour
{
    public static SabotageManager singleton;

    [SerializeField] private PhotonView photonView;
    public bool canSabotage = true;
    public bool isSabotaged;
    public bool commsDown;

    [SerializeField] private TaskData oxygen;
    [SerializeField] private TaskData reactor;
    [SerializeField] private TaskData lights;
    [SerializeField] private TaskData comms;
    
    private void Awake()
    {
        singleton = this;
    }

    public void Oxygen()
    {
        photonView.RPC("RPCOxygen", RpcTarget.AllBuffered);
        photonView.RPC("RPCSabotageStart", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void RPCOxygen()
    {
        UIManager.singleton.SabotageOn("Warning: Oxygen Supply Running Low");
        StartCoroutine(SabotageDeathTimer());
        isSabotaged = true;
        AddTask(oxygen);
    }
    
    public void Reactor()
    {
        photonView.RPC("RPCReactor", RpcTarget.AllBuffered);
        photonView.RPC("RPCSabotageStart", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void RPCReactor()
    {
        UIManager.singleton.SabotageOn("Warning: Reactor Meltdown Imminent");
        StartCoroutine(SabotageDeathTimer());
        isSabotaged = true;
        AddTask(reactor);
    }
    
    public void Lights()
    {
        photonView.RPC("RPCLights", RpcTarget.AllBuffered);
        photonView.RPC("RPCSabotageStart", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void RPCLights()
    {
        UIManager.singleton.SabotageOn("Lights: Off");
        isSabotaged = true;
        AddTask(lights);
        if(GameManager.singleton.GetLocalPlayer().crewmate)
            RenderSettings.fogEndDistance = GameManager.singleton.fogDistance / 5;
    }
    
    public void Comms()
    {
        photonView.RPC("RPCComms", RpcTarget.AllBuffered);
        photonView.RPC("RPCSabotageStart", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void RPCComms()
    {
        UIManager.singleton.SabotageOn("Comms: Down");
        isSabotaged = true;
        commsDown = true;
        AddTask(comms);
    }
    
    [PunRPC]
    private void RPCSabotageStart()
    {
        Debug.Log("Sabotaging");
        canSabotage = false;
        PlayerManager p = GameManager.singleton.GetLocalPlayer();
        UIManager.singleton.UpdateTasks(p.crewmate ? 
            p.GetComponent<PlayerTasks>().tasks : p.GetComponent<PlayerImpostor>().tasks);
    }
    
    private void SabotageEnd()
    {
        Debug.Log("Sabotage End");
        canSabotage = true;
    }
    
    private IEnumerator SabotageDeathTimer()
    {
        for (int i = 0; i < 30; i++)
        {
            RenderSettings.fogColor = i % 2 == 0 ? Color.black : Color.red;

            UIManager.singleton.UpdateSabotage(30 - i);
            yield return new WaitForSeconds(1);
        }
        UIManager.singleton.SabotageOff();
        foreach (PlayerManager p in GameManager.singleton.totalPlayers)
        {
            if(p.crewmate)
                p.Die();
        }
    }

    private IEnumerator ResetSabotageTimer()
    {
        Debug.Log("Starting Timer");
        yield return new WaitForSeconds(10);
        //if(PhotonNetwork.IsMasterClient)
        SabotageEnd();
            //photonView.RPC("RPCToggleCanSabotage", RpcTarget.AllBuffered);
    }

    public void CompleteSabotage(TaskData type)
    {
        photonView.RPC("RPCCompleteSabotage", RpcTarget.AllBuffered, (int)DataToEnum(type));
    }
    [PunRPC]
    public void RPCCompleteSabotage(int type)
    {
        StopAllCoroutines();
        StartCoroutine(ResetSabotageTimer());
        isSabotaged = false;
        RemoveTask(EnumToData(type));
        RenderSettings.fogEndDistance = GameManager.singleton.fogDistance;
        commsDown = false;
        UIManager.singleton.SabotageOff();
        PlayerManager p = GameManager.singleton.GetLocalPlayer();
        UIManager.singleton.UpdateTasks(p.crewmate ? 
            p.GetComponent<PlayerTasks>().tasks : p.GetComponent<PlayerImpostor>().tasks);
        RenderSettings.fogColor = Color.black;
        Debug.Log("Sabotage: "  + EnumToData(type).name + " has been averted");
    }

    private TaskData EnumToData(int type)
    {
        switch (type)
        {
            case (int)SabotageTypes.REACTOR:
                return reactor;
            case (int)SabotageTypes.OXYGEN:
                return oxygen;
            case (int)SabotageTypes.COMMS:
                return comms;
            case (int)SabotageTypes.LIGHTS:
                return lights;
        }

        return null;
    }
    
    private SabotageTypes DataToEnum(TaskData type)
    {
        if (type == reactor)
            return SabotageTypes.REACTOR;
        if (type == oxygen)
            return SabotageTypes.OXYGEN;
        if (type == comms)
            return SabotageTypes.COMMS;
        if (type == lights)
            return SabotageTypes.LIGHTS;

        return SabotageTypes.REACTOR;
    }

    private void AddTask(TaskData task)
    {
        foreach (PlayerManager p in GameManager.singleton.totalPlayers)
        {
            p.GetComponent<PlayerTasks>().tasks.Add(task);
            p.GetComponent<PlayerImpostor>().tasks.Add(task);
        }
    }

    private void RemoveTask(TaskData task)
    {
        foreach (PlayerManager p in GameManager.singleton.totalPlayers)
        {
            p.GetComponent<PlayerTasks>().tasks.Remove(task);
            p.GetComponent<PlayerImpostor>().tasks.Remove(task);
        }
    }
}

public enum SabotageTypes
{
    REACTOR,
    LIGHTS,
    COMMS,
    OXYGEN
}
