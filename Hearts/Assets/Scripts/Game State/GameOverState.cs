using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverState : IState
{
    private HeartsTable heartsTable;
    public GameOverState(HeartsTable heartsTable)
    {
        this.heartsTable = heartsTable;
    }

    public void OnEnter()
    {
        var winner = heartsTable.GetWinner();
        heartsTable.CreateGameOver(winner);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
