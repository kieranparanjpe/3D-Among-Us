using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager singleton;

    [SerializeField] private Image crosshair;
    [SerializeField] private Slider taskBar;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject customize;
    [SerializeField] private GameObject meeting;
    [SerializeField] private GameObject displayEject;
    [SerializeField] public GameObject deadMask;
    [SerializeField] private GameObject impostorWin;
    [SerializeField] private GameObject crewmateWin;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject mapSabotages;
    [SerializeField] private GameObject mapTaskIcon;
    [SerializeField] private Transform mapTaskParent;
    [SerializeField] private RectTransform playerMap;
    [SerializeField] private TextMeshProUGUI sabotageText;
    [SerializeField] private TextMeshProUGUI sabotageTimer;
    [SerializeField] private GameObject killCooldownImage;
    [SerializeField] private GameObject killCooldownMask;
    [SerializeField] private TextMeshProUGUI killCooldownTimer;

    private string currentSabotage;
    
    [SerializeField] private TextMeshProUGUI votingTimerText;
    [SerializeField] private TextMeshProUGUI displayEjectText;

    [SerializeField] private GameObject votingUIObject;
    [SerializeField] private Transform votingUIObjectParent;

    [SerializeField] private Image showColour;
    [SerializeField] private Image[] colours;

    private string playerName = "";

    private void Awake()
    {
        singleton = this;
        
        foreach (Image i in colours)
        {
            GameManager.singleton.totalColours.Add(i.color);
        }
    }
    
    private void Update()
    {
        if (Input.GetButtonDown("Use") && PhotonNetwork.IsMasterClient && !GameManager.singleton.started && !customize.activeSelf)
        {
            settings.SetActive(!settings.activeSelf);
            customize.SetActive(false);
            GameManager.singleton.pause = !GameManager.singleton.pause;
        } 
        if (Input.GetButtonDown("Report") && !GameManager.singleton.started && !settings.activeSelf)
        {
            customize.SetActive(true);
            settings.SetActive(false);
            GameManager.singleton.pause = true;
        }
        if (GameManager.singleton.started)
        {
            settings.SetActive(false);
            customize.SetActive(false);
        }
    }

    public void SabotageOn(string type)
    {
        currentSabotage = type;
        sabotageText.text = currentSabotage;
    }

    public void UpdateSabotage(int timer)
    {
        sabotageTimer.text = timer.ToString();
        sabotageText.text = timer % 2 == 0 ? currentSabotage : "";
    }

    public void SabotageOff()
    {
        sabotageText.text = "";
        sabotageTimer.text = "";
    }

    public void SetCrosshairColour(Color colour)
    {
        crosshair.color = colour;
        
        mapSabotages.SetActive(colour != Color.white);
        killCooldownImage.SetActive(colour != Color.white);
    }

    public void UpdateKillCooldown(int time)
    {
        killCooldownTimer.text = time <= 0 ? "Q" : time.ToString();
        killCooldownMask.SetActive(time > 0);
    }
    
    public void UpdateTaskBar(float value)
    {
        taskBar.value = value;
    }

    public void SetEmergencyMeetings(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["EMGM"] = (int)number.Value;
        }
    }

    public void SetShortTasks(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["#ST"] = (int)number.Value;
        }
    }
    
    public void SetLongTasks(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["#LT"] = (int)number.Value;
        }
    }
    
    public void SetCommonTasks(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["#CT"] = (int)number.Value;
        }
    }
    
    public void SetSpeed(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["SPD"] = number.Value;
        }
    }
    
    public void SetImpostorVision(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["IVIS"] = number.Value;
        }
    }
    
    public void SetCrewmateVision(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["CVIS"] = number.Value;
        }
    }
    
    public void SetVotingTime(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["VTIME"] = (int)number.Value;
        }
    }
    
    public void SetKillCooldown(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["KCD"] = (int)number.Value;
        }
    }
    
    public void SetKillDistance(string input)
    {
        KeyValuePair<bool, float> number = ConvertToNumber(input);
        if (number.Key)
        {
            GameManager.singleton.output["KDIST"] = number.Value;
        }
    }

    public void SetColour(string hex)
    {
        Color c;
        ColorUtility.TryParseHtmlString(hex, out c);
        if (!GameManager.singleton.inUseColours.Contains(c))
        {
            playerMap.GetComponent<Image>().color = c;
            GameManager.singleton.GetLocalPlayer().UpdateColour(hex);
        }
    }

    public void SetName(string n)
    {
        playerName = n;
    }

    public void SyncName()
    {
        if (!string.IsNullOrEmpty(playerName))
        {
            GameManager.singleton.GetLocalPlayer().photonView.RPC("UpdateName", RpcTarget.AllBuffered, playerName);
            playerName = "";
        }
        
        GameManager.singleton.pause = false;
        customize.SetActive(false);
    }

    private KeyValuePair<bool, float> ConvertToNumber(string input)
    {
        try
        { 
            return new KeyValuePair<bool, float>(true, float.Parse(input));
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return new KeyValuePair<bool, float>(false, 0);
        } 
    }

    public void Meeting(PlayerManager player)
    {
        meeting.SetActive(true);

        deadMask.SetActive(GameManager.singleton.voteState == VoteState.Dead);


        List<PlayerManager> players = GameManager.singleton.totalPlayers;
        players.Sort(new PlayerCompare());
        
        foreach (PlayerManager p in players)
        {
            if (p == player)
            {
                Instantiate(votingUIObject, votingUIObjectParent).GetComponent<VotingUIObject>().InitializeReport(p);
            }
            else
            {
                Instantiate(votingUIObject, votingUIObjectParent).GetComponent<VotingUIObject>().Initialize(p);
            }
        }
    }

    public void FinishMeeting(PlayerManager eject)
    {
        Transform[] children = votingUIObjectParent.GetComponentsInChildren<Transform>();

        for (int i = 1; i < children.Length; i++)
        {
            Destroy(children[i].gameObject);
        }
        
        meeting.SetActive(false);

        StartCoroutine(DisplayEject(eject));
    }

    private IEnumerator DisplayEject(PlayerManager eject)
    {
        displayEject.SetActive(true);

        if (eject == null)
            displayEjectText.text = "No one was ejected.";
        else
            displayEjectText.text = eject.name + " was ejected.";
        
        yield return new WaitForSeconds(4);
            
        displayEject.SetActive(false);
    }

    public void VotingTimer(int time)
    {
        votingTimerText.text = time.ToString();
    }
    public IEnumerator GameEnd(bool crewmate)
    {
        impostorWin.SetActive(!crewmate);
        crewmateWin.SetActive(crewmate);
        
        yield return new WaitForSeconds(4);
        
        impostorWin.SetActive(false);
        crewmateWin.SetActive(false);
    }

    public void ToggleMap()
    {
        map.SetActive(!map.activeSelf);
    }

    public void UpdateMap(Vector3 position)
    {
        if (map.activeSelf)
        {
            playerMap.anchoredPosition = new Vector2(position.x * 11, position.z * 11);
        }
    }

    public void UpdateTasks(List<TaskData> tasks)
    {
        foreach (Transform t in mapTaskParent.GetComponentInChildren<Transform>())
        {
            if(t != mapTaskParent)
                Destroy(t.gameObject);
        }
        
        foreach (TaskData t in tasks)
        {
            if(t != null)
                Instantiate(mapTaskIcon, mapTaskParent).GetComponent<RectTransform>().anchoredPosition = 
                    new Vector2(t.position.x * 11, t.position.z * 11);
        }
    }
}
