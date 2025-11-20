using System;
using System.Collections.Generic;
using System.Linq;

namespace LAB4_FOX.Models
{
    public class FormulaParser
    {
        private Dictionary<string, int> _operators = new Dictionary<string, int>
        {
            { "!", 4 }, { "¬", 4 }, { "not", 4 },
            { "&", 3 }, { "∧", 3 }, { "and", 3 },
            { "|", 2 }, { "∨", 2 }, { "or", 2 },
            { "^", 2 }, { "xor", 2 },
            { "->", 1 }, { "→", 1 },
            { "=", 1 }, { "↔", 1 }
        };

        public ParseResult Parse(string formula)
        {
            var tokens = Tokenize(formula);
            var rpn = ConvertToRPN(tokens);
            return EvaluateRPN(rpn);
        }

        private List<string> Tokenize(string formula)
        {
            var tokens = new List<string>();
            string current = "";

            foreach (char c in formula)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        tokens.Add(current);
                        current = "";
                    }
                    continue;
                }

                if (IsOperatorChar(c) || c == '(' || c == ')')
                {
                    if (!string.IsNullOrEmpty(current))
                    {
                        tokens.Add(current);
                        current = "";
                    }
                    tokens.Add(c.ToString());
                }
                else
                {
                    current += c;
                }
            }

            if (!string.IsNullOrEmpty(current))
                tokens.Add(current);

            return tokens;
        }

        private bool IsOperatorChar(char c)
        {
            return "!¬&∧|∨^→↔=-".Contains(c);
        }

        private List<string> ConvertToRPN(List<string> tokens)
        {
            var output = new List<string>();
            var stack = new Stack<string>();

            foreach (string token in tokens)
            {
                if (IsVariable(token))
                {
                    output.Add(token);
                }
                else if (_operators.ContainsKey(token.ToLower()))
                {
                    while (stack.Count > 0 && _operators.ContainsKey(stack.Peek().ToLower()) &&
                           _operators[stack.Peek().ToLower()] >= _operators[token.ToLower()])
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
                else if (token == "(")
                {
                    stack.Push(token);
                }
                else if (token == ")")
                {
                    while (stack.Count > 0 && stack.Peek() != "(")
                    {
                        output.Add(stack.Pop());
                    }
                    stack.Pop();
                }
            }

            while (stack.Count > 0)
            {
                output.Add(stack.Pop());
            }

            return output;
        }

        private ParseResult EvaluateRPN(List<string> rpn)
        {
            var variables = rpn.Where(IsVariable).Distinct().ToList();
            int variableCount = variables.Count;
            int tableSize = 1 << variableCount;
            var truthTable = new bool[tableSize];
            var varMap = variables.Select((v, i) => new { v, i }).ToDictionary(x => x.v, x => x.i);

            for (int i = 0; i < tableSize; i++)
            {
                var values = new Dictionary<string, bool>();
                for (int j = 0; j < variableCount; j++)
                {
                    values[variables[j]] = ((i >> (variableCount - 1 - j)) & 1) == 1;
                }

                truthTable[i] = EvaluateExpression(rpn, values);
            }

            return new ParseResult
            {
                TruthTable = truthTable,
                VariableCount = variableCount,
                Variables = variables
            };
        }

        private bool EvaluateExpression(List<string> rpn, Dictionary<string, bool> values)
        {
            var stack = new Stack<bool>();

            foreach (string token in rpn)
            {
                if (IsVariable(token))
                {
                    stack.Push(values[token]);
                }
                else
                {
                    switch (token.ToLower())
                    {
                        case "!":
                        case "¬":
                        case "not":
                            stack.Push(!stack.Pop());
                            break;
                        case "&":
                        case "∧":
                        case "and":
                            stack.Push(stack.Pop() & stack.Pop());
                            break;
                        case "|":
                        case "∨":
                        case "or":
                            stack.Push(stack.Pop() | stack.Pop());
                            break;
                        case "^":
                        case "xor":
                            stack.Push(stack.Pop() ^ stack.Pop());
                            break;
                        case "->":
                        case "→":
                            bool b = stack.Pop(), a = stack.Pop();
                            stack.Push(!a | b);
                            break;
                        case "=":
                        case "↔":
                            stack.Push(stack.Pop() == stack.Pop());
                            break;
                    }
                }
            }

            return stack.Pop();
        }

        private bool IsVariable(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
            if (_operators.ContainsKey(token.ToLower())) return false;
            if (token == "(" || token == ")") return false;
            return true;
        }
    }

    public class ParseResult
    {
        public bool[] TruthTable { get; set; }
        public int VariableCount { get; set; }
        public List<string> Variables { get; set; }
    }
}