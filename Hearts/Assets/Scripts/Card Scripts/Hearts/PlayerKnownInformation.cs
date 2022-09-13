using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerKnownInformation
{
    [SerializeField]
    private List<Card> knownCards = new List<Card>();

    [SerializeField]
    private List<Card> potentialCards = new List<Card>();
    /*
    [SerializeField]
    private Dictionary<Suit, bool> suitEmpty = new Dictionary<Suit, bool>
    {
        {Suit.Clubs, false },
        {Suit.Spades, false },
        {Suit.Diamonds, false },
        {Suit.Hearts, false }
    };
    */
    [SerializeField]
    private List<Suit> emptySuits = new List<Suit>();

                







    public List<Suit> GetEmptySuits()
    {
        return emptySuits;
    }

    public void AddEmptySuit(Suit suit)
    {
        if(!emptySuits.Contains(suit))
            emptySuits.Add(suit);
    }

    public bool GetSuitStatus(Suit suit)
    {
        return emptySuits.Contains(suit);
    }

    public void ResetSuitStatus()
    {
        emptySuits.Clear();
    }

    public List<Card> GetPotentialCards()
    {
        return potentialCards;
    }

    public void RemoveFromPotentialCards(Card card)
    {
        potentialCards.Remove(card);
    }

    public void AddToPotentialCards(Card card)
    {
        potentialCards.Add(card);
    }


    public List<Card> GetKnownCards()
    {
        return knownCards;
    }

    public void RemoveFromKnownCards(Card card)
    {
        knownCards.Remove(card);
    }

    public void AddToKnownCards(Card card)
    {
        knownCards.Add(card);
    }

}
