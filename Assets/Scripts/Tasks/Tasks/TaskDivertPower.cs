using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskDivertPower : TaskUI
{
    [SerializeField] private Slider[] sliders;
    
    public override void ShowTask(Task task)
    {
        base.ShowTask(task);

        foreach (var slider in sliders)
        {
            slider.value = 1;
        }
        sliders[Random.Range(0, 5)].value = 0;
    }

    public void ChangeValue()
    {
        foreach (var slider in sliders)
        {
            if (slider.value < 0.98f)
                return;
        }
        
        CompleteTask();
    }
}
