using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealCardsState : IState
{
    private readonly HeartsTable heartsTable;

    public bool areCardsDealt = false;
    private Deck deck;

    private PlayerController southPlayerController;
    private PlayerController westPlayerController;
    private PlayerController eastPlayerController;
    private PlayerController northPlayerController;

    public bool readyToExit = false;



    public DealCardsState(Deck deck, PlayerController southPlayerController, PlayerController westPlayerController, PlayerController eastPlayerController, PlayerController northPlayerController, HeartsTable heartsTable)
    {
        this.deck = deck;
        this.southPlayerController = southPlayerController;
        this.westPlayerController = westPlayerController;
        this.northPlayerController = northPlayerController;
        this.eastPlayerController = eastPlayerController;
        this.heartsTable = heartsTable;
    }

    public void OnEnter()
    {
        areCardsDealt = false;
        //Debug.Log("Deal State entered");
        

        deck.ResetDeck();
        heartsTable.LeadingPlayer = Player.None;
        heartsTable.HardDeal = false;

        for (int i = 0; i < 13; i++)
        {
            var southCard = deck.DealCard(Player.South);
            var westCard = deck.DealCard(Player.West);
            var eastCard = deck.DealCard(Player.East);
            var northCard = deck.DealCard(Player.North);

            if (heartsTable.LeadingPlayer == Player.None)
            {
                if (southCard.CardSuit == Suit.Clubs && southCard.CardRank == 2)
                    heartsTable.LeadingPlayer = Player.South;
                else if (westCard.CardSuit == Suit.Clubs && westCard.CardRank == 2)
                    heartsTable.LeadingPlayer = Player.West;
                else if (eastCard.CardSuit == Suit.Clubs && eastCard.CardRank == 2)
                    heartsTable.LeadingPlayer = Player.East;
                else if (northCard.CardSuit == Suit.Clubs && northCard.CardRank == 2)
                    heartsTable.LeadingPlayer = Player.North;

            }

            southPlayerController.AddToHand(southCard);
            westPlayerController.AddToHand(westCard);
            eastPlayerController.AddToHand(eastCard);
            northPlayerController.AddToHand(northCard);
        }

        southPlayerController.PlayerHand.Sort();
        westPlayerController.PlayerHand.Sort();
        eastPlayerController.PlayerHand.Sort();
        northPlayerController.PlayerHand.Sort();

        //southPlayerController.PlayerHand.ChangeHandVisibility(CardVisibility.Visible);

        heartsTable.ResetSetPlay();
        areCardsDealt = true;
    }
    
    public void OnExit()
    {

    }

    public void Tick()
    {

    }


}
