using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player { South = 6, West = 7, East = 8, North = 9, None}
public class PlayerController : MonoBehaviour
{
    [SerializeField] 
    private Transform tableArea;

    [SerializeField] 
    private Hand playerHand;

    [SerializeField] 
    private bool isAI;

    [SerializeField] 
    private AILevels aILevel;

    [SerializeField] 
    private Player player;

    public void AddToHand(Card card)
    {
        card.transform.parent = transform;
        card.transform.GetChild(0).gameObject.layer = (int)Player;
        card.CurrentOwner = player;
        playerHand.AddToHand(card);
    }
    public void RemoveFromoHand(Card card)
    {
        playerHand.RemoveFromHand(card);
    }

    public void PlayCard(Card card)
    {
        card.transform.parent = tableArea;
        HeartsTable.instance.AddCardToTable(card);
        playerHand.RemoveFromHand(card);
        playerHand.UpdateHandUI();
    }

    public Hand PlayerHand { get => playerHand; set => playerHand = value; }
    public Player Player { get => player; }
    public bool IsAI { get => isAI; set => isAI = value; }
    public AILevels AILevel { get => aILevel; }
}
