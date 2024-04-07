using System;
using System.Threading.Tasks;

namespace TOTOllyGeek.Awesome.Tests;

[Serializable]
public class Player(int moves = 10)
{
    private int _moveCount = moves;

    public string Nickname { get; init; }
    public PlayerRace Race { get; init; }
    public PlayerClass Class { get; init; }
    public int Level { get; init; }
    
    public void Attack()
    {
        throw new NotImplementedException("The synchronous attack move is too slow for production");
    }

    public Task AttackAsync(int moves = 1)
    {
        if (_moveCount < 1)
            throw new InvalidOperationException("You are out of moves");

        _moveCount -= moves;
        
        return Task.CompletedTask;
    }
}

public enum PlayerRace
{
    Orc,
    Human,
    Elf,
    Gnome
}

public enum PlayerClass
{
    Rogue,
    Paladin,
    Hunter,
    Warrior
}