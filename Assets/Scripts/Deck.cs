using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<string> cards = new List<string>();

    public Deck()
    {
        InitializeDeck();
        Shuffle();
    }

    private void InitializeDeck()
    {
        // Add normal cards
        string[] themes = { "Animal", "Plant", "Food", "Color" };
        string[][] names = new string[][]
        {
            new string[] { "Tiger", "Lion", "Elephant", "Monkey", "Rabbit" },
            new string[] { "Rose", "Sunflower", "Cherry", "Bamboo", "Pine" },
            new string[] { "Apple", "Banana", "Orange", "Grape", "Strawberry" },
            new string[] { "Red", "Blue", "Green", "Yellow", "Purple" }
        };

        for (int t = 0; t < themes.Length; t++)
        {
            for (int i = 0; i < 10; i++)
            {
                string name = names[t][i % names[t].Length];
                cards.Add($"{name}_{themes[t]}");
            }
        }

        // Add wild cards
        for (int i = 0; i < 20; i++)
        {
            cards.Add("Wild");
        }
    }

    private void Shuffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(i, cards.Count);
            string temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public string DrawCard()
    {
        if (cards.Count == 0) return null;
        string card = cards[0];
        cards.RemoveAt(0);
        return card;
    }

    public int GetCount()
    {
        return cards.Count;
    }
}

