using System;
using System.Collections.Generic;
using System.Text;

namespace Origin.Styling
{
    public class CssParser
    {
        public Stylesheet Parse(string cssText)
        {
            var stylesheet = new Stylesheet();
            try 
            {
                var tokenizer = new CssTokenizer(cssText);
                var tokens = tokenizer.Tokenize();
                
                var parser = new CssRuleParser(tokens);
                while (parser.HasMoreRules)
                {
                    if (parser.Peek() == "@media")
                    {
                        var mediaQuery = ParseMediaQuery(parser);
                        if (mediaQuery != null)
                        {
                            stylesheet.MediaQueries.Add(mediaQuery);
                        }
                    }
                    else if (parser.Peek() == "@keyframes")
                    {
                        var animation = ParseKeyframes(parser);
                        if (animation != null)
                        {
                            stylesheet.Animations.Add(animation);
                        }
                    }
                    else if (parser.Peek() == "--")
                    {
                        var variable = ParseCustomProperty(parser);
                        if (variable != null)
                        {
                            stylesheet.Variables.Add(variable);
                        }
                    }
                    else
                    {
                        var rule = parser.ParseRule();
                        if (rule != null)
                        {
                            stylesheet.Rules.Add(rule);
                        }
                    }
                }
            }
            catch (CssParseException ex)
            {
                Console.WriteLine($"CSS parse error: {ex.Message}");
            }
            
            return stylesheet;
        }

        private KeyframeAnimation ParseKeyframes(CssRuleParser parser)
        {
            parser.Consume("@keyframes");
            var name = parser.ReadUntil("{").Trim();
            parser.Consume("{");
            
            var animation = new KeyframeAnimation { Name = name };
            
            while (!parser.Peek().Equals("}"))
            {
                var keyframe = parser.ParseKeyframe();
                if (keyframe != null)
                {
                    animation.Keyframes.Add(keyframe);
                }
            }
            
            parser.Consume("}");
            return animation;
        }

        private CssVariable ParseCustomProperty(CssRuleParser parser)
        {
            parser.Consume("--");
            var name = parser.ReadUntil(":").Trim();
            parser.Consume(":");
            var value = parser.ReadUntil(";").Trim();
            parser.Consume(";");
            
            return new CssVariable(name, value);
        }

        private MediaQuery ParseMediaQuery(CssRuleParser parser)
        {
            parser.Consume("@media");
            var condition = parser.ReadUntil("{");
            parser.Consume("{");
            
            var mediaQuery = new MediaQuery { Condition = condition.Trim() };
            
            while (!parser.Peek().Equals("}"))
            {
                var rule = parser.ParseRule();
                if (rule != null)
                {
                    mediaQuery.Rules.Add(rule);
                }
            }
            
            parser.Consume("}");
            return mediaQuery;
        }
    }

    internal class CssTokenizer
    {
        private readonly string _input;
        private int _position;

        public CssTokenizer(string input)
        {
            _input = input;
        }

        public List<CssToken> Tokenize()
        {
            var tokens = new List<CssToken>();
            while (_position < _input.Length)
            {
                var c = _input[_position];
                switch (c)
                {
                    case '{':
                    case '}':
                    case ';':
                    case ':':
                    case '(':
                    case ')':
                    case '[':
                    case ']':
                        tokens.Add(new CssToken(c.ToString(), CssTokenType.Delimiter));
                        _position++;
                        break;
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        _position++;
                        break;
                    case '/':
                        if (_position + 1 < _input.Length && _input[_position + 1] == '*')
                        {
                            SkipComment();
                        }
                        else
                        {
                            tokens.Add(new CssToken(c.ToString(), CssTokenType.Delimiter));
                            _position++;
                        }
                        break;
                    case '"':
                    case '\'':
                        tokens.Add(ReadString());
                        break;
                    default:
                        if (char.IsLetter(c) || c == '_' || c == '-')
                        {
                            tokens.Add(ReadIdentifier());
                        }
                        else if (char.IsDigit(c) || c == '.' || c == '+' || c == '-')
                        {
                            tokens.Add(ReadNumeric());
                        }
                        else
                        {
                            _position++;
                        }
                        break;
                }
            }
            return tokens;
        }

        private CssToken ReadString()
        {
            var quote = _input[_position];
            var sb = new StringBuilder();
            _position++;
            
            while (_position < _input.Length && _input[_position] != quote)
            {
                sb.Append(_input[_position]);
                _position++;
            }
            
            _position++;
            return new CssToken(sb.ToString(), CssTokenType.String);
        }

        private CssToken ReadIdentifier()
        {
            var sb = new StringBuilder();
            while (_position < _input.Length)
            {
                var c = _input[_position];
                if (char.IsLetterOrDigit(c) || c == '_' || c == '-')
                {
                    sb.Append(c);
                    _position++;
                }
                else
                {
                    break;
                }
            }
            return new CssToken(sb.ToString(), CssTokenType.Identifier);
        }

        private CssToken ReadNumeric()
        {
            var sb = new StringBuilder();
            while (_position < _input.Length)
            {
                var c = _input[_position];
                if (char.IsDigit(c) || c == '.' || c == '+' || c == '-')
                {
                    sb.Append(c);
                    _position++;
                }
                else
                {
                    break;
                }
            }
            return new CssToken(sb.ToString(), CssTokenType.Number);
        }

        private void SkipComment()
        {
            _position += 2;
            while (_position < _input.Length)
            {
                if (_input[_position] == '*' && 
                    _position + 1 < _input.Length && 
                    _input[_position + 1] == '/')
                {
                    _position += 2;
                    return;
                }
                _position++;
            }
        }
    }

    internal class CssToken
    {
        public string Value { get; }
        public CssTokenType Type { get; }

        public CssToken(string value, CssTokenType type)
        {
            Value = value;
            Type = type;
        }
    }

    internal enum CssTokenType
    {
        Identifier,
        Number,
        String,
        Delimiter
    }
}
