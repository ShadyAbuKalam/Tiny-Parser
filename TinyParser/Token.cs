// ReSharper disable InconsistentNaming

using System.Linq;

namespace TinyParser
{
    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(string value)
        {
            Value = value;
            Type = ValueToType(Value);
        }

        private static TokenType ValueToType(string value)
        {
            switch (value)
            {
                case "if":
                    return TokenType.IF;
                case "then":
                    return TokenType.THEN;
                case "else":
                    return TokenType.ELSE;
                case "end":
                    return TokenType.END;
                case "repeat":
                    return TokenType.REPEAT;
                case "until":
                    return TokenType.UNTIL;
                case "read":
                    return TokenType.READ;
                case "write":
                    return TokenType.WRITE;
                case "+":
                    return TokenType.PLUS;
                case "-":
                    return TokenType.MINUS;
                case "*":
                    return TokenType.MLT;
                case "/":
                    return TokenType.DIV;
                case "=":
                    return TokenType.EQ;
                case "<":
                    return TokenType.LT;
                case "(":
                    return TokenType.LPAREN;
                case ")":
                    return TokenType.RPAREN;
                case ";":
                    return TokenType.SEMI;
                case ":=":
                    return TokenType.ASSIGN;
                default:
                    if (value.All(char.IsLetter))
                    {
                        return TokenType.IDENTIFIER;
                    }
                    return value.All(char.IsDigit) ? TokenType.NUMBER : TokenType.NONE;
            }
        }
    }

    public enum TokenType
    {
        IF, // if
        THEN, // then
        ELSE, // else
        END, // end
        REPEAT, // repeat
        UNTIL, // until
        READ, // read
        WRITE, // write
        PLUS, // +
        MINUS, // -
        MLT, // *
        DIV, // /
        EQ, // =
        LT, // <
        LPAREN, // (
        RPAREN, // )
        SEMI, // ;
        ASSIGN, // :=
        IDENTIFIER, // 1 or more letters 
        NUMBER, // 1 or more digits
        NONE // error
    }
}
