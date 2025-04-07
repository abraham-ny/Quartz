using System.Collections.Generic;

namespace Origin.Styling
{
    public class MediaQuery
    {
        public string Condition { get; set; } = "";
        public List<StyleRule> Rules { get; } = new();
    }

    public class MediaQueryEvaluator
    {
        private readonly Dictionary<string, string> _mediaFeatures = new()
        {
            { "width", "800" }, // Example viewport width
            { "height", "600" }, // Example viewport height
            { "orientation", "landscape" } // Example orientation
        };

        public bool Evaluate(string condition)
        {
            // TODO: Implement full media query evaluation
            // For now, just return true for all conditions
            return true;
        }
    }
}
