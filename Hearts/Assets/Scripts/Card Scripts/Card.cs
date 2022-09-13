using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum CardOwner {  South, West, East, North, Deck, Discard}

public enum CardID { Ace_Spades = 1, Two_Spades = 2, Three_Spades = 3, Four_Spades = 4, Five_Spades = 4, Six_Spades = 6, Seven_Spades = 7, Eight_Spades = 8, Nine_Spades = 9, Ten_Spades = 10, Jack_Spades = 11, Queen_Spades = 12, King_Spades= 13,
                    Ace_Clubs = 14, Two_Clubs = 15, Three_Clubs = 16, Four_Clubs = 17, Five_Clubs = 18, Six_Clubs = 19, Seven_Clubs = 20, Eight_Clubs = 21, Nine_Clubs = 22, Ten_Clubs = 23, Jack_Clubs = 24, Queen_Clubs = 25, King_Clubs = 26,
                    Ace_Diamonds = 27, Two_Diamonds = 28, Three_Diamonds = 29, Four_Diamonds = 30, Five_Diamonds = 31, Six_Diamonds = 32, Seven_Diamonds = 33, Eight_Diamonds = 34, Nine_Diamonds = 35, Ten_Diamonds = 36, Jack_Diamonds = 37, Queen_Diamonds = 38, King_Diamonds = 39,
                    Ace_Hearts = 40, Two_Hearts = 41, Three_Hearts = 42, Four_Hearts = 43, Five_Hearts = 44, Six_Hearts = 45, Seven_Hearts = 46, Eight_Hearts = 47, Nine_Hearts = 48, Ten_Hearts = 49, Jack_Hearts = 50, Queen_Hearts = 51, King_Hearts = 52,
}

[System.Serializable]
public class Card : MonoBehaviour, IComparable<Card>
{
    [SerializeField]
    private CardUI cardUI;

    [SerializeField]
    private CardUI cardUIBack;

    [SerializeField, ReadOnly] 
    private Suit cardSuit;
    
    [SerializeField] 
    private int cardRank;

    [SerializeField]
    private int cardWeight;

    [SerializeField] 
    [ReadOnly]
    private Player currentOwner;

    [SerializeField]
    [ReadOnly]
    private CardID cardID;


    private void Awake()
    {
        cardUI = GetComponent<CardUI>();
    }

    public void SetCard(Suit cardSuit, int cardRank, Player currentOwner, CardID cardID)
    {
        this.cardSuit = cardSuit;
        this.currentOwner = currentOwner;
        this.cardID = cardID;
     
        if (cardRank == 1)
            this.cardRank = 14;
        else
            this.cardRank = cardRank;
    
    }

    public void SetCardSortingLayer(int layer)
    {
        cardUI.CardRenderer.sortingOrder = layer;
    }

    public void SetCardWeight(int cardWeight)
    {
        this.cardWeight = cardWeight;
    }

    public bool AnimateCard(Vector2 destination, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, speed);

        if (Vector2.Distance(transform.position, destination) < 0.001f)
        {
            return true;
        }

        return false;

    }

    public Suit CardSuit { get => cardSuit;}
    public int CardRank { get => cardRank;}
    public Player CurrentOwner { get => currentOwner; set => currentOwner = value; }
    public CardUI CardUI { get => cardUI;}
    public CardID CardID { get => cardID;}

    public int CompareTo(Card y)
    {
        return (cardSuit.CompareTo(y.cardSuit) * 100) + cardRank.CompareTo(y.cardRank);
    }
}
