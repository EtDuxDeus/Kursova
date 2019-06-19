using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace graphic
{
	static class Parser1
	{
		static public double Calculate(string Expression, double x)
		{
			string expr = Expression;
			Parser p = new Parser();
			Regex regEx = new Regex("x", RegexOptions.IgnoreCase);
			expr = regEx.Replace(expr, x.ToString());
			regEx = new Regex("sqrt", RegexOptions.IgnoreCase);
			expr = regEx.Replace(expr, "1k");
			regEx = new Regex("sin", RegexOptions.IgnoreCase);
			expr = regEx.Replace(expr, "1s");
			regEx = new Regex("cos", RegexOptions.IgnoreCase);
			expr = regEx.Replace(expr, "1c");
			regEx = new Regex("exp", RegexOptions.IgnoreCase);
			expr = regEx.Replace(expr, "1e");
			regEx = new Regex("ln", RegexOptions.IgnoreCase);
			expr = regEx.Replace(expr, "1l");
			double y = p.Evaluate(expr);
			return y;
		}
		class ParserException : ApplicationException
		{
			public ParserException(string str) : base(str) { }

			public override string ToString()
			{
				return Message;
			}
		}

		class Parser
		{
			enum Types { NONE, DELIMITER, NUMBER };

			enum Errors { SYNTAX, UNBALPARENS, NOEXP, DIVBYZERO };

			string exp;
			int expIdx;
			string token;
			Types tokType;

			public double Evaluate(string expstr)
			{
				double result;
				exp = expstr;
				expIdx = 0;
				try
				{
					GetToken();
					if (token == "")
					{
						SyntaxErr(Errors.NOEXP);

						return 0.0;
					}
					EvalExp1(out result);
					if (token != "")
						SyntaxErr(Errors.SYNTAX);
					return result;
				}
				catch (ParserException)
				{
					return 0.0;
				}
			}


			void EvalExp1(out double result)
			{
				string op;
				double partialResult;
				EvalExp2(out result);
				while ((op = token) == "+" || op == "-")
				{
					GetToken();
					EvalExp2(out partialResult);
					switch (op)
					{
						case "-":
							result = result - partialResult;
							break;
						case "+":
							result = result + partialResult;
							break;
					}
				}
			}
			void EvalExp2(out double result)
			{
				string op;
				double partialResult = 0.0;
				EvalExp3(out result);
				while ((op = token) == "*" || op == "/" || op == "%")
				{
					GetToken();
					EvalExp3(out partialResult);
					switch (op)
					{
						case "*":
							result = result * partialResult;
							break;
						case "/":
							if (partialResult == 0.0)
								SyntaxErr(Errors.DIVBYZERO);
							result = result / partialResult;
							break;
					}
				}
			}

			void EvalExp3(out double result)
			{
				double partialResult;
				EvalExp4(out result);
				if (token == "^")
				{
					GetToken();
					EvalExp3(out partialResult);
					result = Math.Pow(result, partialResult);
				}
				if (token == "k")
				{
					GetToken();
					EvalExp3(out result);
					result = Math.Sqrt(result);
				}
				if (token == "s")
				{
					GetToken();
					EvalExp3(out result);
					result = Math.Sin(result);
				}
				if (token == "c")
				{
					GetToken();
					EvalExp3(out result);
					result = Math.Cos(result);
				}
				if (token == "e")
				{
					GetToken();
					EvalExp3(out result);
					result = Math.Exp(result);
				}
				if (token == "l")
				{
					GetToken();
					EvalExp3(out partialResult);
					if (partialResult <= 0)
					{
						SyntaxErr(Errors.DIVBYZERO);
					}
					result = Math.Log(partialResult);
				}
			}


			void EvalExp4(out double result)
			{
				string op;
				op = "";
				if ((tokType == Types.DELIMITER) && token == "+" || token == "-")
				{
					op = token;
					GetToken();
				}
				EvalExp5(out result);
				if (op == "-") result = -result;
			}


			void EvalExp5(out double result)
			{
				if (token == "(")
				{
					GetToken();
					EvalExp1(out result);
					if (token != ")")
						SyntaxErr(Errors.UNBALPARENS);
					GetToken();
				}
				else Atom(out result);
			}
			void Atom(out double result)
			{
				switch (tokType)
				{
					case Types.NUMBER:
						try
						{
							result = Double.Parse(token);
						}
						catch (FormatException)
						{
							result = 0.0;
							SyntaxErr(Errors.SYNTAX);
						}
						GetToken();
						return;
					default:
						result = 0.0;
						SyntaxErr(Errors.SYNTAX);
						break;

				}
			}


			void SyntaxErr(Errors error)
			{
				string err ="";
				throw new ParserException(err);
			}


			void GetToken()
			{
				tokType = Types.NONE;
				token = "";
				if (expIdx == exp.Length) return;
				while (expIdx < exp.Length && Char.IsWhiteSpace(exp[expIdx])) ++expIdx;

				if (expIdx == exp.Length) return;
				if (IsDelim(exp[expIdx]))
				{
					token += exp[expIdx];
					expIdx++;
					tokType = Types.DELIMITER;
				}
				else if (Char.IsDigit(exp[expIdx]))
				{
					while (!IsDelim(exp[expIdx]))
					{
						token += exp[expIdx];
						expIdx++;
						if (expIdx >= exp.Length)
							break;
					}
					tokType = Types.NUMBER;
				}
			}

			bool IsDelim(char c)
			{
				if (("+-/*^()kscel".IndexOf(c) != -1)) return true;
				return false;
			}
		}

	}
}
