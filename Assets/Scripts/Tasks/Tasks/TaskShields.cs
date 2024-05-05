using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskShields : TaskUI
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private bool nav;

    public override void ShowTask(Task task)
    {
        base.ShowTask(task);

        foreach (var b in buttons)
        {
            if (nav)
                b.interactable = Random.Range(0, 2) == 0;
            else
                b.interactable = true;
        }
    }

    public void UpdateShield(int index)
    {
        buttons[index].interactable = false;
        
        foreach (var b in buttons)
        {
            if (b.interactable)
                return;
        }
        
        CompleteTask();
    }
}
