using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : IState
{
    private HeartsTable heartsTable;
    private PlayerController playerController;
    private Camera camera;
    private LayerMask layerMask;
    private Vector2 cardLocation;

    private bool playCard = false;
    private Card cardPlayed;

    private readonly float ANIMATIONSPEED = .25f;

    public bool nextPlayersTurn = false;

    public PlayerTurnState(Camera camera, LayerMask layerMask, Vector2 cardLocation, PlayerController playerController, HeartsTable heartsTable)
    {
        this.camera = camera;
        this.layerMask = layerMask;
        this.cardLocation = cardLocation;
        this.playerController = playerController;
        this.heartsTable = heartsTable;
    }

    public void OnEnter()
    {
        //Debug.Log("Entered " + playerController.Player + " Player State");
        
        if (playerController.IsAI)
        {
           
            cardPlayed = heartsTable.AIPlayCard(playerController);           
            playerController.PlayCard(cardPlayed);
            
            
            if (heartsTable.ActiveAnimations)
            {
                playCard = true;
            }
            else
            {
                cardPlayed.transform.position = cardLocation;
                nextPlayersTurn = true;
                heartsTable.TurnChangeReady = true;
            }
        }


    }

    public void OnExit()
    {
        nextPlayersTurn = false;
        playCard = false;
        heartsTable.TurnChangeReady = false;
    }

    public void Tick()
    {
        if (!playerController.IsAI && !playCard)
        {
            RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);

            if (hit.collider != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Transform objectHit = hit.transform.parent;

                    if(objectHit.TryGetComponent(out Card card))
                    {
                        if(heartsTable.CheckIfPlayIsAllowed(card, playerController))
                        {
                            cardPlayed = card;
                            playerController.PlayCard(card);

                            
                            playCard = true;
                        }
                    }
                }

            }

        }

        if (playCard)
        {
            var destinationReached = cardPlayed.AnimateCard(cardLocation, ANIMATIONSPEED);

            if (destinationReached)
            {
                nextPlayersTurn = true;
                heartsTable.TurnChangeReady = true;
            }


        }
    }

   
}
