using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

public static class ExpressionCompiler
{

    private const string TokenPattern = @"(-?\d+\.?\d*)|([\+\-\*\/])|(\()|(\))";
    private const string OperatorsPattern = @"[\+\-\*\/]";

    public static string GetResult(string input)
    {
        var symbol = Regex.Match(input, "[aA-zZ]");
        if (symbol.Length > 0)
        {
            return string.Format("Incorrect expression! Wrong symbol: {0}", symbol);
        }

        try
        {
            return Calculate(ToReversePolishNotation(input)).ToString(CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private static IEnumerable<string> ToReversePolishNotation(string expression)
    {
        var parsed = Regex.Split(expression, TokenPattern).Where(a => !string.IsNullOrEmpty(a) || a != "");
        var output = new Queue<string>();
        var operators = new Stack<char>();
        foreach (var token in parsed)
        {
            float number;
            if (token[0] == '(')
            {
                operators.Push(token[0]);
            }

            else if (token[0] == ')')
            {
                while (operators.Count > 0 && operators.Peek() != '(')
                {
                    if (operators.Count == 0)
                        throw new ArgumentException("Brackets isn't balanced!");
                    output.Enqueue(operators.Pop().ToString());
                }

                operators.Pop();
            }

            else if (float.TryParse(token, out number))
            {
                output.Enqueue(token);
            }

            else if (Regex.IsMatch(token, OperatorsPattern))
            {
                while (operators.Count > 0 && operators.Peek() != '(' && GetPrecedence(operators.Peek()) >= GetPrecedence(token[0]))
                {
                    output.Enqueue(operators.Pop().ToString());
                }

                operators.Push(token[0]);
            }
        }

        while (operators.Count > 0)
        {
            if (operators.Peek() == '(' || operators.Peek() == ')')
                throw new ArgumentException("Brackets isn't balanced!");
            output.Enqueue(operators.Pop().ToString());
        }
        return output;
    }

    private static float Calculate(IEnumerable<string> expression)
    {
        var expressions = new Stack<float>();
        var tokens = expression.ToList();
        foreach (var token in tokens)
        {
            float result;
            if (float.TryParse(token, out result))
            {
                expressions.Push(result);
            }
            else
            {
                try
                {
                    switch (token)
                    {
                        case "*":
                            expressions.Push(expressions.Pop() * expressions.Pop());
                            break;
                        case "/":
                            var second = expressions.Pop();
                            if (second == 0.0f)
                                throw new DivideByZeroException();
                            expressions.Push(expressions.Pop() / second);
                            break;
                        case "+":
                            expressions.Push(expressions.Pop() + expressions.Pop());
                            break;
                        case "-":
                            expressions.Push(expressions.Pop() - expressions.Pop());
                            break;
                        default:
                            throw new InvalidOperationException("Invalid operation!");
                    }
                }
                catch
                {
                    throw new InvalidOperationException("Syntax error! Wrong symbol index: " + tokens.LastIndexOf(token) + "(without whitespaces)");
                }
            }
        }

        return expressions.Pop();
    }

    private static int GetPrecedence(char op)
    {
        if (op == '+' || op == '-')
            return 0;
        if (op == '*' || op == '/')
        {
            return 1;
        }

        throw new FormatException(string.Format("{0} is not an operator!", op));
    }
}