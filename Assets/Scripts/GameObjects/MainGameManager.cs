﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{

    public Camera mainCam;
    public CameraTrack mainCamTracker;
    public PlayerCount playerCount;
    public PlayerNameDreamColor playerNameDreamColor;
    public PlayerProfessionReveal playerProfessionReveal;
    public GameStateDisplay gameStateDisplay;
    public TurnOrder turnOrder;
    public FinancialStatementToggle financialStatementToggle;
    public PayDayAnimation payDayAnimation;
    public PayDayAnimation babyAnimation;
    public PlayerTabContainer playerTabs;
    public Canvas mainUICanvas;
    public BoardManager board;

    public GameObject diePrefab;
    public float diceSpawnDistance = 1f;
    public float diceSpawnOffset = 2f;

    public GameObject gamePiecePrefab;

    public GameObject charityOptionPrefab;
    public GameObject dieAmountSelectPrefab;
    public GameObject doodadDisplayPrefab;
    public GameObject loanDisplayPrefab;
    public GameObject preTurnChoicesPrefab;
    public GameObject postTurnChoicesPrefab;

    private GameState currentState;
    private Player[] players;
    private Stack<Professions.Profession> professions;
    private Stack<DoodadCard> doodadCards;
    private List<DoodadCard> doodadDiscardPile;
    public TurnManager turnManager;
    public int NumPlayers { get { return players != null ? players.Length : 0; } }

    // Start is called before the first frame update
    void Start()
    {
        LoadProfessions();
        LoadDoodads();
        mainCamTracker.origin = board.transform.position + board.ratRaceCenterOffset;
        currentState = new PlayerCountSelectState(this);
    }

    private void LoadProfessions()
    {
        List<Professions.Profession> professionList = new List<Professions.Profession>();
        professionList.AddRange(GameData.Instance.GetProfessions().professions);
        Utility.ShuffleList(professionList);
        professions = new Stack<Professions.Profession>();
        foreach (Professions.Profession profession in professionList)
        {
            professions.Push(profession);
        }
    }

    private void LoadDoodads()
    {
        List<DoodadCard> doodadCardList = new List<DoodadCard>();
        doodadCardList.AddRange(DoodadCard.LoadDoodadCards());
        Utility.ShuffleList(doodadCardList);
        doodadCards = new Stack<DoodadCard>();
        foreach (DoodadCard doodad in doodadCardList)
        {
            doodadCards.Push(doodad);
        }
        doodadDiscardPile = new List<DoodadCard>();
    }

    public void SetNumPlayers(int numPlayers)
    {
        if(players == null)
        {
            players = new Player[numPlayers];
            turnManager = new TurnManager(numPlayers);
        } 
        else
        {
            throw new System.Exception("The player count was already set!");
        }
    }

    public void RegisterPlayer(int index, string name, string dream, Color color)
    {
        if(professions.Count == 0)
        {
            throw new System.Exception("No more professions!");
        }
        players[index] = new Player(index, name, dream, color, professions.Pop());
    }

    public void RegisterPlayerTurnRoll(int player, int rollValue)
    {
        turnManager.RegisterRoll(player, rollValue);
    }

    public void CreatePlayerTabs()
    {
        int[] playerOrder = turnManager.TurnOrder();
        foreach (int pIndex in playerOrder)
        {
            players[pIndex].SetTab(playerTabs.AddTab(players[pIndex]));
        }
    }

    public void SpawnGamePiece(int player)
    {
        GameObject gamePiece = Instantiate(gamePiecePrefab);
        players[player].SetGamePiece(gamePiece.GetComponent<GamePiece>());
    }

    public Player GetPlayer(int i)
    {
        return players[i];
    }

    // Update is called once per frame
    void Update()
    {
        currentState = currentState.Update(this);
    }

    public DiceContainer[] SpawnDice(int n)
    {
        DiceContainer[] dice = new DiceContainer[n];
        Transform camTransform = mainCam.transform;
        float mid = (n - 1) / 2f;
        Vector3 center = camTransform.position + camTransform.forward * diceSpawnDistance;
        for (int i = 0; i < n; i++)
        {
            float distToMid = i - mid;
            Vector3 spawnPoint = center + Vector3.right * distToMid * diceSpawnOffset;
            GameObject diceObject = Instantiate(diePrefab, spawnPoint, camTransform.rotation);
            diceObject.transform.Rotate(Random.onUnitSphere * 360);
            dice[i] = diceObject.GetComponent<DiceContainer>();
        }
        return dice;
    }

    public DoodadCard PullDoodadCard()
    {
        if(doodadCards.Count == 0)
        {
            Utility.ShuffleList(doodadDiscardPile);
            foreach (DoodadCard doodad in doodadDiscardPile)
            {
                doodadCards.Push(doodad);
            }
            doodadDiscardPile.Clear();
        }
        DoodadCard card = doodadCards.Pop();
        doodadDiscardPile.Add(card);
        return card;
    }

}
