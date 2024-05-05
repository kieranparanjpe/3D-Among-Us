using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SabotageLights : TaskUI
{
    [SerializeField] private Image[] lights;

    public override void ShowTask(Task task)
    {
        base.ShowTask(task);
        int c = 0;
        foreach (Image i in lights)
        {
            bool on = Random.Range(0, 2) == 1;

            i.color = on ? Color.green : new Color(0, 0, 0);

            if (on)
                c++;
        }

        if (c == 0)
            lights[0].color = Color.green;
        else if (c == 7)
            lights[0].color = new Color(0, 0, 0);
    }

    public void Switch(int id)
    {
        lights[id].color = lights[id].color == Color.green ? new Color(0, 0, 0) : Color.green;
        
        int c = 0;
        foreach (Image i in lights)
        {
            bool on = i.color == Color.green;
            
            if (on)
                c++;
        }

        if (c == 7)
        {
            CompleteTask();
        }
    }
}
