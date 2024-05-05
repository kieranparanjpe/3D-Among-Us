using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskRefillGas : TaskUI
{

    [SerializeField] private Slider progress;
    public override void ShowTask(Task task)
    {
        base.ShowTask(task);

        progress.value = 0;
    }

    public void Increase()
    {
        progress.value += 0.1f;

        if (progress.value >= 1)
        {
            CompleteTask();
        }
    }
}
