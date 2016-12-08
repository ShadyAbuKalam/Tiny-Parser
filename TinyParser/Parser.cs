using System;
using System.Windows;

namespace TinyParser
{
    public class Parser
    {
        private readonly Scanner _scanner;
        private Token _currentToken;

        public Parser(string input)
        {
            _scanner = new Scanner(input);
            _currentToken = _scanner.GetNextToken();
        }

        private void AdvanceInput()
        {
            _currentToken = _scanner.GetNextToken();
        }

        private static void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Match(TokenType expectedTokenType)
        {
            if (_currentToken != null && _currentToken.Type == expectedTokenType)
            {
                AdvanceInput();
            }
            else
            {
                throw new Exception();
            }
        }

        public SyntaxNode Parse()
        {
            return _currentToken == null ? null : stmt_sequence();
        }

        private SyntaxNode stmt_sequence()
        {
            if (_currentToken == null)
            {
                return null;
            }
            var node = Statement();
            var tmp = node;
            while (_currentToken != null && _currentToken.Type == TokenType.SEMI)
            {
                try
                {
                    Match(TokenType.SEMI);
                    tmp.Sibling = Statement();
                    tmp = tmp.Sibling;
                }
                catch (Exception e)
                {
                    Error("Something Went Wrong: " + e);
                }
            }
            return node;
        }

        private SyntaxNode Statement()
        {
            if (_currentToken == null)
            {
                return null;
            }
            SyntaxNode node;
            switch (_currentToken.Type)
            {
                case TokenType.IF:
                    node = if_stmt();
                    break;
                case TokenType.REPEAT:
                    node = repeat_stmt();
                    break;
                case TokenType.READ:
                    node = read_stmt();
                    break;
                case TokenType.WRITE:
                    node = write_stmt();
                    break;
                case TokenType.IDENTIFIER:
                    node = assign_stmt();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return node;
        }

        private SyntaxNode if_stmt()
        {
            if (_currentToken == null)
            {
                return null;
            }
            SyntaxNode node = null;
            try
            {
                Match(TokenType.IF);
                node = new SyntaxNode
                {
                    Label = "if",
                    Shape = NodeShape.Square
                };
                node.ChildNodes.Add(Exp());
                Match(TokenType.THEN);
                node.ChildNodes.Add(stmt_sequence());
                if (_currentToken != null && _currentToken.Type == TokenType.ELSE)
                {
                    Match(TokenType.ELSE);
                    node.ChildNodes.Add(stmt_sequence());
                }
                Match(TokenType.END);
            }
            catch (Exception e)
            {
                Error("Something Went Wrong: " + e);
            }
            return node;
        }

        private SyntaxNode repeat_stmt()
        {
            if (_currentToken == null)
            {
                return null;
            }
            SyntaxNode node = null;
            try
            {
                Match(TokenType.REPEAT);
                node = new SyntaxNode
                {
                    Label = "repeat",
                    Shape = NodeShape.Square
                };
                node.ChildNodes.Add(stmt_sequence());
                Match(TokenType.UNTIL);
                node.ChildNodes.Add(Exp());
            }
            catch (Exception e)
            {
                Error("Something Went Wrong: " + e);
            }
            return node;
        }

        private SyntaxNode assign_stmt()
        {
            if (_currentToken == null)
            {
                return null;
            }
            SyntaxNode node = null;
            try
            {
                string tokenValue = _currentToken.Value;
                Match(TokenType.IDENTIFIER);
                node = new SyntaxNode
                {
                    Label = "assign (" + tokenValue + ")",
                    Shape = NodeShape.Square
                };
                Match(TokenType.ASSIGN);
                node.ChildNodes.Add(Exp());
            }
            catch (Exception e)
            {
                Error("Something Went Wrong: " + e);
            }
            return node;
        }

        private SyntaxNode read_stmt()
        {
            if (_currentToken == null)
            {
                return null;
            }
            SyntaxNode node = null;
            try
            {
                Match(TokenType.READ);
                string tokenValue = _currentToken.Value;
                Match(TokenType.IDENTIFIER);
                node = new SyntaxNode
                {
                    Label = "read (" + tokenValue + ")",
                    Shape = NodeShape.Square
                };
            }
            catch (Exception e)
            {
                Error("Something Went Wrong: " + e);
            }
            return node;
        }

        private SyntaxNode write_stmt()
        {
            if (_currentToken == null)
            {
                return null;
            }
            SyntaxNode node = null;
            try
            {
                Match(TokenType.WRITE);
                node = new SyntaxNode
                {
                    Label = "write",
                    Shape = NodeShape.Square
                };
                node.ChildNodes.Add(Exp());
                
            }
            catch (Exception e)
            {
                Error("Something Went Wrong: " + e);
            }
            return node;
        }

        private SyntaxNode Exp()
        {
            if (_currentToken == null)
            {
                return null;
            }
            var node = simple_exp();
            try
            {
                if (_currentToken != null && _currentToken.Type == TokenType.LT)
                {
                    Match(TokenType.LT);
                    var newNode = new SyntaxNode
                    {
                        Label = "op (<)",
                        Shape = NodeShape.Round
                    };
                    newNode.ChildNodes.Add(node);
                    newNode.ChildNodes.Add(simple_exp());
                    node = newNode;
                }
                else if (_currentToken != null && _currentToken.Type == TokenType.EQ)
                {
                    Match(TokenType.EQ);
                    var newNode = new SyntaxNode
                    {
                        Label = "op (=)",
                        Shape = NodeShape.Round
                    };
                    newNode.ChildNodes.Add(node);
                    newNode.ChildNodes.Add(simple_exp());
                    node = newNode;
                }
            }
            catch (Exception e)
            {
                Error("Something Went Wrong: " + e);
            }
            return node;
        }

        private SyntaxNode simple_exp()
        {
            if (_currentToken == null)
            {
                return null;
            }
            var node = Term();
            while (_currentToken != null && (_currentToken.Type == TokenType.PLUS 
                || _currentToken.Type == TokenType.MINUS))
            {
                try
                {
                    if (_currentToken.Type == TokenType.PLUS)
                    {
                        Match(TokenType.PLUS);
                        var newNode = new SyntaxNode
                        {
                            Label = "op (+)",
                            Shape = NodeShape.Round
                        };
                        newNode.ChildNodes.Add(node);
                        newNode.ChildNodes.Add(Term());
                        node = newNode;
                    }
                    else if (_currentToken.Type == TokenType.MINUS)
                    {
                        Match(TokenType.MINUS);
                        var newNode = new SyntaxNode
                        {
                            Label = "op (-)",
                            Shape = NodeShape.Round
                        };
                        newNode.ChildNodes.Add(node);
                        newNode.ChildNodes.Add(Term());
                        node = newNode;
                    }
                }
                catch (Exception e)
                {
                    Error("Something Went Wrong: " + e);
                }
            }
            return node;
        }

        private SyntaxNode Term()
        {
            if (_currentToken == null)
            {
                return null;
            }
            var node = Factor();
            while (_currentToken != null && (_currentToken.Type == TokenType.MLT
                || _currentToken.Type == TokenType.DIV))
            {
                try
                {
                    if (_currentToken.Type == TokenType.MLT)
                    {
                        Match(TokenType.MLT);
                        var newNode = new SyntaxNode
                        {
                            Label = "op (*)",
                            Shape = NodeShape.Round
                        };
                        newNode.ChildNodes.Add(node);
                        newNode.ChildNodes.Add(Factor());
                        node = newNode;
                    }
                    else if (_currentToken.Type == TokenType.DIV)
                    {
                        Match(TokenType.DIV);
                        var newNode = new SyntaxNode
                        {
                            Label = "op (/)",
                            Shape = NodeShape.Round
                        };
                        newNode.ChildNodes.Add(node);
                        newNode.ChildNodes.Add(Factor());
                        node = newNode;
                    }
                }
                catch (Exception e)
                {
                    Error("Something Went Wrong: " + e);
                }
            }
            return node;
        }

        private SyntaxNode Factor()
        {
            if (_currentToken == null)
            {
                return null;
            }
            SyntaxNode node = null;
            try
            {
                if (_currentToken.Type == TokenType.LPAREN)
                {
                    Match(TokenType.LPAREN);
                    node = Exp();
                    Match(TokenType.RPAREN);
                }
                else if (_currentToken.Type == TokenType.NUMBER)
                {
                    var tokenValue = _currentToken.Value;
                    Match(TokenType.NUMBER);
                    node = new SyntaxNode()
                    {
                        Label = "const (" + tokenValue + ")",
                        Shape = NodeShape.Round
                    };
                }
                else if (_currentToken.Type == TokenType.IDENTIFIER)
                {
                    var tokenValue = _currentToken.Value;
                    Match(TokenType.IDENTIFIER);
                    node = new SyntaxNode()
                    {
                        Label = "id (" + tokenValue + ")",
                        Shape = NodeShape.Round
                    };
                }
            }
            catch (Exception e)
            {
                Error("Something Went Wrong: " + e);
            }
            return node;
        }
    }
}
