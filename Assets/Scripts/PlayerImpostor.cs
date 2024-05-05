using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerImpostor : MonoBehaviour
{
    public List<TaskData> tasks = new List<TaskData>();

    [SerializeField] private new Transform camera;
    private float killDistance;
    private int killCooldown;

    private float lastKill;
    
    private void OnEnable()
    {
        UIManager.singleton.SetCrosshairColour(Color.red);
        
        killCooldown = (int)GameManager.singleton.output["KCD"];
        killDistance = (float)GameManager.singleton.output["KDIST"];

        lastKill = 15;
    }
    
    private void Update()
    {
        lastKill -= Time.deltaTime;
        UIManager.singleton.UpdateKillCooldown((int)lastKill);
        RaycastHit hit;
        if (lastKill <= 0 && Input.GetButtonDown("Kill") && Physics.Raycast(camera.position, camera.forward, out hit, killDistance))
        {
            Debug.Log("Tried to kill: " + hit.transform.gameObject.name);
            PlayerManager player = hit.transform.GetComponent<PlayerManager>();
            if (player != null && player.crewmate)
            {
                lastKill = killCooldown;
                player.Die();
            }
        }

        if (SabotageManager.singleton.canSabotage)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SabotageManager.singleton.Reactor();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SabotageManager.singleton.Lights();

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SabotageManager.singleton.Comms();

            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SabotageManager.singleton.Oxygen();

            }
            
        }
    }
    
}
