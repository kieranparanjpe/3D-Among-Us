using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Keypad : TaskUI
{
    protected string code;
    protected string currentCode;
    [SerializeField] private int length;
    [SerializeField] private TextMeshProUGUI currentCodeText;
    public override void ShowTask(Task task)
    {
        base.ShowTask(task);

        code = "";
        currentCode = "";
        currentCodeText.text = currentCode;

        for (int i = 0; i < length; i++)
        {
            int r = Random.Range(0, 10);
            code += r.ToString();
        }
    }

    public virtual void Input(string digit)
    {
        currentCode += digit;

        currentCodeText.text = currentCode;

        if (code == currentCode)
        {
            Debug.Log("Keypad Complete");
            CompleteTask();
        
        }
    }

    public void Clear()
    {
        currentCode = "";
        currentCodeText.text = currentCode;
    }
}
