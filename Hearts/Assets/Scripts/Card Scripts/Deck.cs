using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Suit { Clubs = 0, Hearts = 13, Spades = 26, Diamonds = 39, None = 1000}

public class Deck : MonoBehaviour
{
    private readonly float DECKSPACING = 1.01F;

    Suit suit;
    [SerializeField] 
    private List<Card> cards = new List<Card>();
    
    [SerializeField] 
    private List<Card> discard = new List<Card>();


    [SerializeField]
    private Transform cardPrefab;

    private new Transform transform;


    private void Awake()
    {
        transform = GetComponent<Transform>();
        StartDeck();


    }

    private void Start()
    {
       
    }

    public void StartDeck()
    {
        
        for (int i = 0; i < 13; i++)
        {
            Transform cardTransform;
            Card card;




            cardTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
            card = cardTransform.GetComponent<Card>();

            card.SetCard(Suit.Spades, i + 1, Player.None, (CardID)(i + 1));
            cardTransform.GetComponent<CardUI>().SetCardUI(GameSprites.instance.Spades[i], GameSprites.instance.CardBack,  i + 3);
            SetName(cardTransform.gameObject, card);
            
            cards.Add(card);


            cardTransform = Instantiate(cardPrefab, new Vector3(0,0, 0), Quaternion.identity, transform);
            card = cardTransform.GetComponent<Card>();

            card.SetCard(Suit.Clubs, i + 1, Player.None, (CardID)(i+14));
            cardTransform.GetComponent<CardUI>().SetCardUI(GameSprites.instance.Clubs[i], GameSprites.instance.CardBack, i + 1);
            SetName(cardTransform.gameObject, card);

            cards.Add(card);


            cardTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
            card = cardTransform.GetComponent<Card>();

            card.SetCard(Suit.Diamonds, i + 1, Player.None, (CardID)(i + 27));
            cardTransform.GetComponent<CardUI>().SetCardUI(GameSprites.instance.Diamonds[i], GameSprites.instance.CardBack, i + 4);
            SetName(cardTransform.gameObject, card);

            cards.Add(card);


            cardTransform = Instantiate(cardPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform);
            card = cardTransform.GetComponent<Card>();

            card.SetCard(Suit.Hearts, i + 1,  Player.None, (CardID)(i + 40));
            cardTransform.GetComponent<CardUI>().SetCardUI(GameSprites.instance.Hearts[i], GameSprites.instance.CardBack, i + 2);
            SetName(cardTransform.gameObject, card);

            cards.Add(card);
        }

        

        ShuffleCards();

        void SetName(GameObject gameObject, Card card)
        {
            string rank = card.CardRank.ToString();

            switch (rank)
            {
                case "14":
                    rank = "Ace";
                    break;
                case "11":
                    rank = "Jack";
                    break;
                case "12":
                    rank = "Queen";
                    break;
                case "13":
                    rank = "King";
                    break;
                default:
                    break;
            }
            gameObject.name = (rank + " of " + card.CardSuit);
        }


    }

    public Card DealCard(Player owner)
    {
        var card = cards[cards.Count - 1];
        card.CurrentOwner = owner;
        cards.RemoveAt(cards.Count - 1);
        
        if(!card.gameObject.activeSelf)
        {
            card.gameObject.SetActive(true);
        }

;       return card;
    }

    public void ShuffleCards()
    {
        if (cards.Count > 0)
        {
            var index = Random.Range(0, cards.Count);
            var card = cards[index];

            cards.RemoveAt(index);

            ShuffleCards();

            card.CardUI.UpdateSortingLayer(cards.Count);
            cards.Add(card);
        }
        else
        {
            //Debug.Log("Stack Shuffled");
        }
    }

    public void AddToDiscard(Card card)
    {
        Discard.Add(card);
    }
    public void RemoveFromDiscard(Card card)
    {
        Discard.Remove(card);
    }
    public void AddToDeck(Card card)
    {
        cards.Add(card);
    }

    public void ResetDeck()
    {
        if (discard.Count > 0)
        {
            for (int i = discard.Count - 1; i >= 0; i--)
            {
                var card = discard[i];
                RemoveFromDiscard(card);
                AddToDeck(card);
            }
        }
        ShuffleCards();
    }

    public Card DrawFromDiscard()
    {
        var card = Discard[discard.Count];
        discard.RemoveAt(discard.Count);
        return card;
    }

    public List<Card> Cards { get => cards; }
    public List<Card> Discard { get => discard; }
}
