using System;
using Quartz.DOM;
using SkiaSharp;

namespace Quartz.Rendering
{
    public class PaintEngine
    {
        private readonly SKSurface _surface;
        private readonly SKPaint _sharedPaint = new SKPaint();
        private readonly SKPaint _textPaint = new SKPaint();
        private Element _lastRenderedRoot;

        public PaintEngine(SKSurface surface)
        {
            _surface = surface;
        }

        public void Render(Element root)
        {
            if (root == _lastRenderedRoot) return;
            
            using var canvas = _surface.Canvas;
            canvas.Clear(SKColors.White);
            
            _sharedPaint.Reset();
            _textPaint.Reset();
            
            RenderElement(canvas, root);
            _lastRenderedRoot = root;
        }

        private void RenderElement(SKCanvas canvas, Element element)
        {
            // Skip invisible elements
            if (element.ComputedStyle.TryGetValue("display", out var display) && display == "none")
                return;

            // Render background
            if (element.ComputedStyle.TryGetValue("background-color", out var bgColor))
            {
                _sharedPaint.Color = ParseColor(bgColor);
                canvas.DrawRect(element.LayoutBounds, _sharedPaint);
            }

            // Render text content
            if (!string.IsNullOrEmpty(element.TextContent))
            {
                _textPaint.Color = ParseColor(element.ComputedStyle.GetValueOrDefault("color", "black"));
                _textPaint.TextSize = float.Parse(element.ComputedStyle.GetValueOrDefault("font-size", "16"));
                canvas.DrawText(element.TextContent, element.LayoutBounds.Left, element.LayoutBounds.Top, _textPaint);
            }

            // Render children
            foreach (var child in element.Children)
            {
                RenderElement(canvas, child);
            }
        }

        private SKColor ParseColor(string color)
        {
            return color switch
            {
                "red" => SKColors.Red,
                "blue" => SKColors.Blue,
                "green" => SKColors.Green,
                "black" => SKColors.Black,
                "white" => SKColors.White,
                _ => SKColors.Black
            };
        }
    }
}
