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

            var expected = @"     _                                         
    / \__      _____  ___  ___  _ __ ___   ___ 
   / _ \ \ /\ / / _ \/ __|/ _ \| '_ ` _ \ / _ \
  / ___ \ V  V /  __/\__ \ (_) | | | | | |  __/
 /_/   \_\_/\_/ \___||___/\___/|_| |_| |_|\___|
                                               
";
            
            figle.Should().Be(expected);
        }
        
        [TestMethod]
        public void TestTotollygeekOutput()
        {
            var figle = new FigMe("totollygeek").ToString();

            var expected = @"  _        _        _ _                       _    
 | |_ ___ | |_ ___ | | |_   _  __ _  ___  ___| | __
 | __/ _ \| __/ _ \| | | | | |/ _` |/ _ \/ _ \ |/ /
 | || (_) | || (_) | | | |_| | (_| |  __/  __/   < 
  \__\___/ \__\___/|_|_|\__, |\__, |\___|\___|_|\_\
                        |___/ |___/                
";
            
            figle.Should().Be(expected);
        }
    }
}