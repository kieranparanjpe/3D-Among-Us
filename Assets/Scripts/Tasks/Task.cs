using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Task : MonoBehaviour
{
    [SerializeField] public TaskData taskData;
    [SerializeField] protected TaskUI task;

    protected PlayerTasks playerTasks;

    private void Start()
    {
        taskData.position = transform.position;
    }
    
    public virtual void Interact(PlayerTasks tasks, PlayerImpostor impostor)
    {
        playerTasks = tasks;
        if (tasks.tasks.Contains(taskData))
        {
            DoTask();
        }
    }

    protected void DoTask()
    {
        task.gameObject.SetActive(true);
        task.ShowTask(this);
        GameManager.singleton.pause = true;
    }

    public virtual void CompleteTask()
    {
        task.gameObject.SetActive(false);
        GameManager.singleton.pause = false;
        playerTasks.tasks.Remove(taskData);
        if (taskData.nextTask != null)
            playerTasks.tasks.Add(taskData.nextTask);
        else
            GameManager.singleton.IncreaseTaskBar();
        
        UIManager.singleton.UpdateTasks(playerTasks.tasks);
    }

    public virtual void CloseTask()
    {
        task.gameObject.SetActive(false);
        GameManager.singleton.pause = false;
    }
}
