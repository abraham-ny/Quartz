using System;
using System.Drawing;
using Quartz.DOM;

namespace Quartz.Rendering
{
    public class LayoutEngine
    {
        private readonly FlexLayoutEngine _flexEngine = new();

        public LayoutNode CalculateLayout(Element element, int containingWidth, int containingHeight)
        {
            // Check if this is a flex container
            if (element.ComputedStyle.Display == "flex" || 
                element.ComputedStyle.Display == "inline-flex")
            {
                return _flexEngine.CalculateFlexLayout(element, containingWidth, containingHeight);
            }

            var layoutNode = new LayoutNode { Element = element };
            
            // Calculate box model
            var margin = ParseBoxValue(element.ComputedStyle.Margin);
            var border = ParseBoxValue(element.ComputedStyle.BorderWidth);
            var padding = ParseBoxValue(element.ComputedStyle.Padding);
            
            // Calculate content dimensions
            var width = ParseLength(element.ComputedStyle.Width, containingWidth);
            var height = ParseLength(element.ComputedStyle.Height, containingHeight);
            
            // Calculate final dimensions
            var totalWidth = margin.Left + border.Left + padding.Left + width + 
                           padding.Right + border.Right + margin.Right;
                           
            var totalHeight = margin.Top + border.Top + padding.Top + height + 
                            padding.Bottom + border.Bottom + margin.Bottom;
            
            // Position the element
            layoutNode.Bounds = new RectangleF(
                margin.Left + border.Left + padding.Left,
                margin.Top + border.Top + padding.Top,
                width,
                height);
            
            // Process children
            foreach (var child in element.Children)
            {
                var childNode = CalculateLayout(child, width, height);
                layoutNode.Children.Add(childNode);
            }
            
            return layoutNode;
        }

        private float ParseLength(string value, float containerSize)
        {
            if (string.IsNullOrEmpty(value) || value == "auto") return containerSize;
            if (value.EndsWith("px")) return float.Parse(value[..^2]);
            if (value.EndsWith("%")) return containerSize * float.Parse(value[..^1]) / 100f;
            return float.Parse(value);
        }

        private BoxValues ParseBoxValue(string value)
        {
            var parts = value?.Split(' ') ?? Array.Empty<string>();
            return parts.Length switch
            {
                1 => new BoxValues(float.Parse(parts[0])),
                2 => new BoxValues(float.Parse(parts[0]), float.Parse(parts[1])),
                4 => new BoxValues(
                    float.Parse(parts[0]), 
                    float.Parse(parts[1]), 
                    float.Parse(parts[2]), 
                    float.Parse(parts[3])),
                _ => new BoxValues(0)
            };
        }

        private struct BoxValues
        {
            public float Top;
            public float Right;
            public float Bottom;
            public float Left;

            public BoxValues(float all) : this(all, all, all, all) { }
            public BoxValues(float vertical, float horizontal) : this(vertical, horizontal, vertical, horizontal) { }
            public BoxValues(float top, float right, float bottom, float left)
            {
                Top = top;
                Right = right;
                Bottom = bottom;
                Left = left;
            }
        }
    }
}
