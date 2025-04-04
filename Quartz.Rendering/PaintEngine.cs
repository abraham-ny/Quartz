using System.Drawing;
using Quartz.DOM;

namespace Quartz.Rendering
{
    public class PaintEngine
    {
        public void Paint(LayoutNode node, Graphics graphics)
        {
            // Paint background
            PaintBackground(node, graphics);
            
            // Paint borders
            PaintBorders(node, graphics);
            
            // Paint text content
            PaintText(node, graphics);
            
            // Paint children
            foreach (var child in node.Children)
            {
                Paint(child, graphics);
            }
        }

        private void PaintBackground(LayoutNode node, Graphics graphics)
        {
            if (!string.IsNullOrEmpty(node.Element.ComputedStyle.BackgroundColor))
            {
                var color = ParseColor(node.Element.ComputedStyle.BackgroundColor);
                using var brush = new SolidBrush(color);
                graphics.FillRectangle(brush, node.Bounds);
            }
        }

        private void PaintBorders(LayoutNode node, Graphics graphics)
        {
            if (!string.IsNullOrEmpty(node.Element.ComputedStyle.BorderColor) && 
                !string.IsNullOrEmpty(node.Element.ComputedStyle.BorderWidth))
            {
                var color = ParseColor(node.Element.ComputedStyle.BorderColor);
                var width = float.Parse(node.Element.ComputedStyle.BorderWidth[..^2]); // assumes px
                using var pen = new Pen(color, width);
                graphics.DrawRectangle(pen, 
                    node.Bounds.X, node.Bounds.Y, 
                    node.Bounds.Width, node.Bounds.Height);
            }
        }

        private void PaintText(LayoutNode node, Graphics graphics)
        {
            if (!string.IsNullOrEmpty(node.Element.TextContent) && 
                !string.IsNullOrEmpty(node.Element.ComputedStyle.Color))
            {
                var color = ParseColor(node.Element.ComputedStyle.Color);
                using var brush = new SolidBrush(color);
                
                var fontFamily = string.IsNullOrEmpty(node.Element.ComputedStyle.FontFamily) 
                    ? "Arial" 
                    : node.Element.ComputedStyle.FontFamily;
                
                var fontSize = string.IsNullOrEmpty(node.Element.ComputedStyle.FontSize) 
                    ? 12 
                    : float.Parse(node.Element.ComputedStyle.FontSize[..^2]); // assumes px
                
                using var font = new Font(fontFamily, fontSize);
                graphics.DrawString(node.Element.TextContent, font, brush, node.Bounds);
            }
        }

        private Color ParseColor(string colorValue)
        {
            if (colorValue.StartsWith("#"))
            {
                return ColorTranslator.FromHtml(colorValue);
            }
            else if (colorValue.StartsWith("rgb("))
            {
                var parts = colorValue[4..^1].Split(',');
                return Color.FromArgb(
                    int.Parse(parts[0].Trim()),
                    int.Parse(parts[1].Trim()),
                    int.Parse(parts[2].Trim()));
            }
            else
            {
                return Color.FromName(colorValue);
            }
        }
    }
}
