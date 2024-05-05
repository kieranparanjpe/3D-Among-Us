using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] Room room;
    [SerializeField] private AdminTable table;
    
    public void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponentInParent<PlayerManager>();
        if (player != null && !player.isDead)
        {
            player.room = room;
            table.UpdateTable();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        PlayerManager player = other.GetComponentInParent<PlayerManager>();
        if (player != null && !player.isDead)
        {
            player.room = Room.NONE;
            table.UpdateTable();
        }
    }
}

public enum Room
{    
    ADMIN,
    CAFETERIA,
    COMMS,
    ELECTRICAL,
    LOWER_ENGINE,
    MEDBAY,
    NAVIGATION,
    OXYGEN,
    REACTOR,
    SECURITY,
    SHIELDS,
    UPPER_ENGINE,
    WEAPONS,
    NONE,
    STORAGE
}
