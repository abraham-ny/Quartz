using System;
using System.Windows.Forms;
using Origin.DOM;
using Origin.Rendering;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Origin.Demo
{
    public class MainForm : Form
    {
        private readonly SKControl _skiaControl;
        private readonly PageRenderer _renderer;
        private Document _document;

        public MainForm()
        {
            Text = "Origin Renderer Demo";
            Width = 800;
            Height = 600;

            _skiaControl = new SKControl
            {
                Dock = DockStyle.Fill
            };
            _skiaControl.PaintSurface += OnPaintSurface;
            Controls.Add(_skiaControl);

            // Initialize rendering pipeline
            var paintEngine = new PaintEngine(_skiaControl.Surface);
            var layoutEngine = new LayoutEngine();
            _renderer = new PageRenderer(paintEngine, layoutEngine);

            // Create sample document
            _document = CreateSampleDocument();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            _renderer.Render(_document);
        }

        private Document CreateSampleDocument()
        {
            var doc = new Document();
            var root = new Element("div")
            {
                ComputedStyle = 
                {
                    ["width"] = "100%",
                    ["height"] = "100%",
                    ["background-color"] = "white"
                }
            };

            var heading = new Element("h1")
            {
                TextContent = "Hello Origin!",
                ComputedStyle =
                {
                    ["color"] = "blue",
                    ["font-size"] = "32px",
                    ["margin"] = "20px"
                }
            };

            var paragraph = new Element("p")
            {
                TextContent = "This is rendered by the Origin engine.",
                ComputedStyle =
                {
                    ["color"] = "black",
                    ["font-size"] = "16px",
                    ["margin"] = "10px 20px"
                }
            };

            root.Children.Add(heading);
            root.Children.Add(paragraph);
            doc.RootElement = root;

            return doc;
        }
    }
}
