using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class AdminTable : MonoBehaviour
{
    [SerializeField] private Transform cafeteria, admin, oxygen, weapons, nav, shields, comms, storage, 
        electrical, lowerEngine, upperEngine, reactor, security, medbay;

    [SerializeField] private GameObject playerIcon;
    [SerializeField] private List<GameObject> securityIndicatorLights;
    private List<GameObject> icons = new List<GameObject>();

    [SerializeField] private List<GameObject> commsDown;

    public void UpdateTable()
    {
        foreach (GameObject i in icons)
        {
            ToggleCams(false);

            Destroy(i);
        }
        
        List<PlayerManager> players = GameManager.singleton.totalPlayers;

        foreach (PlayerManager p in players)
        {
            if (!p.isDead || p.room != Room.NONE)
            {
                switch (p.room)
                {
                    case(Room.ADMIN):
                        icons.Add(Instantiate(playerIcon, admin));
                        break;
                    case(Room.CAFETERIA):
                        icons.Add(Instantiate(playerIcon, cafeteria));
                        break;
                    case(Room.OXYGEN):
                        icons.Add(Instantiate(playerIcon, oxygen));
                        break;
                    case(Room.WEAPONS):
                        icons.Add(Instantiate(playerIcon, weapons));
                        break;
                    case(Room.NAVIGATION):
                        icons.Add(Instantiate(playerIcon, nav));
                        break;
                    case(Room.SHIELDS):
                        icons.Add(Instantiate(playerIcon, shields));
                        break;
                    case(Room.COMMS):
                        icons.Add(Instantiate(playerIcon, comms));
                        break;
                    case(Room.STORAGE):
                        icons.Add(Instantiate(playerIcon, storage));
                        break;
                    case(Room.ELECTRICAL):
                        icons.Add(Instantiate(playerIcon, electrical));
                        break;
                    case(Room.LOWER_ENGINE):
                        icons.Add(Instantiate(playerIcon, lowerEngine));
                        break;
                    case(Room.UPPER_ENGINE):
                        icons.Add(Instantiate(playerIcon, upperEngine));
                        break;
                    case(Room.REACTOR):
                        icons.Add(Instantiate(playerIcon, reactor));
                        break;
                    case(Room.SECURITY):
                        icons.Add(Instantiate(playerIcon, security));
                        ToggleCams(true);
                        break;
                    case(Room.MEDBAY):
                        icons.Add(Instantiate(playerIcon, medbay));
                        break;
                }
            }
        }
    }
    
    private void ToggleCams(bool state)
    {
        foreach (GameObject c in securityIndicatorLights)
        {
            c.SetActive(state);
        }
    }

    private void Update()
    {
        foreach (GameObject c in commsDown)
        {
            c.SetActive(SabotageManager.singleton.commsDown);
        }
    }
}
