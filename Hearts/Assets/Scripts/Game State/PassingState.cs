using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PassingDirection { Left, Right, Up, Hold}
public class PassingState : IState
{
    HeartsTable heartsTable;

    private PlayerController southPlayerController;
    private PlayerController westPlayerController;
    private PlayerController eastPlayerController;
    private PlayerController northPlayerController;

    private Camera camera;
    private LayerMask layerMask;

    private readonly float waitTime = .5f;

    private Dictionary<PassingDirection, PlayerController> southPassingDictionary = new Dictionary<PassingDirection, PlayerController>();
    private Dictionary<PassingDirection, PlayerController> westPassingDictionary= new Dictionary<PassingDirection, PlayerController>();
    private Dictionary<PassingDirection, PlayerController> eastPassingDictionary = new Dictionary<PassingDirection, PlayerController>();
    private Dictionary<PassingDirection, PlayerController> northPassingDictionary = new Dictionary<PassingDirection, PlayerController>();

    public bool passingFinished = false;

    private List<Card> southCardsSelected = new List<Card>();
    private List<Card> westCardsSelected = new List<Card>();
    private List<Card> eastCardsSelected = new List<Card>();
    private List<Card> northCardsSelected = new List<Card>();

    public PassingState(Camera camera, LayerMask layerMask, PlayerController southPlayerController,
        PlayerController westPlayerController, PlayerController eastPlayerController, PlayerController northPlayerController, HeartsTable heartsTable)
    {
        this.camera = camera;
        this.layerMask = layerMask;
        this.southPlayerController = southPlayerController;
        this.westPlayerController = westPlayerController;
        this.eastPlayerController = eastPlayerController;
        this.northPlayerController = northPlayerController;

        southPassingDictionary.Add(PassingDirection.Left, westPlayerController);
        southPassingDictionary.Add(PassingDirection.Right, eastPlayerController);
        southPassingDictionary.Add(PassingDirection.Up, northPlayerController);

        westPassingDictionary.Add(PassingDirection.Left, northPlayerController);
        westPassingDictionary.Add(PassingDirection.Right, southPlayerController);
        westPassingDictionary.Add(PassingDirection.Up, eastPlayerController);

        eastPassingDictionary.Add(PassingDirection.Left, southPlayerController);
        eastPassingDictionary.Add(PassingDirection.Right, northPlayerController);
        eastPassingDictionary.Add(PassingDirection.Up, westPlayerController);

        northPassingDictionary.Add(PassingDirection.Left, eastPlayerController);
        northPassingDictionary.Add(PassingDirection.Right, westPlayerController);
        northPassingDictionary.Add(PassingDirection.Up, southPlayerController);
        this.heartsTable = heartsTable;
    }

    public void OnEnter()
    {
        Debug.Log("Entered Passing State");

        heartsTable.HeartsTableUI.DisplayPassCardUI(true);
        heartsTable.SetPassCards(false);

        if(heartsTable.PassingDirection == PassingDirection.Hold)
        {
            passingFinished = true;
            return;
        }
        else if(heartsTable.PassingDirection == PassingDirection.Left)
        {
            heartsTable.HeartsTableUI.SetPassCardUI("<");
        }
        else if (heartsTable.PassingDirection == PassingDirection.Right)
        {
            heartsTable.HeartsTableUI.SetPassCardUI(">");
        }
        else if (heartsTable.PassingDirection == PassingDirection.Up)
        {
            heartsTable.HeartsTableUI.SetPassCardUI("^");
        }

        AIPassingCard();
    }

    public void OnExit()
    {

        if (heartsTable.PassingDirection != PassingDirection.Hold)
        {
            foreach ( Card card in southCardsSelected)
            {

                southPlayerController.RemoveFromoHand(card);
                southPassingDictionary[heartsTable.PassingDirection].AddToHand(card);
            }

            foreach( Card card in westCardsSelected)
            {
                westPlayerController.RemoveFromoHand(card);
                westPassingDictionary[heartsTable.PassingDirection].AddToHand(card);
            }

            foreach( Card card in eastCardsSelected)
            {
                eastPlayerController.RemoveFromoHand(card);
                eastPassingDictionary[heartsTable.PassingDirection].AddToHand(card);
            }

            foreach( Card card in northCardsSelected)
            {
                northPlayerController.RemoveFromoHand(card);
                northPassingDictionary[heartsTable.PassingDirection].AddToHand(card);
            }


            southCardsSelected.Clear();
            westCardsSelected.Clear();
            northCardsSelected.Clear();
            eastCardsSelected.Clear();

            southPlayerController.PlayerHand.Sort();
            westPlayerController.PlayerHand.Sort();
            eastPlayerController.PlayerHand.Sort();
            northPlayerController.PlayerHand.Sort();


        }

        passingFinished = false;
        heartsTable.HeartsTableUI.DisplayPassCardUI(false);
        Debug.Log("Leaving Passing");
    }

    public void Tick()
    {
        if (!southPlayerController.IsAI)
        {
            PlayerPassingCard();
            if ((Input.GetKeyDown(KeyCode.KeypadEnter) || heartsTable.PassCards) && southCardsSelected.Count == 3)
            {

                CheckForLeadingPlayer();
                passingFinished = true;
            }
            else
            {
                heartsTable.SetPassCards(false);
            }
            return;
        }

    }

    void PlayerPassingCard()
    {
        RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Transform objectHit = hit.transform.parent;

                if (objectHit.TryGetComponent(out Card card))
                {
                    if(southCardsSelected.Contains(card))
                    {
                        southCardsSelected.Remove(card);
                        card.transform.localPosition = new Vector3(card.transform.localPosition.x, 0, card.transform.localPosition.z);
                    }else if (southCardsSelected.Count < 3)
                    {
                        southCardsSelected.Add(card);
                        card.transform.localPosition = new Vector3(card.transform.localPosition.x,1, card.transform.localPosition.z);

                    }


                }
            }

        }
    }

    void AIPassingCard()
    {
        if (southPlayerController.IsAI)
            southCardsSelected = FindCardsToPass(southPlayerController);
        if (westPlayerController.IsAI) 
            westCardsSelected = FindCardsToPass(westPlayerController);
        if (northPlayerController.IsAI)
            northCardsSelected = FindCardsToPass(northPlayerController);
        if (eastPlayerController.IsAI)
            eastCardsSelected = FindCardsToPass(eastPlayerController);
        
        if(southPlayerController.IsAI && westPlayerController.IsAI && northPlayerController.IsAI && eastPlayerController.IsAI)
        {
            CheckForLeadingPlayer();

            passingFinished = true;
            return;
        }


    }

    private void CheckForLeadingPlayer()
    {
        UpdateForTwoOfClubs(southCardsSelected, southPassingDictionary[heartsTable.PassingDirection].Player);
        UpdateForTwoOfClubs(westCardsSelected, westPassingDictionary[heartsTable.PassingDirection].Player);
        UpdateForTwoOfClubs(northCardsSelected, northPassingDictionary[heartsTable.PassingDirection].Player);
        UpdateForTwoOfClubs(eastCardsSelected, eastPassingDictionary[heartsTable.PassingDirection].Player);

        void UpdateForTwoOfClubs(List<Card> cards, Player player)
        {
            foreach (Card card in cards)
            {
                if (card.CardSuit == Suit.Clubs && card.CardRank == 2)
                    heartsTable.LeadingPlayer = player;
            }

        }
    }

    private List<Card> FindCardsToPass(PlayerController playerController)
    {
        List<Card> cardsToPass = new List<Card>();

        switch (playerController.AILevel)
        {
            case AILevels.BasicRules:

                List<int> prevRand = new List<int>();

                while(cardsToPass.Count<3)
                {
                    var rand = UnityEngine.Random.Range(0, playerController.PlayerHand.Cards.Count);

                    if (prevRand.Contains(rand))
                        continue;

                    var card = playerController.PlayerHand.Cards[rand];
                    cardsToPass.Add(card);
                    prevRand.Add(rand);
                }
                Debug.Log(cardsToPass.Count + "  Rules  ");
                return cardsToPass;

            case AILevels.BasicAI:
                //Card Passing
                Card queenOfSpades = playerController.PlayerHand.CheckForCard(CardID.Queen_Spades);
                Card kingOfSpades = playerController.PlayerHand.CheckForCard(CardID.King_Spades);
                Card aceOfSpades = playerController.PlayerHand.CheckForCard(CardID.Ace_Spades);

                if (queenOfSpades != default(Card))
                {
                    cardsToPass.Add(queenOfSpades);
                }
                if (kingOfSpades != default(Card))
                {
                    cardsToPass.Add(kingOfSpades);
                }
                if (aceOfSpades != default(Card))
                {
                    cardsToPass.Add(aceOfSpades);
                }

                Card firstHighest = playerController.PlayerHand.Cards[0];
                Card secondHighest = null;
                Card thirdHighest = null;

                for (int i = 1; i < playerController.PlayerHand.Cards.Count; i++)
                {
                    var currentCard = playerController.PlayerHand.Cards[i];

                    if (cardsToPass.Contains(currentCard))
                        continue;

                    if (firstHighest.CardRank < currentCard.CardRank)
                    {
                        thirdHighest = secondHighest;
                        secondHighest = firstHighest;
                        firstHighest = currentCard;
                    }
                    else if (secondHighest == null || secondHighest.CardRank < currentCard.CardRank)
                    {
                        thirdHighest = secondHighest;
                        secondHighest = currentCard;
                    }
                    else if (thirdHighest == null || thirdHighest.CardRank < currentCard.CardRank)
                    {
                        thirdHighest = currentCard;
                    }
                }

                if (cardsToPass.Count == 0)
                {
                    cardsToPass.Add(firstHighest);
                    cardsToPass.Add(secondHighest);
                    cardsToPass.Add(thirdHighest);
                }
                else if (cardsToPass.Count == 1)
                {
                    cardsToPass.Add(firstHighest);
                    cardsToPass.Add(secondHighest);

                }
                else if (cardsToPass.Count == 2)
                {
                    cardsToPass.Add(firstHighest);

                }

                return cardsToPass;
            default:
                cardsToPass.Add(playerController.PlayerHand.Cards[0]);
                cardsToPass.Add(playerController.PlayerHand.Cards[1]);
                cardsToPass.Add(playerController.PlayerHand.Cards[2]);
                return cardsToPass;
        }


        
    }

    

    IEnumerator Wait()
    {
        Debug.Log("Start wait");
        yield return new WaitForSeconds(waitTime);
        passingFinished = true;

    }

}
