using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum AILevels { BasicRules, BasicAI}
public class HeartsTable : MonoBehaviour
{

    public static HeartsTable instance;

    //Private Variables
    private HeartsTableUI heartsTableUI;
    private Deck deck;
    private StateMachine gameStateMachine;

    #region AI Fields
    [TabGroup("AI Handling")]
    [SerializeField]
    private AILevels AILevel;
    #endregion

    #region Game Status Fields
    [TabGroup("Card Handling")]
    [SerializeField, ReadOnly] 
    private List<Card> cardsPlayed;
      
    [TabGroup("Card Handling")]
    [SerializeField, ReadOnly]
    private int setPlayCounter = 0;

    [TabGroup("Card Handling")]
    [SerializeField, ReadOnly]
    private PassingDirection passingDirection = PassingDirection.Left;

    [TabGroup("Card Handling")]
    [SerializeField, ReadOnly]
    private Suit leadingSuit;

    [TabGroup("Card Handling")]
    [SerializeField, ReadOnly]
    private Player leadingPlayer;

    [TabGroup("Card Handling")]
    [SerializeField, ReadOnly]
    private bool heartsBroken;

    [TabGroup("Card Handling")]
    [SerializeField, ReadOnly]
    private bool turnChangeReady;

    [TabGroup("Card Handling")]
    [SerializeField]
    private bool hardDeal;

    [TabGroup("Card Handling")]
    [SerializeField]
    private bool activeAnimations;

    [TabGroup("Card Handling")]
    [SerializeField]
    private bool passCards = false;

    #endregion

    #region Player Controller Fields
    [TabGroup("Player Controllers")]
    [SerializeField] 
    private PlayerController southPlayer;

    [TabGroup("Player Controllers")]
    [SerializeField]
    private PlayerController westPlayer;

    [TabGroup("Player Controllers")]
    [SerializeField]
    private PlayerController eastPlayer;

    [TabGroup("Player Controllers")]
    [SerializeField] 
    private PlayerController northPlayer;

    #endregion

    #region Card Locations Fields
    [TabGroup("Table Handling")]
    [SerializeField]
    private Transform southCardLocation;
    [TabGroup("Table Handling")]
    [SerializeField]
    private Transform westCardLocation;
    [TabGroup("Table Handling")]
    [SerializeField]
    private Transform eastCardLocation;
    [TabGroup("Table Handling")]
    [SerializeField]
    private Transform northCardLocation;

    [TabGroup("Table Handling")]
    [SerializeField]
    private Transform southCardsWonLocation;
    [TabGroup("Table Handling")]
    [SerializeField]
    private Transform westCardsWonLocation;
    [TabGroup("Table Handling")]
    [SerializeField]
    private Transform eastCardsWonLocation;
    [TabGroup("Table Handling")]
    [SerializeField]
    private Transform northCardsWonLocation;
    #endregion

    #region Score Fields

    private Dictionary<Player, int> setScore = new Dictionary<Player, int>
    {
        { Player.South, 0},
        { Player.West, 0},
        { Player.East, 0},
        { Player.North, 0}
    };
    
    private Dictionary<Player, int> totalScore = new Dictionary<Player, int>
    {
        { Player.South, 0},
        { Player.West, 0},
        { Player.East, 0},
        { Player.North, 0}
    };

    [TabGroup("Score Tracking")]
    [SerializeField]
    private bool pointLimitReached;

    #endregion


    private void Awake()
    {
        #region Singleton
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        #endregion

        heartsTableUI = GetComponent<HeartsTableUI>();
        deck = GetComponent<Deck>();
    }

    private void Start()
    {
        gameStateMachine = new StateMachine();

        var southLayer = 1 << (int)Player.South;
        var westLayer = 1 << (int)Player.West;
        var eastLayer = 1 << (int)Player.East;
        var northLayer = 1 << (int)Player.North;


        var dealState = new DealCardsState(deck, southPlayer, westPlayer, eastPlayer, northPlayer, this);
        var finishSetState = new FinishSetState(this);

        var southPlayerTurnState = new PlayerTurnState(Camera.main, southLayer, southCardLocation.position, southPlayer, this);
        var westPlayerTurnState = new PlayerTurnState(Camera.main, westLayer, westCardLocation.position, westPlayer, this);
        var eastPlayerTurnState = new PlayerTurnState(Camera.main, eastLayer, eastCardLocation.position, eastPlayer, this);
        var northPlayerTurnState = new PlayerTurnState(Camera.main, northLayer, northCardLocation.position, northPlayer, this);

        var passingState = new PassingState(Camera.main, southLayer, southPlayer, westPlayer, eastPlayer, northPlayer, this);
        var gameOverState = new GameOverState(this);
        
        //Game Over State
        At(to: finishSetState, from: gameOverState, condition: gameOver());

        At(to: dealState, from: passingState, condition: dealFinished());

        //Deal To Whomever has the 2 of clubs
        At(to: passingState, from: southPlayerTurnState, condition: passingFinishedSouthStart());
        At(to: passingState, from: westPlayerTurnState, condition: passingFinishedWestStart());
        At(to: passingState, from: eastPlayerTurnState, condition: passingFinishedEastStart());
        At(to: passingState, from: northPlayerTurnState, condition: passingFinishedNorthStart());

        //Switch Player turns
        At(to: eastPlayerTurnState, from: southPlayerTurnState, condition: isSouthPlayerTurn());                               
        At(to: southPlayerTurnState, from: westPlayerTurnState, condition: isWestPlayersTurn());
        At(to: westPlayerTurnState, from: northPlayerTurnState, condition: isNorthPlayersTurn());
        At(to: northPlayerTurnState, from: eastPlayerTurnState, condition: isEastPlayersTurn());

        //Re-Deal
        At(to: finishSetState, from: dealState, condition: reDealState());

        //Finish current set
        At(to: finishSetState, from: southPlayerTurnState, condition: southStartSet());
        At(to: finishSetState, from: westPlayerTurnState, condition: westStartSet());
        At(to: finishSetState, from: eastPlayerTurnState, condition: eastStartSet());
        At(to: finishSetState, from: northPlayerTurnState, condition: northStartSet());





        gameStateMachine.SetState(dealState);

        gameStateMachine.AddAnyTransition(finishSetState, predicate: () => cardsPlayed.Count > 3 && turnChangeReady);
        gameStateMachine.AddAnyTransition(dealState, predicate: () => HardDeal);

        void At(IState to, IState from, Func<bool> condition) => gameStateMachine.AddTransition(from: to, to: from, condition);


        Func<bool> dealFinished() => () => dealState.areCardsDealt; 
        Func<bool> gameOver() => () => pointLimitReached;

        Func<bool> passingFinishedSouthStart() => () => passingState.passingFinished && leadingPlayer == Player.South;
        Func<bool> passingFinishedWestStart() => () => passingState.passingFinished && leadingPlayer == Player.West;
        Func<bool> passingFinishedEastStart() => () => passingState.passingFinished && leadingPlayer == Player.East;
        Func<bool> passingFinishedNorthStart() => () => passingState.passingFinished && leadingPlayer == Player.North;



        Func<bool> reDealState() => () => finishSetState.NextHand && setPlayCounter == 13;
        Func<bool> southStartSet() => () => finishSetState.NextHand && leadingPlayer == Player.South;
        Func<bool> westStartSet() => () => finishSetState.NextHand && leadingPlayer == Player.West;
        Func<bool> eastStartSet() => () => finishSetState.NextHand && leadingPlayer == Player.East;
        Func<bool> northStartSet() => () => finishSetState.NextHand && leadingPlayer == Player.North;

        Func<bool> isSouthPlayerTurn() => () => eastPlayerTurnState.nextPlayersTurn;
        Func<bool> isWestPlayersTurn() => () => southPlayerTurnState.nextPlayersTurn;
        Func<bool> isEastPlayersTurn() => () => northPlayerTurnState.nextPlayersTurn;
        Func<bool> isNorthPlayersTurn() => () => westPlayerTurnState.nextPlayersTurn;
    }

    private void Update()
    {
        gameStateMachine.Tick();    
    }

    #region Game Handling
    public void AddCardToTable(Card card)
    {
        if (cardsPlayed.Count == 0)
           leadingSuit = card.CardSuit;

        cardsPlayed.Add(card);
        card.SetCardSortingLayer(cardsPlayed.Count);
        card.CardUI.UpdateCardSprite(CardVisibility.Visible);
    }

    public void PrepareNextHand(Player newLeadingPlayer)
    {
        for(int i = 0; i < cardsPlayed.Count; i++)
        {
            deck.AddToDiscard(cardsPlayed[i]);
            cardsPlayed[i].gameObject.SetActive(false);
        }

        cardsPlayed.Clear();
        leadingPlayer = newLeadingPlayer;
    }

    public Player FinishSet()
    {
        var winningCard = cardsPlayed[0];
        var setPoints = 0;

        foreach (Card card in cardsPlayed)
        {
            if (card.CardSuit == Suit.Hearts)
                setPoints++;
            else if (card.CardSuit == Suit.Spades && card.CardRank == 12)
                setPoints += 13;

            if (card.CardRank > winningCard.CardRank && card.CardSuit == winningCard.CardSuit)
            {
                winningCard = card;
            }
        }

        setScore[winningCard.CurrentOwner] += setPoints; 
        heartsTableUI.UpdatePlayerSetScore(winningCard.CurrentOwner, setScore[winningCard.CurrentOwner]);
        setPlayCounter++;
        
        if(setPlayCounter == 13)
        {
            FinishRound();
        }

        return winningCard.CurrentOwner;
    }

    private void FinishRound()
    {
        if(PassingDirection == PassingDirection.Hold)
            passingDirection = PassingDirection.Left; 
        else if (PassingDirection == PassingDirection.Left)
            passingDirection = PassingDirection.Right;
        else if (PassingDirection == PassingDirection.Right)
            passingDirection = PassingDirection.Up;
        else if (PassingDirection == PassingDirection.Up)
            passingDirection = PassingDirection.Hold;

        if(setScore[Player.South] == 26)
        {
            totalScore[Player.West] += 26;
            totalScore[Player.North] += 26;
            totalScore[Player.East] += 26;
            Debug.LogWarning("South Shot the Moon");
        }
        else if(setScore[Player.West] == 26)
        {
            totalScore[Player.South] += 26;
            totalScore[Player.North] += 26;
            totalScore[Player.East] += 26;
            Debug.LogWarning("West Shot the Moon");
        }
        else if (setScore[Player.North] == 26)
        {
            totalScore[Player.South] += 26;
            totalScore[Player.West] += 26;
            totalScore[Player.East] += 26;
            Debug.LogWarning("North Shot the Moon");
        }
        else if (setScore[Player.East] == 26)
        {
            totalScore[Player.South] += 26;
            totalScore[Player.North] += 26;
            totalScore[Player.West] += 26;
            Debug.LogWarning("East Shot the Moon");
        }
        else
        {
            totalScore[Player.South] += setScore[Player.South];
            totalScore[Player.West] += setScore[Player.West];
            totalScore[Player.North] += setScore[Player.North];
            totalScore[Player.East] += setScore[Player.East];
        }

        setScore[Player.South] = 0;
        setScore[Player.West] = 0;
        setScore[Player.North] = 0;
        setScore[Player.East] = 0;

        heartsTableUI.UpdatePlayerTotalScore(Player.South, totalScore[Player.South]);
        heartsTableUI.UpdatePlayerTotalScore(Player.West, totalScore[Player.West]);
        heartsTableUI.UpdatePlayerTotalScore(Player.North, totalScore[Player.North]);
        heartsTableUI.UpdatePlayerTotalScore(Player.East, totalScore[Player.East]);

        heartsTableUI.UpdatePlayerSetScore(Player.South, setScore[Player.South]);
        heartsTableUI.UpdatePlayerSetScore(Player.West, setScore[Player.West]);
        heartsTableUI.UpdatePlayerSetScore(Player.North, setScore[Player.North]);
        heartsTableUI.UpdatePlayerSetScore(Player.East, setScore[Player.East]);

        CheckIfAnyoneIsOver100();
    }

    public bool CheckIfPlayIsAllowed(Card card,PlayerController playerController)
    {
        if (CheckIfLeadingHandAndDidntPlayAClubs(card))
        {
            Debug.Log("Must play the two of clubs");
            return false;
        }

        if (cardsPlayed.Count > 0)
        {
            if (LeadingSuit != card.CardSuit && playerController.PlayerHand.SuitCount[LeadingSuit] > 0)
            {
                Debug.Log("Must play a : " + LeadingSuit);
                return false;
            }
            else if (card.CardSuit == Suit.Hearts)
            {
                if (setPlayCounter > 0)
                {
                    heartsBroken = true;
                    return true;
                    //All Good
                }
                else
                {
                    Debug.Log("Hearts Cannot Be Played");
                    return false;
                }
            }

        }
        else if (card.CardSuit == Suit.Hearts)
        {
            if (setPlayCounter > 0 && heartsBroken || (playerController.PlayerHand.SuitCount[Suit.Clubs] == 0 && 
                playerController.PlayerHand.SuitCount[Suit.Diamonds] == 0 && playerController.PlayerHand.SuitCount[Suit.Spades] == 0))
            {
                heartsBroken = true;
                //All Good
            }
            else
            {
                Debug.Log("Hearts Cannot Be Played");
                return false;
            }
        }
        else if (card.CardSuit == Suit.Spades && card.CardRank == 12)
        {
            if(setPlayCounter == 0)
            {
                Debug.Log("Cannot play Queen of Spades on Round 1");
                return false;
            }
        }
        return true;

        bool CheckIfLeadingHandAndDidntPlayAClubs(Card card)
        {
            return ((setPlayCounter == 0 && cardsPlayed.Count == 0) && (card.CardSuit != Suit.Clubs || card.CardRank != 2));
        }

    }

    private void CheckIfAnyoneIsOver100()
    {
        if (totalScore[Player.South] >= 100)
            pointLimitReached = true;
        else if (totalScore[Player.North] >= 100)
            pointLimitReached = true;
        else if (totalScore[Player.West] >= 100)
            pointLimitReached = true;
        else if (totalScore[Player.East] >= 100)
            pointLimitReached = true;
    }

    public Player GetWinner()
    {
        var winner = Player.South;

        if (totalScore[winner] > totalScore[Player.West])
        {
            winner = Player.West;
        }
        if (totalScore[winner] > totalScore[Player.North])
        {
            winner = Player.North;

        }
        if (totalScore[winner] > totalScore[Player.East])
        {
            winner = Player.East;

        }

        return winner;
    }

    public void SetPassCards(bool value)
    {
        passCards = value;
    }

    #endregion

    #region UI Methods
    public void CreateGameOver(Player winner)
    {
        var score = totalScore[winner];
        heartsTableUI.CreateGameOverUI(winner.ToString(), score);
    }

    #endregion

    #region AI Methods
    private Card AILeadingTurn(PlayerController playerController)
    {
        //RULE: First Lead of Round Must be 2 Clubs
        if(setPlayCounter == 0)
        {
            return playerController.PlayerHand.CheckForCard(CardID.Two_Clubs);
        }

        List<Card> playableCards;
        Card playedCard;

        //RULE: If Hearts are Not Broken, You Cannot* Lead Hearts. If Hearts are broken, you can Lead Hearts
        if (!heartsBroken)
        {
            //*If Hearts are not broken but you only have Hearts, you can Lead hearts
            if (playerController.PlayerHand.SuitCount[Suit.Hearts] == playerController.PlayerHand.Cards.Count)
            {
                playableCards = playerController.PlayerHand.Cards;
            }
            else
            {
                playableCards = playerController.PlayerHand.GetCardsByNotSuit(Suit.Hearts);
            }
        }
        else
        {
            playableCards = playerController.PlayerHand.GetCardsByNotSuit(Suit.None);
        }

        //Choose a Card Based On Different Levels Of AI
        switch (playerController.AILevel)
        {
            case AILevels.BasicRules:
                var rand = UnityEngine.Random.Range(0, playableCards.Count);
                playedCard = playableCards[rand];
                break;
            case AILevels.BasicAI:
                var lowestCard = playableCards[0];

                foreach (Card card in playableCards)
                {
                    if (lowestCard.CardRank > card.CardRank)
                    {
                        lowestCard = card;
                    }
                }
                playedCard = lowestCard;
                break;
            default:
                playedCard = playableCards[0];
                break;
        }

        

        

        return playedCard;


    }

    private Card AIFollowingTurn(PlayerController playerController)
    {
        //Need Cariables
        var suitCount = playerController.PlayerHand.GetSuitCount(leadingSuit);
        var followSuit = true;

        List<Card> playableCards;

        //RULE: If you have a Card(s) matching the Suit of the leading card, you must follow Suit
        if (suitCount > 0)
        {
            playableCards = playerController.PlayerHand.GetCardsBySuit(leadingSuit);
        }
        else
        {
            followSuit = false;

            //RULE: Cannot Break Hearts on Set 1
            if (setPlayCounter == 0)
            {
                playableCards = playerController.PlayerHand.GetCardsForFirstRoundLead();
                

                
            }
            else
            {
                playableCards = playerController.PlayerHand.GetCardsByNotSuit(Suit.None);
            }
        }

        switch (playerController.AILevel)
        {
            case AILevels.BasicRules:
                //This AI Plays Randomly
                var rand = UnityEngine.Random.Range(0, playableCards.Count);
                return(playableCards[rand]);
            case AILevels.BasicAI:
                //This AI Will Try to Avoid Any Trick Possible, if it has to Take a trick, it will dump highest card

                //RULE: Get Current Winning Card of Set
                var currentWinningCard = cardsPlayed[0];
                foreach (Card card in cardsPlayed)
                {
                    if (card.CardSuit == LeadingSuit && card.CardRank > currentWinningCard.CardRank)
                    {
                        currentWinningCard = card;
                    }

                }

                if (followSuit)
                {
                    //Following Suit
                    Card currentCard;

                    //Duck Card If Possible
                    for (int i = playableCards.Count - 1; i >= 0; i--)
                    {
                        currentCard = playableCards[i];

                        if (currentCard.CardRank < currentWinningCard.CardRank)
                        {
                            //Ducking Card
                            return currentCard;
                        }
                    }

                    if (cardsPlayed.Count == 2)
                    {
                        //Dumping Highest Card As you will take trick no matter what
                        Debug.Log("Dump Highest");
                        return playableCards[playableCards.Count - 1];
                    }
                    else
                    {
                        //Dumping lowest Card in hopes of next player winning;
                        return playableCards[0];
                    }
                    
                }
                else
                {

                    var highestCard = playableCards[0];

                    foreach (Card card in playableCards)
                    {
                        //Q-A of Spades are considered Highest Cards for dumping
                        if((card.CardRank == 12 && card.CardSuit == Suit.Spades) || 
                            (card.CardRank == 13 && card.CardSuit == Suit.Spades) || 
                            (card.CardRank == 14 && card.CardSuit == Suit.Spades))
                        {
                            return card;
                        }

                        if (highestCard.CardRank < card.CardRank)
                        {
                            highestCard = card;
                        }
                    }

                    if (!heartsBroken && highestCard.CardSuit == Suit.Hearts)
                    {
                        heartsBroken = true;
                    }

                    return highestCard;

                }

            default:
                return (playableCards[0]);
        }


        
    }

    public Card AIPlayCard(PlayerController playerController)
    {
        Card card;
        if(cardsPlayed.Count == 0)
        {
            card = AILeadingTurn(playerController);
        }
        else
        {
            card = AIFollowingTurn(playerController);
        }
        return card;
    }

    #endregion

    #region Reset Methods
    public void ResetSetPlay()
    {
        setPlayCounter = 0;
        heartsBroken = false;
    }


    public void ResetGame()
    {
        setPlayCounter = 0;
        heartsBroken = false;

        totalScore[Player.South] = 0;
        totalScore[Player.West]  = 0;
        totalScore[Player.North] = 0;
        totalScore[Player.East] = 0;

        setScore[Player.South] = 0;
        setScore[Player.West] = 0;
        setScore[Player.North] = 0;
        setScore[Player.East] = 0;
        pointLimitReached = false;

        passingDirection = PassingDirection.Left;
        hardDeal = true;


        heartsTableUI.ResetUI();


    }

    #endregion

   

    public Player LeadingPlayer { get => leadingPlayer; set => leadingPlayer = value; }
    public bool HardDeal { get => hardDeal; set => hardDeal = value; }
    public bool ActiveAnimations { get => activeAnimations;}
    public bool TurnChangeReady { get => turnChangeReady; set => turnChangeReady = value; }
    public PassingDirection PassingDirection { get => passingDirection; }
    public Suit LeadingSuit { get => leadingSuit; set => leadingSuit = value; }
    public bool PassCards { get => passCards;}
    public List<Card> CardsPlayed { get => cardsPlayed; }
    public HeartsTableUI HeartsTableUI { get => heartsTableUI;}


    public Transform SouthCardsWonLocation { get => southCardsWonLocation;}
    public Transform WestCardsWonLocation { get => westCardsWonLocation;}
    public Transform EastCardsWonLocation { get => eastCardsWonLocation;}
    public Transform NorthCardsWonLocation { get => northCardsWonLocation;}
}
