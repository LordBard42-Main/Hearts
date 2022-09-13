using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishSetState : IState
{
    private HeartsTable heartsTable;

    private Vector2 cardDestination;
    private readonly float ANIMATIONSPEED = .5f;

    private Player winner;

    private bool nextHand = false;
    private bool pause;
    private readonly float waitTime = .5f;

    public FinishSetState(HeartsTable heartsTable)
    {
        this.heartsTable = heartsTable;
    }


    public void OnEnter()
    {
        pause = true;
        winner = heartsTable.FinishSet();
        Debug.Log(winner);
        switch (winner)
        {
            case Player.South:
                cardDestination = heartsTable.SouthCardsWonLocation.position;
                break;
            case Player.West:
                cardDestination = heartsTable.WestCardsWonLocation.position;
                break;
            case Player.East:
                cardDestination = heartsTable.EastCardsWonLocation.position;
                break;
            case Player.North:
                cardDestination = heartsTable.NorthCardsWonLocation.position;
                break;
            case Player.None:
                break;
            default:
                break;
        }

        heartsTable.StartCoroutine(Wait());
    }


    public void OnExit()
    {
        nextHand = false;
    }

    public void Tick()
    {
        if(!heartsTable.ActiveAnimations)
        {
            heartsTable.PrepareNextHand(winner);
            nextHand = true;
            return;
        }


        if (!pause)
        {
            bool allCardsDesitnationReached = true;

            foreach(Card card in heartsTable.CardsPlayed)
            {
                var destinationReached = card.AnimateCard(cardDestination, ANIMATIONSPEED);

                if(!destinationReached)
                    allCardsDesitnationReached = false;
                
                
            }

            if (allCardsDesitnationReached)
            {

                heartsTable.PrepareNextHand(winner);
                nextHand = true;
            }


        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitTime);
        pause = false;
    }
    public bool NextHand { get => nextHand; private set => nextHand = value; }

}
