using System.Collections.Generic;
using Quartz.DOM;

namespace Quartz.Styling
{
    public class StyleContext
    {
        private readonly Document _document;
        private readonly StyleResolver _resolver = new();

        public StyleContext(Document document)
        {
            _document = document;
        }

        public void RecalculateStyles()
        {
            if (_document.RootElement == null) return;
            ApplyStyles(_document.RootElement);
        }

        private void ApplyStyles(Element element)
        {
            element.ComputedStyle = _resolver.Resolve(element, _document.Stylesheets);
            foreach (var child in element.Children)
            {
                ApplyStyles(child);
            }
        }
    }

    public class StyleResolver
    {
        public ComputedStyle Resolve(Element element, List<Stylesheet> stylesheets)
        {
            var computedStyle = new ComputedStyle();
            // TODO: Implement CSS selector matching and cascade
            return computedStyle;
        }
    }

    public class ComputedStyle
    {
        // Layout properties
        public string Display { get; set; } = "block";
        public string Position { get; set; } = "static";
        public string Float { get; set; } = "none";
        public string Clear { get; set; } = "none";
        
        // Box model
        public string Width { get; set; } = "auto";
        public string Height { get; set; } = "auto";
        public string Margin { get; set; } = "0";
        public string Padding { get; set; } = "0";
        public string BorderWidth { get; set; } = "0";
        public string BorderColor { get; set; } = "transparent";
        public string BorderStyle { get; set; } = "none";
        
        // Visual properties
        public string BackgroundColor { get; set; } = "transparent";
        public string Color { get; set; } = "black";
        public string Opacity { get; set; } = "1";
        
        // Text properties
        public string FontFamily { get; set; } = "Arial";
        public string FontSize { get; set; } = "16px";
        public string FontWeight { get; set; } = "normal";
        public string TextAlign { get; set; } = "left";
        
        // Flexbox properties
        public string FlexDirection { get; set; } = "row";
        public string FlexWrap { get; set; } = "nowrap";
        public string JustifyContent { get; set; } = "flex-start";
        public string AlignItems { get; set; } = "stretch";
        
        // Transition/animation
        public string Transition { get; set; } = "none";
        
        // Transform
        public string Transform { get; set; } = "none";
        
        // Z-index
        public string ZIndex { get; set; } = "auto";
    }
}
