using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMedBay : TaskUI
{
    [SerializeField] private Animator animator;
    private float t;
    public override void ShowTask(Task task)
    {
        base.ShowTask(task);
        animator.SetTrigger("Start");
        t = 0;
    }

    protected void Update()
    {
        base.Update();

        t += Time.deltaTime;
        
        if(t >= 4)
            CompleteTask();
    }
}
