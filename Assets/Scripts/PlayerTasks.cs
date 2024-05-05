using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTasks : MonoBehaviour
{
    public List<TaskData> tasks = new List<TaskData>();

    private void OnEnable()
    {
        UIManager.singleton.SetCrosshairColour(Color.white);
    }
}
