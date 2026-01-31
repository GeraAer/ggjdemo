using System.Collections.Generic;

/// <summary>
/// Player data class
/// </summary>
public class Player
{
    public int id;
    public string name;
    public int masks;
    public List<string> hand = new List<string>(); // Simple string list for cards

    public Player(int id, string name)
    {
        this.id = id;
        this.name = name;
        this.masks = 5;
    }

    public void AddCard(string card)
    {
        if (!string.IsNullOrEmpty(card)) 
            hand.Add(card);
    }

    public bool RemoveCard(string card)
    {
        return hand.Remove(card);
    }

    public void AddMask() 
    { 
        masks++; 
    }

    public void RemoveMask() 
    { 
        if (masks > 0) 
            masks--; 
    }

    public int GetHandCount()
    {
        return hand.Count;
    }
}

