using System.Collections.Generic;

namespace Origin.DOM
{
    public class Element
    {
        public string TagName { get; set; } = "";
        public Element? Parent { get; set; }
        public Dictionary<string, string> Attributes { get; } = new();
        public List<Element> Children { get; } = new();
        public string TextContent { get; set; } = "";
        public ComputedStyle ComputedStyle { get; set; } = new();

        public void AddChild(Element child)
        {
            child.Parent = this;
            Children.Add(child);
        }
    }
}
