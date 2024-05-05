using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWiring : TaskUI
{
    [SerializeField] private GameObject wiresA;
    [SerializeField] private GameObject wiresB;
    [SerializeField] private RectTransform[] wires;

    public RectTransform[] childrenA = null;
    public RectTransform[] childrenB = null;

    private string currentColour = "";
    private int count = 0;

    private bool trackWire = false;
    
    public override void ShowTask(Task task)
    {
        base.ShowTask(task);
        currentColour = "";
        count = 0;
        Debug.Log("Task");

        foreach (RectTransform w in wires)
        {
            w.GetComponent<Image>().color = Color.white;
            w.SetPositionAndRotation(new Vector2(0, 0), Quaternion.Euler(new Vector2(0, 0)));
            w.localScale = new Vector3(1, 1, 1);
        }
        
        if(childrenA.Length == 0)
            childrenA = wiresA.GetComponentsInChildren<RectTransform>();
        List<int> set = new List<int>() {0, 1, 2, 3};
        for (int i = 1; i < childrenA.Length; i+=2)
        {
            int random = set[Random.Range(0, set.Count)];
            childrenA[i].SetSiblingIndex(random);
            set.Remove(random);
        }
        
        if(childrenB.Length == 0)
            childrenB = wiresB.GetComponentsInChildren<RectTransform>();
        set = new List<int>() {0, 1, 2, 3};
        for (int i = 1; i < childrenB.Length; i+=2)
        {
            int random = set[Random.Range(0, set.Count)];
            childrenB[i].SetSiblingIndex(random);
            set.Remove(random);
        }
    }

    private void Update()
    {
        base.Update();
        
        if (trackWire)
        {
            int index = 1 + (int.Parse(currentColour) * 2);
            TrackWire(childrenA[index].position, Input.mousePosition, currentColour);
        }
    }

    public void AClick(string colour)
    {
        currentColour = colour;

        trackWire = true;
    }

    public void BClick(string colour)
    {
        if (currentColour != colour)
            return;

        trackWire = false;
        int index = 1 + (int.Parse(currentColour) * 2);
        TrackWire(childrenA[index].position, childrenB[index].position, colour);
        count++;
        
        if(count >= 4)
            CompleteTask();
    }

    private void TrackWire(Vector3 positionA, Vector3 positionB, string colour)
    {
        Vector3 position = positionA + new Vector3(0, 35/2, 0);
        Quaternion rotation =
            Quaternion.LookRotation(Vector3.forward, (positionB - positionA));
        rotation.eulerAngles += new Vector3(0, 0, 90);
        wires[count].SetPositionAndRotation(position, rotation);
        wires[count].localScale = new Vector3(Vector3.Distance(positionB , position), 1, 1);
        switch (colour)
        {
            case "0":
                wires[count].GetComponent<Image>().color = Color.red;
                break;
            case "1":
                wires[count].GetComponent<Image>().color = Color.blue;
                break;
            case "2":
                wires[count].GetComponent<Image>().color = Color.magenta;
                break;
            case "3":
                wires[count].GetComponent<Image>().color = Color.yellow;
                break;
        }
    }

}
