using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TOTOllyGeek.Awesome.Lib;

namespace TOTOllyGeek.Awesome.Tests
{
    [TestClass]
    public class TestFiglets
    {
        [TestMethod]
        public void TestAwesomeOutput()
        {
            var figle = new FigMe("Awesome").ToString();

            var expected = 
                @$"     _                                         {Environment.NewLine}" +
                @$"    / \__      _____  ___  ___  _ __ ___   ___ {Environment.NewLine}" + 
                @$"   / _ \ \ /\ / / _ \/ __|/ _ \| '_ ` _ \ / _ \{Environment.NewLine}" +
                @$"  / ___ \ V  V /  __/\__ \ (_) | | | | | |  __/{Environment.NewLine}" +
                @$" /_/   \_\_/\_/ \___||___/\___/|_| |_| |_|\___|{Environment.NewLine}" +
                @$"                                               {Environment.NewLine}";
            
            figle.Should().NotBeNull().And.NotBeEmpty().And.Be(expected);
        }
        
        [TestMethod]
        public void TestTotollygeekOutput()
        {
            var figle = new FigMe("totollygeek").ToString();

            var expected = 
                @$"  _        _        _ _                       _    {Environment.NewLine}" +
                @$" | |_ ___ | |_ ___ | | |_   _  __ _  ___  ___| | __{Environment.NewLine}" +
                @$" | __/ _ \| __/ _ \| | | | | |/ _` |/ _ \/ _ \ |/ /{Environment.NewLine}" +
                @$" | || (_) | || (_) | | | |_| | (_| |  __/  __/   < {Environment.NewLine}" +
                @$"  \__\___/ \__\___/|_|_|\__, |\__, |\___|\___|_|\_\{Environment.NewLine}" +
                @$"                        |___/ |___/                {Environment.NewLine}";
            
            figle.Should().NotBeNull().And.NotBeEmpty().And.Be(expected);
        }
    }
}