using System.Collections.Generic;

namespace Origin.DOM
{
    public class Document
    {
        public Element? RootElement { get; set; }
        public List<Stylesheet> Stylesheets { get; } = new();
    }

    public class Element
    {
        public string TagName { get; set; } = "";
        public Dictionary<string, string> Attributes { get; } = new();
        public List<Element> Children { get; } = new();
        public string TextContent { get; set; } = "";
        public ComputedStyle ComputedStyle { get; set; } = new();
    }

    public class Stylesheet
    {
        public List<StyleRule> Rules { get; } = new();
    }
}
