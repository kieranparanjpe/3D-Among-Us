using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VotingUIObject : MonoBehaviour
{
    private PlayerManager player;

    [SerializeField] private GameObject confirmButton;
    [SerializeField] private GameObject report;
    [SerializeField] private GameObject deadMask;

    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI name;
    public void Initialize(PlayerManager p)
    {
        player = p;
        icon.color = p.colour;
        name.text = p.name;
        
        deadMask.SetActive(player.isDead);
        button.interactable = !player.isDead;
    }
    
    public void InitializeReport(PlayerManager p)
    {
        player = p;
        icon.color = p.colour;
        name.text = p.name;
        
        report.SetActive(true);
        
        deadMask.SetActive(player.isDead);
        button.interactable = !player.isDead;
    }

    public void MainClick()
    {
        confirmButton.SetActive(!confirmButton.activeSelf);
    }

    public void Confirm()
    {
        if(GameManager.singleton.voteState == VoteState.CanVote)
            GameManager.singleton.CastVote(player);
        
        confirmButton.SetActive(false);
    }
}
