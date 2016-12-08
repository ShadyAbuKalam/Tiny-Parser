using System;
// ReSharper disable InconsistentNaming

namespace TinyParser
{
    public class Scanner
    {
        private State CurrentState { get; set; }
        private string Input { get; }
        private int Position { get; set; }

        public Scanner(string input)
        {
            Input = input;
            Position = 0;
        }

        public Token GetNextToken()
        {
            CurrentState = State.START;
            var tokenValue = "";
            while (CurrentState != State.DONE)
            {
                if (Position == Input.Length)
                {
                    break;
                }
                switch (CurrentState)
                {
                    case State.START:
                        switch (Input[Position])
                        {
                            case ' ':
                                Position++;
                                break;
                            case '\t':
                                Position++;
                                break;
                            case '\r':
                                Position++;
                                break;
                            case '\n':
                                Position++;
                                break;
                            case '{':
                                CurrentState = State.INCOMMENT;
                                Position++;
                                break;
                            case ':':
                                tokenValue += Input[Position];
                                CurrentState = State.INASSIGN;
                                Position++;
                                break;
                            default:
                                if (char.IsLetter(Input[Position]))
                                {
                                    tokenValue += Input[Position];
                                    CurrentState = State.INID;
                                    Position++;
                                }
                                else if (char.IsDigit(Input[Position]))
                                {
                                    tokenValue += Input[Position];
                                    CurrentState = State.INNUM;
                                    Position++;
                                }
                                else
                                {
                                    tokenValue += Input[Position];
                                    CurrentState = State.DONE;
                                    Position++;
                                }
                                break;
                        }
                        break;
                    case State.INNUM:
                        while (char.IsDigit(Input[Position]))
                        {
                            tokenValue += Input[Position];
                            Position++;
                        }
                        CurrentState = State.DONE;
                        break;
                    case State.INID:
                        while (char.IsLetter(Input[Position]))
                        {
                            tokenValue += Input[Position];
                            Position++;
                        }
                        CurrentState = State.DONE;
                        break;
                    case State.INASSIGN:
                        tokenValue += Input[Position];
                        if (tokenValue == ":=")
                        {
                            Position++;
                        }
                        CurrentState = State.DONE;
                        break;
                    case State.INCOMMENT:
                        while (Input[Position] != '}')
                        {
                            Position++;
                        }
                        CurrentState = State.START;
                        Position++;
                        break;
                    case State.DONE:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return tokenValue != "" ? new Token(tokenValue) : null;
        }
    }

    public enum State
    {
        START,
        INNUM,
        INID,
        INASSIGN,
        INCOMMENT,
        DONE
    }
}
