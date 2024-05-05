using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Task", menuName = "Task")]
public class TaskData : ScriptableObject
{
    public string id = "";
    public string name = "";

    public TaskData nextTask = null;
    public bool isCommon = false;
    public Vector3 position;
}
