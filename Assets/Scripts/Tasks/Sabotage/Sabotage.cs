using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sabotage : Task
{
    private PlayerImpostor playerImpostor;
    public override void Interact(PlayerTasks tasks, PlayerImpostor impostor)
    {
        playerTasks = tasks;
        playerImpostor = impostor;
        if (tasks.tasks.Contains(taskData) || impostor.tasks.Contains(taskData))
        {
            DoTask();
        }
    }

    public override void CompleteTask()
    {
        task.gameObject.SetActive(false);
        GameManager.singleton.pause = false;
        playerTasks.tasks.Remove(taskData);
        playerImpostor.tasks.Remove(taskData);
        if (taskData.nextTask != null)
            playerTasks.tasks.Add(taskData.nextTask);

        SabotageManager.singleton.CompleteSabotage(taskData);
        UIManager.singleton.UpdateTasks(playerTasks.tasks);
    }
}
