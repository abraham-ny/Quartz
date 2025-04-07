using Origin.DOM;
using Origin.Styling;

namespace Origin.Rendering
{
    public class PageRenderer
    {
        private readonly PaintEngine _paintEngine;
        private readonly LayoutEngine _layoutEngine;

        public PageRenderer(PaintEngine paintEngine, LayoutEngine layoutEngine)
        {
            _paintEngine = paintEngine;
            _layoutEngine = layoutEngine;
        }

        public void Render(Document document)
        {
            // Apply styles
            var styleContext = new StyleContext();
            styleContext.ApplyStyles(document);

            // Calculate layout
            _layoutEngine.CalculateLayout(document.RootElement);

            // Paint to output
            _paintEngine.Render(document.RootElement);
        }
    }
}
