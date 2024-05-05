using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskTimedButton : TaskUI
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private float time;
    
    public override void ShowTask(Task task)
    {
        base.ShowTask(task);

        progressBar.value = 0;
        progressBar.maxValue = time;
    }

    public void PressButton()
    {
        progressBar.value += Time.deltaTime;
    }

    protected void Update()
    {
        base.Update();

        if (progressBar.value > 0)
        {
            progressBar.value += Time.deltaTime;
        } 
        if (progressBar.value >= time)
        {
            CompleteTask();
        }
    }
}
