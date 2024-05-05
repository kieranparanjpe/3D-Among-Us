using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerManager : MonoBehaviour
{
    public PhotonView photonView;

    public bool crewmate = true;
    public bool isDead = false;

    [HideInInspector] public Color colour = new Color(0, 0, 0, 0);
    public new string name = "Player";
    [SerializeField] private GameObject deadBody;
    [HideInInspector] public GameObject m_deadBody;
    [SerializeField] private MeshRenderer bodyMesh;
    public TextMeshPro displayName;
    public Room room = Room.CAFETERIA;

    private PlayerManager localPlayer;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (!photonView.IsMine)
        {
            GetComponent<PlayerCamera>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;

            Camera[] cameras = GetComponentsInChildren<Camera>();
            foreach (Camera cam in cameras)
            {
                cam.gameObject.SetActive(false);
            }
        }
        else
        {
            GameManager.singleton.photonView.RPC("AddPlayer", RpcTarget.AllBuffered, photonView.ViewID);

            StartCoroutine(StartDelay());
        }
    }

    private IEnumerator StartDelay()
    {
        photonView.RPC("UpdateName", RpcTarget.AllBuffered, "Player " + Random.Range(0, 1000));

        while (GameManager.singleton.inUseColours.Count == 0 && !PhotonNetwork.IsMasterClient)
        {
            yield return new WaitForSeconds(0.1f);
        }

        List<Color> possibilities = GameManager.singleton.totalColours;
        foreach (Color c in GameManager.singleton.inUseColours)
        {
            Debug.Log(ColorUtility.ToHtmlStringRGB(c));
            possibilities.Remove(c);
        }

        string hex = "#" + ColorUtility.ToHtmlStringRGB(possibilities[Random.Range(0, possibilities.Count)]);
        photonView.RPC("RPCUpdateColour", RpcTarget.AllBuffered, hex);
    }

private void Update()
    {
        if (!photonView.IsMine)
        {
            if (localPlayer == null)
            {
                localPlayer = GameManager.singleton.GetLocalPlayer();
                if (localPlayer == null)
                    localPlayer = this;
            }

            float sightDistance = RenderSettings.fogEndDistance == GameManager.singleton.fogDistance ? 
                GameManager.singleton.fogDistance : GameManager.singleton.fogDistance / 5;
            
            displayName.gameObject.SetActive(
                Vector3.Distance(transform.position, localPlayer.transform.position) < sightDistance);

            Quaternion targetRotation =
                Quaternion.LookRotation(displayName.transform.position - localPlayer.displayName.transform.position);
            //displayName.transform.LookAt(localPlayer.displayName.transform.position);
            displayName.transform.rotation = targetRotation;

        }
    }

    public void UpdateColour(String c)
    {
        photonView.RPC("RPCUpdateColour", RpcTarget.AllBuffered, c);
    }

    [PunRPC]
    private void RPCUpdateColour(String c)
    {
        if (colour != new Color(0, 0, 0, 0))
            GameManager.singleton.inUseColours.Remove(colour);
            
        ColorUtility.TryParseHtmlString(c, out colour);
        
        bodyMesh.material.color = colour;
        GameManager.singleton.inUseColours.Add(colour);
    }
    
    [PunRPC]
    public void UpdateName(String n)
    {
        name = n;
        displayName.text = name;
    }

    public void StartGame(bool team, int index)
    {
        print("Starting Game...");
        photonView.RPC("SetTeam", RpcTarget.AllBuffered, team, index);
    }
    
    [PunRPC]
    public void SetTeam(bool team, int index)
    {
        crewmate = team;
        transform.position = GameManager.singleton.spawns[index].position;


        if (photonView.IsMine)
        {
            GameManager.singleton.started = true;
            
            List<TaskData> tasks = new List<TaskData>();

            int shortTasks = (int)GameManager.singleton.output["#ST"];
            int longTasks = (int)GameManager.singleton.output["#LT"];
            int commonTasks = (int)GameManager.singleton.output["#CT"];

            shortTasks = Mathf.Min(shortTasks, GameManager.singleton.shortTasks.Length);
            longTasks = Mathf.Min(longTasks, GameManager.singleton.longTasks.Length);
            commonTasks = Mathf.Min(commonTasks, GameManager.singleton.commonTasks.Length);

            for (int i = 0; i < shortTasks; i++)
            {
                TaskData data = GameManager.singleton.shortTasks[Random.Range(0, GameManager.singleton.shortTasks.Length)];
                if (!tasks.Contains(data))
                    tasks.Add(data);
                else
                    i--;
            }
            for (int i = 0; i < longTasks; i++)
            {
                TaskData data = GameManager.singleton.longTasks[Random.Range(0, GameManager.singleton.longTasks.Length)];
                if (!tasks.Contains(data))
                    tasks.Add(data);
                else
                    i--;
            }
            for (int i = 0; i < commonTasks; i++)
            {
                TaskData data = GameManager.singleton.commonTasks[Random.Range(0, GameManager.singleton.commonTasks.Length)];
                if (!tasks.Contains(data))
                    tasks.Add(data);
                else
                    i--;
            }
            UIManager.singleton.UpdateTasks(tasks);

            if (crewmate)
            {
                GetComponent<PlayerTasks>().enabled = true;
                GetComponent<PlayerTasks>().tasks = tasks;
                
                GetComponent<PlayerImpostor>().enabled = false;
            }
            else
            {
                GetComponent<PlayerTasks>().enabled = false;
                
                GetComponent<PlayerImpostor>().enabled = true;
                GetComponent<PlayerImpostor>().tasks = tasks;
            }

        }

    }
    
    public void Die()
    {
        photonView.RPC("RPCDie", RpcTarget.AllBuffered);
    }
    
    [PunRPC]
    public void RPCDie()
    {
        print(transform.name + " Is Dead");
        isDead = true;

        int crewmates = 0;
        int impostors = 0;
        
        foreach (PlayerManager p in GameManager.singleton.totalPlayers)
        {
            if (p.crewmate && !p.isDead)
                crewmates++;

            if (!p.crewmate && !p.isDead)
                impostors++;
        }
        
        if(impostors <= 0)
            GameManager.singleton.GameEnd(true);
        else if(crewmates / impostors <= 1)
            GameManager.singleton.GameEnd(false);
        
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform c in children)
        {
            c.gameObject.layer = LayerMask.NameToLayer("Dead");
        }

        m_deadBody = Instantiate(deadBody, transform.position, transform.rotation);
        m_deadBody.GetComponent<DeadBody>().Initialize(photonView.ViewID);

        if (photonView.IsMine)
            GameManager.singleton.voteState = VoteState.Dead;
    }
}

public class PlayerCompare : IComparer<PlayerManager>
{
    public int Compare(PlayerManager x, PlayerManager y)
    {
        if (x == null || y == null)
        {
            Debug.Log("Null Comparison");
            return 0;
        }

        return x.isDead.CompareTo(y.isDead);
    }
}