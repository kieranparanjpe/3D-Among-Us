using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskCardSwipe : Keypad
{
    [SerializeField] TextMeshProUGUI displayCode;

    public override void ShowTask(Task task)
    {
        base.ShowTask(task);

        displayCode.text = "ID Number: " + code;
    }
}
