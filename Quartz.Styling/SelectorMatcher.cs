using System.Collections.Generic;
using System.Linq;
using Quartz.DOM;

namespace Quartz.Styling
{
    public class SelectorMatcher
    {
        private static readonly MediaQueryEvaluator _mediaEvaluator = new();

        public static List<StyleRule> MatchRules(Element element, List<Stylesheet> stylesheets)
        {
            var matchedRules = new List<StyleRule>();
            
            foreach (var stylesheet in stylesheets)
            {
                // Add regular rules
                matchedRules.AddRange(GetMatchingRules(element, stylesheet.Rules));
                
                // Add media query rules
                foreach (var mediaQuery in stylesheet.MediaQueries)
                {
                    if (_mediaEvaluator.Evaluate(mediaQuery.Condition))
                    {
                        matchedRules.AddRange(GetMatchingRules(element, mediaQuery.Rules));
                    }
                }
            }
            
            // Sort by specificity
            return matchedRules
                .OrderByDescending(r => CalculateSpecificity(r.Selectors))
                .ToList();
        }

        private static IEnumerable<StyleRule> GetMatchingRules(Element element, List<StyleRule> rules)
        {
            foreach (var rule in rules)
            {
                if (MatchesSelectors(element, rule.Selectors))
                {
                    yield return rule;
                }
            }
        }

        private static bool MatchesSelectors(Element element, List<string> selectors)
        {
            foreach (var selector in selectors)
            {
                if (MatchesSelector(element, selector))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool MatchesSelector(Element element, string selector)
        {
            // TODO: Implement full CSS selector matching
            // For now, just match by tag name
            return selector == element.TagName;
        }

        private static int CalculateSpecificity(List<string> selectors)
        {
            // TODO: Implement full CSS specificity calculation
            // For now, just count the number of selectors
            return selectors.Count;
        }
    }
}
