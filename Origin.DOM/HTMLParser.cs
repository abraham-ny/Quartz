using System;
using System.Collections.Generic;
using System.Text;

namespace Origin.DOM
{
    public class HTMLParser
    {
        private readonly string _html;
        private int _position;
        
        public HTMLParser(string html)
        {
            _html = html;
            _position = 0;
        }

        public Document Parse()
        {
            var document = new Document();
            var root = new Element { TagName = "html" };
            document.AddElement(root);
            
            while (_position < _html.Length)
            {
                if (_html[_position] == '<')
                {
                    _position++;
                    if (_html[_position] == '/')
                    {
                        // Closing tag
                        _position++;
                        ParseClosingTag();
                    }
                    else
                    {
                        // Opening tag
                        var element = ParseElement(root);
                        root.AddChild(element);
                    }
                }
                else
                {
                    // Text content
                    root.TextContent += ParseText();
                }
            }
            
            return document;
        }

        private Element ParseElement(Element parent)
        {
            var element = new Element
            {
                TagName = ParseTagName(),
                Parent = parent
            };
            
            element.Attributes = ParseAttributes();
            
            if (_html[_position] == '>')
            {
                _position++;
            }
            
            return element;
        }

        private string ParseTagName()
        {
            var start = _position;
            while (_position < _html.Length && char.IsLetterOrDigit(_html[_position]))
            {
                _position++;
            }
            return _html.Substring(start, _position - start).ToLower();
        }

        private Dictionary<string, string> ParseAttributes()
        {
            var attributes = new Dictionary<string, string>();
            
            while (_position < _html.Length && _html[_position] != '>')
            {
                SkipWhitespace();
                
                var attrName = ParseAttributeName();
                var attrValue = "";
                
                if (_html[_position] == '=')
                {
                    _position++;
                    attrValue = ParseAttributeValue();
                }
                
                attributes[attrName] = attrValue;
            }
            
            return attributes;
        }

        private string ParseAttributeName()
        {
            var start = _position;
            while (_position < _html.Length && 
                  (char.IsLetterOrDigit(_html[_position]) || _html[_position] == '-'))
            {
                _position++;
            }
            return _html.Substring(start, _position - start).ToLower();
        }

        private string ParseAttributeValue()
        {
            if (_html[_position] == '"' || _html[_position] == '\'')
            {
                var quote = _html[_position];
                _position++;
                var start = _position;
                
                while (_position < _html.Length && _html[_position] != quote)
                {
                    _position++;
                }
                
                var value = _html.Substring(start, _position - start);
                _position++;
                return value;
            }
            
            return ParseText();
        }

        private string ParseText()
        {
            var start = _position;
            while (_position < _html.Length && _html[_position] != '<')
            {
                _position++;
            }
            return _html.Substring(start, _position - start).Trim();
        }

        private void ParseClosingTag()
        {
            while (_position < _html.Length && _html[_position] != '>')
            {
                _position++;
            }
            _position++;
        }

        private void SkipWhitespace()
        {
            while (_position < _html.Length && char.IsWhiteSpace(_html[_position]))
            {
                _position++;
            }
        }
    }
}
