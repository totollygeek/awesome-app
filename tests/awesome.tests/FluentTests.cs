using System;
using System.ComponentModel;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TOTOllyGeek.Awesome.Tests;

[TestClass]
public class FluentTests
{
    [TestMethod]
    public void TestPositiveNumbersArray()
    {
        var numbers = new[] { 2, 4, 6, 8, 10 };

        numbers.Should().OnlyContain(n => n > 0);
    }
    
    [TestMethod]
    public void TestEvenNumbersArray()
    {
        var numbers = new[] { 2, 4, 6, 8, 10 };

        numbers.Should().AllSatisfy(n => int.IsEvenInteger(n));
    }
    
    [TestMethod]
    [TestCategory("failing")]
    public void TestWhichIsFailingForArray()
    {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        
        numbers.Should().HaveCount(4, "the numbers should be from 1 to 4");
    }
    
    [TestMethod]
    [TestCategory("failing")]
    public void TestWithoutAScope()
    {
        var numbers = new[] { 2, 4, 6, 8, 10 };
        
        numbers.Should().BeEmpty();
        
        // We will not reach this point
        numbers.Should().BeOfType<string>();
    }

    [TestMethod]
    [TestCategory("failing")]
    public void TestInAScope()
    {
        var numbers = new[] { 2, 4, 6, 8, 10 };
        
        using (new AssertionScope())
        {
            numbers.Should().BeEmpty();
            numbers.Should().BeOfType<string>();
        }
        //After disposal of the AssertionScope, we will get all the assertions inside:
        //
        // Expected numbers to be empty, but found {2, 4, 6, 8, 10}.
        // Expected type to be System.String, but found System.Int32[].
        //
    }
    
    [TestMethod]
    public void TestToCompareTwoSameObjects()
    {
        var player1 = new Player
        {
            Nickname = "totollygeek",
            Class = PlayerClass.Paladin,
            Race = PlayerRace.Human,
            Level = 80
        };

        var player2 = player1;

        player1.Should().BeSameAs(player2);
    }
    
    [TestMethod]
    public void TestToCompareTwoEqualObjects()
    {
        var player1 = new Player
        {
            Nickname = "totollygeek",
            Class = PlayerClass.Paladin,
            Race = PlayerRace.Human,
            Level = 80
        };
        
        var player2 = new Player
        {
            Nickname = "totollygeek",
            Class = PlayerClass.Paladin,
            Race = PlayerRace.Human,
            Level = 80
        };

        player1.Should().BeEquivalentTo(player2);
    }
    
    [TestMethod]
    public void TestViewModelEvents()
    {
        var viewModel = new PlayerViewModel();
        using var monitoredViewModel = viewModel.Monitor();

        viewModel.NickName = "totollygeek";
        
        monitoredViewModel
            .Should().Raise("PropertyChanged")
            .WithSender(viewModel)
            .WithArgs<PropertyChangedEventArgs>(args => args.PropertyName == "NickName");
    }
    
    [TestMethod]
    public void TestExceptionThrowing()
    {
        var player = new Player();
        
        // Directly
        player.Invoking(p => p.Attack())
            .Should().Throw<NotImplementedException>()
            .WithMessage("The synchronous attack move is too slow for production");
    }

    [TestMethod]
    public void TestAsyncThrowing()
    {
        var player = new Player(0);
        
        // Using AAA (Arrange-Act-Assert)
        var act = () => player.AttackAsync();

        act
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("You are out of moves");
    }
    
    [TestMethod]
    public void TestClassStructure()
    {
        typeof(Player)
            .Should()
            .BeDecoratedWith<SerializableAttribute>();
        
        typeof(Player)
            .Methods()
                .ThatAreAsync()
                .Should()
                .NotBeVirtual();
    }
    
    [TestMethod]
    [TestCategory("failing")]
    public void TestCustomAssertionFail()
    {
        var player = new Player
        {
            Nickname = "totollygeek",
            Class = PlayerClass.Paladin,
            Race = PlayerRace.Human,
            Level = 80
        };

        player.Should().BeOfRace(
            PlayerRace.Orc, 
            "only Orc players are allowed here");
    }
}