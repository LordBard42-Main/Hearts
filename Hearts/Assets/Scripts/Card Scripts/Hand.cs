using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpacingDirection { Horizontal, Vertical}

[System.Serializable]
public class Hand
{

    [SerializeField] private List<Card> cards = new List<Card>();
    [SerializeField] private SpacingDirection spacingDirection;

    [SerializeField] private CardVisibility handVisibility;

    
    private Dictionary<Suit, int> suitCount = new Dictionary<Suit, int>
    {
        {Suit.Clubs, 0 },
        {Suit.Spades, 0 },
        {Suit.Diamonds, 0 },
        {Suit.Hearts, 0 }

    };


    [BoxGroup("Card UI")]
    [SerializeField]
    private int layer;

    private readonly float CARDSPACING = 1.8F;




    public void AddToHand(Card card)
    {
        card.GetComponentInChildren<SpriteRenderer>().sortingOrder = cards.Count;
        suitCount[card.CardSuit]++;
        cards.Add(card);
        card.CardUI.UpdateCardSprite(handVisibility);

    }

    public void UpdateHandUI()
    {
        int i = 0;

        switch (spacingDirection)
        {
            case SpacingDirection.Horizontal:
                foreach (Card card in cards)
                {
                    card.transform.localPosition = new Vector3(-3 + (i / (CARDSPACING)), 0,13 - i);
                    card.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = i;
                    i++;
                }
                break;
            case SpacingDirection.Vertical:
                foreach (Card card in cards)
                {

                    card.transform.localPosition = new Vector3(0, 3 - (i / (CARDSPACING)), 13- i);
                    card.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = i;
                    i++;
                }
                break;
            default:
                break;
        }

      
    }

    public void Sort()
    {
        cards.Sort();
        UpdateHandUI();
    }

    public void RemoveFromHand(Card card)
    {
        if (cards.Contains(card))
        {
            suitCount[card.CardSuit]--;
            cards.Remove(card);
        }

    }

    public Card CheckForCard(CardID cardID)
    {
        foreach(Card card in cards)
        {
            if(card.CardID == cardID)
            {
                return card;
            }
        }

        return default(Card);
    }

    public int GetSuitCount(Suit suit)
    {
        return suitCount[suit];
    }

    public List<Card> GetCardsBySuit(Suit suit)
    {
        var cards = new List<Card>();

        foreach (var card in this.cards)
        {
            if (card.CardSuit == suit)
                cards.Add(card);
        }
        return cards;
    }

    public List<Card> GetCardsByNotSuit(Suit suit)
    {
        var cards = new List<Card>();

        foreach (var card in this.cards)
        {
            if (card.CardSuit != suit)
                cards.Add(card);
        }
        return cards;
    }

    public List<Card> GetCardsForFirstRoundLead()
    {
        var cards = new List<Card>();

        foreach (var card in this.cards)
        {
            if (card.CardSuit != Suit.Hearts || card.CardID != CardID.Queen_Spades)
                cards.Add(card);
        }
        return cards;
    }

    public void UpdateCardWeight(Card card)
    {
        


    }

    public void ChangeHandVisibility(CardVisibility visibility)
    {
        foreach(Card card in cards)
        {
            card.CardUI.UpdateCardSprite(visibility);
        }
    }

    public List<Card> Cards { get => cards;}
    public Dictionary<Suit, int> SuitCount { get => suitCount; }
}
