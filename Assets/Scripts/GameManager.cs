using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Compression;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager singleton;

    public float taskBar;
    public bool pause = false;

    public List<PlayerManager> totalPlayers = new List<PlayerManager>();
    public PlayerManager GetLocalPlayer()
    {
        foreach (PlayerManager p in totalPlayers)
        {
            if (p.photonView.IsMine)
                return p;
        }

        return null;
    }
    
    /*[HideInInspector]*/ public List<Color> inUseColours = new List<Color>();
    /*[HideInInspector]*/ public List<Color> totalColours = new List<Color>();

    private Dictionary<PlayerManager, int> votes = new Dictionary<PlayerManager, int>();
    private int totalVotesCast = 0;
    public VoteState voteState = VoteState.CanVote;
    public bool inMeeting = false;

    public Transform[] initialSpawns;
    public Transform[] spawns;

    public TaskData[] shortTasks;
    public TaskData[] longTasks; 
    public TaskData[] commonTasks;

    public PhotonView photonView;

    private int numberOfTasks = 0;
    public float fogDistance = 18;


    public ExitGames.Client.Photon.Hashtable output = new ExitGames.Client.Photon.Hashtable()
    {
        {"TB", 0},
        {"#ST", 2},
        {"#LT", 1},
        {"#CT", 1},
        {"SPD", 1f},
        {"IVIS", 1f},
        {"CVIS", 1f}, 
        {"EMGM", 1},//todo
        {"VTIME", 100},
        {"KCD", 25},
        {"KDIST", 4f}
    };

    public bool started;

    private void Awake()
    {
        singleton = this;
    }


    private void Update()
    {
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pause;

        if (!started && Input.GetButtonDown("Kill"))
        {
            StartGame();
        }
    }
   
    
    [PunRPC]
    public void AddPlayer(int id)
    {
        PlayerManager player = PhotonView.Find(id).GetComponent<PlayerManager>();

        totalPlayers.Add(player);
    }

    private void StartGame()
    {
        if (/*totalPlayers.Count < 4 ||*/ !PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.CurrentRoom.SetCustomProperties(output);

        
        if (totalPlayers.Count < 8)
        {
            int impostor = Random.Range(0, totalPlayers.Count);

            for (int i = 0; i < totalPlayers.Count; i++)
            {
                totalPlayers[i].StartGame(i != impostor, i); 
            }

            return;
        }
            
        int impostor1 = Random.Range(0, totalPlayers.Count);
        int impostor2 = Random.Range(0, totalPlayers.Count);

        while (impostor1 == impostor2)
        { 
            impostor2 = Random.Range(0, totalPlayers.Count);
        }

        for (int i = 0; i < totalPlayers.Count; i++)
        {
            if(i != impostor1 && i != impostor2)
                totalPlayers[i].StartGame(true, i);
            else
                totalPlayers[i].StartGame(false, i);
        }
        
    }

    public void IncreaseTaskBar()
    {
        taskBar++;
        output["TB"] = taskBar;
        PhotonNetwork.CurrentRoom.SetCustomProperties(output);
        Debug.Log("Setting Properties");
    }

    public void Meeting(PlayerManager report)
    {
        photonView.RPC("RPCMeeting", RpcTarget.All, report.photonView.ViewID);
    }

    public void CastVote(PlayerManager playerManager)
    {
        photonView.RPC("CastVoteRPC", RpcTarget.All, playerManager.photonView.ViewID);
        voteState = VoteState.AlreadyVoted;
        UIManager.singleton.deadMask.SetActive(true);
    }

    [PunRPC]
    private void CastVoteRPC(int player)
    {
        PlayerManager playerManager = PhotonView.Find(player).GetComponent<PlayerManager>();

        totalVotesCast++;

        if (player >= 0)
        {
            if (votes.ContainsKey(playerManager))
            {
                votes[playerManager]++;
            }
            else 
            {
                votes.Add(playerManager, 1);
            }
        }
        
        int alive = 0;

        foreach (PlayerManager p in totalPlayers)
        {
            if (!p.isDead)
                alive++;
        }

        if (totalVotesCast >= alive)
            FinishMeeting();
    }

    private void FinishMeeting()
    {
        StopAllCoroutines();

        PlayerManager eject = null;
        int mostVotes = 0;
        bool tie = true;

        foreach (KeyValuePair<PlayerManager, int> v in votes)
        {
            if (v.Value > mostVotes)
            {
                tie = false;
                mostVotes = v.Value;
                eject = v.Key;
            }
            else if(v.Value == mostVotes)
            {
                tie = true;
            }
        }

        if (!tie && eject != null && eject.photonView.IsMine)
        {
            eject.Die();
        }
        
        totalVotesCast = 0;
        votes = new Dictionary<PlayerManager, int>();
        
        UIManager.singleton.FinishMeeting(eject);

        if(voteState == VoteState.AlreadyVoted)
            voteState = VoteState.CanVote;

        for (int i = 0; i < totalPlayers.Count; i++ )
        {
            totalPlayers[i].transform.position = spawns[i].position;
        }
        inMeeting = false;
        pause = false;
    }

    [PunRPC]
    private void RPCMeeting(int player)
    {
        pause = true;
        inMeeting = true;
        UIManager.singleton.Meeting(PhotonView.Find(player).GetComponent<PlayerManager>());
        StartCoroutine(VotingTimer());

        foreach (PlayerManager p in totalPlayers)
        {
            if(p.m_deadBody != null)
                Destroy(p.m_deadBody);
        }
    }

    private IEnumerator VotingTimer()
    {
        for (int i = (int)output["VTIME"]; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            UIManager.singleton.VotingTimer(i);
        }
        
        FinishMeeting();
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("Getting Properties");
        
        output.Merge(propertiesThatChanged);

        try
        {
            Debug.Log("Attempting TB");
            taskBar = (float) propertiesThatChanged["TB"];
            UIManager.singleton.UpdateTaskBar(taskBar / numberOfTasks);
            
            if(taskBar >= numberOfTasks)
                GameEnd(true);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        if (propertiesThatChanged.Count > 1)
        {
            int nshortTasks = (int)output["#ST"];
            int nlongTasks = (int)output["#LT"];
            int ncommonTasks = (int)output["#CT"];
            
            nshortTasks = Mathf.Min(nshortTasks, shortTasks.Length);
            nlongTasks = Mathf.Min(nlongTasks, longTasks.Length);
            ncommonTasks = Mathf.Min(ncommonTasks, commonTasks.Length);

            numberOfTasks = (nshortTasks + nlongTasks + ncommonTasks) * 
                (totalPlayers.Count - totalPlayers.Count < 8 ? 1 : 2);
            
            fogDistance = 18 * (GetLocalPlayer().crewmate ? 
                    (float)output["CVIS"] : (float)output["IVIS"]);

            RenderSettings.fogEndDistance = fogDistance;
        }
    }

    public void GameEnd(bool crewmate)
    {
        photonView.RPC("RPCGameEnd", RpcTarget.All, crewmate);
    }

    [PunRPC]
    private void RPCGameEnd(bool crewmate)
    {
        Debug.Log(crewmate ? "crewmates win" : "impostors win");

        output["TB"] = 0;
        voteState = VoteState.CanVote;
        started = false;

        UIManager.singleton.StartCoroutine(UIManager.singleton.GameEnd(crewmate));
        UIManager.singleton.UpdateTaskBar(0);

        for (int i = 0; i < totalPlayers.Count; i++)
        {
            Transform[] children = totalPlayers[i].GetComponentsInChildren<Transform>();
            foreach (Transform c in children)
            {
                c.gameObject.layer = LayerMask.NameToLayer("Default");
            }

            if (totalPlayers[i].m_deadBody != null)
                Destroy(totalPlayers[i].m_deadBody);

            //if(PhotonNetwork.IsMasterClient)
                totalPlayers[i].transform.position = initialSpawns[i].position;
            
            totalPlayers[i].GetComponent<PlayerTasks>().enabled = false;
            totalPlayers[i].GetComponent<PlayerImpostor>().enabled = false;
            totalPlayers[i].isDead = false;
            totalPlayers[i].crewmate = true;
            SabotageManager.singleton.StopAllCoroutines();
            SabotageManager.singleton.canSabotage = true;
            SabotageManager.singleton.isSabotaged = false;
            SabotageManager.singleton.commsDown = false;
            RenderSettings.fogEndDistance = GameManager.singleton.fogDistance;
            UIManager.singleton.SabotageOff();
            RenderSettings.fogColor = Color.black;
        }
    }
}

public enum VoteState
{
    CanVote,
    AlreadyVoted,
    Dead
}
