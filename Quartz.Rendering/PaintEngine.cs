using System;
using Quartz.DOM;
using SkiaSharp;

namespace Quartz.Rendering
{
    public class PaintEngine
    {
        private readonly SKSurface _surface;

        public PaintEngine(SKSurface surface)
        {
            _surface = surface;
        }

        public void Render(Element root)
        {
            using var canvas = _surface.Canvas;
            canvas.Clear(SKColors.White);
            RenderElement(canvas, root);
        }

        private void RenderElement(SKCanvas canvas, Element element)
        {
            // Render background
            if (element.ComputedStyle.TryGetValue("background-color", out var bgColor))
            {
                using var paint = new SKPaint { Color = ParseColor(bgColor) };
                canvas.DrawRect(element.LayoutBounds, paint);
            }

            // Render text content
            if (!string.IsNullOrEmpty(element.TextContent))
            {
                using var paint = new SKPaint 
                { 
                    Color = ParseColor(element.ComputedStyle.GetValueOrDefault("color", "black")),
                    TextSize = float.Parse(element.ComputedStyle.GetValueOrDefault("font-size", "16"))
                };
                canvas.DrawText(element.TextContent, element.LayoutBounds.Left, element.LayoutBounds.Top, paint);
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
