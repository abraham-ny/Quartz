using System;
using System.Collections.Generic;
using Quartz.Styling;
using Xunit;

namespace Quartz.Tests
{
    public class CssParserTests
    {
        [Fact]
        public void ParsesBasicRule()
        {
            var css = "div { color: red; }";
            var parser = new CssParser();
            var stylesheet = parser.Parse(css);

            Assert.Single(stylesheet.Rules);
            Assert.Equal("div", stylesheet.Rules[0].Selectors[0]);
            Assert.Equal("red", stylesheet.Rules[0].Declarations["color"]);
        }

        [Fact]
        public void ParsesMultipleDeclarations()
        {
            var css = "p { margin: 10px; padding: 5px; }";
            var parser = new CssParser();
            var stylesheet = parser.Parse(css);

            Assert.Single(stylesheet.Rules);
            var rule = stylesheet.Rules[0];
            Assert.Equal("10px", rule.Declarations["margin"]);
            Assert.Equal("5px", rule.Declarations["padding"]);
        }

        [Fact]
        public void ParsesMediaQuery()
        {
            var css = "@media (min-width: 800px) { .container { width: 750px; } }";
            var parser = new CssParser();
            var stylesheet = parser.Parse(css);

            Assert.Empty(stylesheet.Rules);
            Assert.Single(stylesheet.MediaQueries);
            Assert.Equal("(min-width: 800px)", stylesheet.MediaQueries[0].Condition);
            Assert.Equal(".container", stylesheet.MediaQueries[0].Rules[0].Selectors[0]);
            Assert.Equal("750px", stylesheet.MediaQueries[0].Rules[0].Declarations["width"]);
        }

        [Fact]
        public void HandlesMultipleSelectors()
        {
            var css = "h1, h2, h3 { font-weight: bold; }";
            var parser = new CssParser();
            var stylesheet = parser.Parse(css);

            Assert.Single(stylesheet.Rules);
            Assert.Equal(3, stylesheet.Rules[0].Selectors.Count);
            Assert.Equal("h1", stylesheet.Rules[0].Selectors[0]);
            Assert.Equal("h2", stylesheet.Rules[0].Selectors[1]);
            Assert.Equal("h3", stylesheet.Rules[0].Selectors[2]);
        }

        [Fact]
        public void HandlesComplexValues()
        {
            var css = "div { background: url('image.png') no-repeat center; }";
            var parser = new CssParser();
            var stylesheet = parser.Parse(css);

            Assert.Single(stylesheet.Rules);
            Assert.Equal("url('image.png') no-repeat center", 
                stylesheet.Rules[0].Declarations["background"]);
        }
    }
}
