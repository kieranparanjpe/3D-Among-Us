using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager singleton;

    private PhotonView _photonView;

    private int currentScene;
    private int multiplayerScene = 1;
    
    private string createRoomName = "";
    private string joinRoomName = "";

    public RoomOptions roomOptions;

    [SerializeField] private TextMeshProUGUI errorText;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(singleton.gameObject);
            singleton = this;
        }
        DontDestroyOnLoad(gameObject);

        _photonView = GetComponent<PhotonView>();
    }
    
    #region InitialConnection
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)10 };
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("Connected");
    }
    
    public void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom(roomOptions.CustomRoomProperties, 10);
        PhotonNetwork.JoinRoom(joinRoomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log(message);
        errorText.text = "Could not join game with code: " + joinRoomName;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        errorText.text = "Could not create game with code: " + createRoomName;
        CreateRoom();
    }
    
    public void CreateRoom()
    {
        if (name != "" && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.CreateRoom(createRoomName, roomOptions);
        }
    }

    #endregion

    #region LobbyUI

    public void SetCreateRoomName(string name)
    {
        createRoomName = name;
    }

    public void SetJoinRoomName(string name)
    {
        joinRoomName = name;
    }

    #endregion

    #region RoomSetup

    public override void OnEnable()
    {
        base.OnEnable();

        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        StartGame();
    }

    private void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;

        if (currentScene == multiplayerScene)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        Debug.Log("Creating Player...");
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), 
            GameManager.singleton.initialSpawns[PhotonNetwork.CurrentRoom.PlayerCount - 1].position , Quaternion.identity, 0);
    }
    #endregion

    #region LeaveMatch
    public void LeaveMatch()
    {
        StartCoroutine("Disconnect");
    }

    private IEnumerator Disconnect()
    {
        PhotonNetwork.LeaveRoom();

        if (PhotonNetwork.InRoom)
            yield return null;

        SceneManager.LoadScene(0);
    }
    
    #endregion
}
