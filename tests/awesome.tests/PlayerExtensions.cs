using System.IO;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace TOTOllyGeek.Awesome.Tests;

public static class PlayerExtensions
{
    public static PlayerAssertions Should(this Player instance)
    {
        return new PlayerAssertions(instance); 
    } 
}

public class PlayerAssertions(Player subject) : ObjectAssertions<Player, PlayerAssertions>(subject)
{
    protected override string Identifier => "player";
    
    public AndConstraint<PlayerAssertions> BeOfRace(PlayerRace race, string because = "", params object[] becauseArgs)
    {
        Execute.Assertion
            .BecauseOf(because, becauseArgs)
            .Given(() => Subject.Race)
            .ForCondition(r => r == race)
            .FailWith(
                "Expected {context:player} to be {0}{reason}, but found {1}.",
                _ => race.ToString(), p => p.ToString());

        return new AndConstraint<PlayerAssertions>(this);
    }
}