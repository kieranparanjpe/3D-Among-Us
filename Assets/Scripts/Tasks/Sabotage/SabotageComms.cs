using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SabotageComms : TaskUI
{
    private int targetA, targetB, targetC;
    [SerializeField] private Slider sliderA, sliderB, sliderC;
    [SerializeField] private Slider sliderTargetA, sliderTargetB, sliderTargetC;

    public override void ShowTask(Task task)
    {
        base.ShowTask(task);

        targetA = Random.Range(0, 10);
        targetB = Random.Range(0, 10);
        targetC = Random.Range(0, 10);

        sliderTargetA.value = targetA;
        sliderTargetB.value = targetB;
        sliderTargetC.value = targetC;

    }

    public void CheckForWin()
    {
        if (sliderA.value == targetA && sliderB.value == targetB && sliderC.value == targetC)
        {
            CompleteTask();
        }
    }
}
