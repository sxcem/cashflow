﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoving : GameState
{

    private int currentSpace;
    private int targetSpace;
    private Player player;
    private Vector3 targetPosition;
    private bool downsizeLoan;
    private int downsizeCost;

    private float moveTime;
    private readonly float moveTimer = 0.4f;

    private Vector3 startPos;

    public PlayerMoving(MainGameManager mgm, int dieCount)
    {
        player = mgm.GetPlayer(mgm.turnManager.GetCurrentPlayer());
        currentSpace = player.space;
        targetSpace = mgm.board.NormalizeSpace(currentSpace + dieCount);
        targetPosition = mgm.board.SpaceCenter(mgm.board.NormalizeSpace(currentSpace + 1));
        targetPosition.y = player.gamePiece.transform.position.y;
        mgm.gameStateDisplay.SetText("Moving " + dieCount + " space" + (dieCount > 1 ? "s" : ""));
        downsizeLoan = false;
        downsizeCost = 0;
        moveTime = 0;
        startPos = player.gamePiece.transform.position;
    }

    public override GameState Update(MainGameManager mgm)
    {
        if(downsizeLoan)
        {
            if(player.ledger.GetCurretBalance() - downsizeCost >= 0)
            {
                player.Downsize(downsizeCost);
                return new PostTurnState(mgm);
            }
            return new LoanState(mgm, this);
        } 
        if (Vector3.SqrMagnitude(player.gamePiece.transform.position - targetPosition) > mgm.board.sqRatRaceSpaceCenterThreshold)
        {
            player.gamePiece.transform.position = Vector3.Lerp(startPos, targetPosition, mgm.playerMovementCurve.Evaluate(moveTime / moveTimer));
            moveTime += Time.deltaTime;
        }
        else
        {
            moveTime = 0;
            startPos = player.gamePiece.transform.position;
            currentSpace = mgm.board.NormalizeSpace(currentSpace + 1);
            if (currentSpace == targetSpace)
            {
                player.gamePiece.transform.position = targetPosition;
                player.space = targetSpace;
                BoardManager.RatRaceSpaceTypes spaceType = mgm.board.GetSpaceType(targetSpace);
                switch(spaceType)
                {
                    case BoardManager.RatRaceSpaceTypes.Downsized:
                        downsizeCost = player.incomeStatement.TotalExpenses;
                        if(player.ledger.GetCurretBalance() - downsizeCost >= 0)
                        {
                            player.Downsize(downsizeCost);
                        } else
                        {
                            downsizeLoan = true;
                            return new LoanState(mgm, this);
                        }
                        break;
                    case BoardManager.RatRaceSpaceTypes.Baby:
                        return new BabyState(mgm);
                    case BoardManager.RatRaceSpaceTypes.Charity:
                        return new CharityOptionState(mgm);
                    case BoardManager.RatRaceSpaceTypes.Doodad:
                        return new DoodadState(mgm);
                    case BoardManager.RatRaceSpaceTypes.Deals:
                        return new ChoosingDealTypeState(mgm);
                    case BoardManager.RatRaceSpaceTypes.Market:
                        return new MarketCardFlipState(mgm);
                }
                return new PostTurnState(mgm);
            }
            targetPosition = mgm.board.SpaceCenter(mgm.board.NormalizeSpace(currentSpace + 1));
            targetPosition.y = player.gamePiece.transform.position.y;
        }
        return this;
    }
}
