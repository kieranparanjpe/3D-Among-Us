using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TaskUI : MonoBehaviour
{
    protected Task task;

    protected void Update()
    {
        if(GameManager.singleton.inMeeting)
            gameObject.SetActive(false);
        if(!GameManager.singleton.started)
            Close();
    }
    
    public virtual void ShowTask(Task task)
    {
        this.task = task;
    }

    protected virtual void CompleteTask()
    {
        task.CompleteTask();
    }

    public void Close()
    {
        task.CloseTask();
    }
}

