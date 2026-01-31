using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Deck deck;
    private List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        deck = new Deck();
        players.Clear();

        for (int i = 0; i < 4; i++)
        {
            Player p = new Player(i, $"Player {i + 1}");
            for (int j = 0; j < 5; j++)
            {
                string card = deck.DrawCard();
                if (card != null)
                {
                    p.AddCard(card);
                }
            }
            players.Add(p);
        }

        currentPlayerIndex = 0;
        Debug.Log("Game started!");
    }

    public Player GetCurrentPlayer()
    {
        if (players.Count == 0) return null;
        return players[currentPlayerIndex];
    }

    public List<Player> GetAllPlayers()
    {
        return players;
    }

    public void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
    }

    public Deck GetDeck()
    {
        return deck;
    }
}

