using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartsTableUI : MonoBehaviour
{
    #region UIFields
    [TabGroup("South Player UI")]
    [SerializeField] 
    private Text southPlayerName;

    [TabGroup("South Player UI")]
    [SerializeField] 
    private Text southPlayerSetScore;

    [TabGroup("South Player UI")]
    [SerializeField] 
    private Text southPlayerTotalScore; 
    
    
    [TabGroup("West Player UI")]
    [SerializeField] 
    private Text westPlayerName;

    [TabGroup("West Player UI")]
    [SerializeField] 
    private Text westPlayerSetScore;

    [TabGroup("West Player UI")]
    [SerializeField] 
    private Text westPlayerTotalScore;
    

    [TabGroup("East Player UI")]
    [SerializeField] 
    private Text eastPlayerName;

    [TabGroup("East Player UI")]
    [SerializeField] 
    private Text eastPlayerSetScore;

    [TabGroup("East Player UI")]
    [SerializeField] 
    private Text eastPlayerTotalScore;
    

    [TabGroup("North Player UI")]
    [SerializeField] 
    private Text northPlayerName;

    [TabGroup("North Player UI")]
    [SerializeField] 
    private Text northPlayerSetScore;

    [TabGroup("North Player UI")]
    [SerializeField] 
    private Text northPlayerTotalScore;


    [TabGroup("Game Over UI")]
    [SerializeField]
    private GameObject gameOverPanel;

    [TabGroup("Game Over UI")]
    [SerializeField]
    private Text winnerName;

    [TabGroup("Game Over UI")]
    [SerializeField]
    private Text winnerScore;

    [TabGroup("Game Over UI")]
    [SerializeField]
    private Button newGame;

    [TabGroup("Game Over UI")]
    [SerializeField]
    private Button quit;


    [TabGroup("Passing UI")]
    [SerializeField]
    private GameObject passingPanel;

    [TabGroup("Passing UI")]
    [SerializeField]
    private Text passingText;

    #endregion

    public void UpdatePlayerSetScore(Player player, int score)
    {
        var scoreString = score.ToString();
        switch (player)
        {
            case Player.South:
                southPlayerSetScore.text = scoreString;
                break;
            case Player.West:
                westPlayerSetScore.text = scoreString;
                break;
            case Player.East:
                eastPlayerSetScore.text = scoreString;
                break;
            case Player.North:
                northPlayerSetScore.text = scoreString;
                break;
            default:
                break;
        }
    }

    public void UpdatePlayerTotalScore(Player player, int score)
    {

        var scoreString = score.ToString();

        switch (player)
        {
            case Player.South:
                southPlayerTotalScore.text = scoreString;
                break;
            case Player.West:
                westPlayerTotalScore.text = scoreString;
                break;
            case Player.East:
                eastPlayerTotalScore.text = scoreString;
                break;
            case Player.North:
                northPlayerTotalScore.text = scoreString;
                break;
            default:
                break;
        }

    }

    public void CreateGameOverUI(string winnerName, int winnerScore)
    {
        this.winnerScore.text = "Player Score: " + winnerScore.ToString();
        this.winnerName.text = winnerName;

        gameOverPanel.SetActive(true);
    }

    public void ResetUI()
    {
        southPlayerSetScore.text = "0";
        westPlayerSetScore.text = "0";
        northPlayerSetScore.text = "0";
        eastPlayerSetScore.text = "0";

        southPlayerTotalScore.text = "0";
        westPlayerTotalScore.text = "0";
        northPlayerTotalScore.text = "0";
        eastPlayerTotalScore.text = "0";

        gameOverPanel.SetActive(false);

    }

    public void DisplayPassCardUI(bool value)
    {
        passingPanel.SetActive(value);
    }

    public void SetPassCardUI(string value)
    {
        passingText.text = value;
    }


}
