using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.Styling
{
    internal class CssRuleParser
    {
        private readonly List<CssToken> _tokens;
        private int _position;

        public CssRuleParser(List<CssToken> tokens)
        {
            _tokens = tokens;
        }

        public bool HasMoreRules => _position < _tokens.Count;

        public string Peek()
        {
            return _position < _tokens.Count ? _tokens[_position].Value : null;
        }

        public void Consume(string expected)
        {
            if (_position >= _tokens.Count || _tokens[_position].Value != expected)
            {
                throw new Exception($"Expected '{expected}' but found '{_tokens[_position].Value}'");
            }
            _position++;
        }

        public string ReadUntil(string delimiter)
        {
            var sb = new StringBuilder();
            while (_position < _tokens.Count && _tokens[_position].Value != delimiter)
            {
                sb.Append(_tokens[_position].Value);
                _position++;
            }
            return sb.ToString();
        }

        public StyleRule ParseRule()
        {
            try
            {
                var selectors = ParseSelectors();
                if (selectors.Count == 0) return null;

                var declarations = ParseDeclarations();
                return new StyleRule
                {
                    Selectors = selectors,
                    Declarations = declarations
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string> ParseSelectors()
        {
            var selectors = new List<string>();
            var currentSelector = new List<string>();

            while (_position < _tokens.Count)
            {
                var token = _tokens[_position];
                if (token.Value == "{")
                {
                    _position++;
                    if (currentSelector.Count > 0)
                    {
                        selectors.Add(string.Join(" ", currentSelector));
                    }
                    return selectors;
                }
                else if (token.Value == ",")
                {
                    if (currentSelector.Count > 0)
                    {
                        selectors.Add(string.Join(" ", currentSelector));
                        currentSelector.Clear();
                    }
                    _position++;
                }
                else
                {
                    currentSelector.Add(token.Value);
                    _position++;
                }
            }

            return selectors;
        }

        private Dictionary<string, string> ParseDeclarations()
        {
            var declarations = new Dictionary<string, string>();

            while (_position < _tokens.Count)
            {
                var token = _tokens[_position];
                if (token.Value == "}")
                {
                    _position++;
                    return declarations;
                }

                var property = token.Value;
                _position += 2; // Skip property and colon

                var valueTokens = new List<string>();
                while (_position < _tokens.Count)
                {
                    token = _tokens[_position];
                    if (token.Value == ";" || token.Value == "}")
                    {
                        break;
                    }
                    valueTokens.Add(token.Value);
                    _position++;
                }

                declarations[property] = string.Join(" ", valueTokens);
                
                if (_position < _tokens.Count && _tokens[_position].Value == ";")
                {
                    _position++;
                }
            }

            return declarations;
        }
    }

    public class StyleRule
    {
        public List<string> Selectors { get; set; } = new();
        public Dictionary<string, string> Declarations { get; set; } = new();
    }
}
