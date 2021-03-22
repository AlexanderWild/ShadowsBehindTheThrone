using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Code
{
	public class EventParser
	{
		public class Token
		{
			public enum Type
			{
				LOCATION_FIELD,
				PERSON_FIELD,
				UNIT_FIELD,
				WORLD_FIELD,
				VARIABLE,
				NUMBER,
				BOOLEAN,
				LPAREN,
				RPAREN,
				EQUALS,
				NEQUALS,
				LESS,
				GREATER,
				PLUS,
				MINUS,
				AND,
				OR,
				NOT
			}

			public Type type;
			public string value;

			public Token(Type t, string v)
			{
				type = t;
				value = v;
			}
		}

		class TokenExpression
		{
			Token.Type type;
			Regex regex;

			public TokenExpression(Token.Type t, string r)
			{
				type = t;
				regex = new Regex(r);
			}

			public Token match(ref string input)
			{
				var res = regex.Match(input);
				if (!res.Success)
					return null;

				input = input.Substring(res.Length);
				return new Token(type, res.Value);
			}
		}

		public class SyntaxNode
		{
			public Token token;

			public SyntaxNode(Token t) { token = t; }
		}

		public class BinaryNode : SyntaxNode
		{
			public SyntaxNode lhs;
			public SyntaxNode rhs;

			public BinaryNode(Token t, SyntaxNode l, SyntaxNode r) : base(t)
			{
				lhs = l;
				rhs = r;
			}
		}

		public class UnaryNode : SyntaxNode
		{
			public SyntaxNode child;

			public UnaryNode(Token t, SyntaxNode c) : base(t) { child = c; }
		}

		static List<TokenExpression> expressions = new List<TokenExpression>
		{
			new TokenExpression(Token.Type.NUMBER,  "^\\d+(\\.\\d+)?"),
			new TokenExpression(Token.Type.BOOLEAN, "^(true|false)"),
			new TokenExpression(Token.Type.LPAREN,  "^\\("),
			new TokenExpression(Token.Type.RPAREN,  "^\\)"),
			new TokenExpression(Token.Type.PLUS,    "^\\+"),
			new TokenExpression(Token.Type.MINUS,   "^\\-"),
			new TokenExpression(Token.Type.EQUALS,  "^="),
			new TokenExpression(Token.Type.NEQUALS, "^~"),
			new TokenExpression(Token.Type.LESS,    "^<"),
			new TokenExpression(Token.Type.GREATER, "^>"),
			new TokenExpression(Token.Type.AND,     "^&"),
			new TokenExpression(Token.Type.OR,      "^\\|"),
			new TokenExpression(Token.Type.NOT,     "^!"),

			new TokenExpression(Token.Type.LOCATION_FIELD, "^location\\.[a-zA-Z_]+"),
			new TokenExpression(Token.Type.PERSON_FIELD,   "^person\\.[a-zA-Z_]+"),
			new TokenExpression(Token.Type.UNIT_FIELD,     "^unit\\.[a-zA-Z_]+"),
			new TokenExpression(Token.Type.WORLD_FIELD,    "^[a-zA-Z_]+"),

			new TokenExpression(Token.Type.VARIABLE, "^\\$[a-zA-Z_]+")
		};

		public static List<Token> tokenize(string expr)
		{
			List<Token> res = new List<Token>();
			while (!String.IsNullOrWhiteSpace(expr))
			{
				Token t = null;
				foreach (var te in expressions)
				{
					t = te.match(ref expr);
					if (t != null)
						break;
				}

				if (t == null)
				{
					World.Log("unexpected token: " + expr.Remove(1));
					throw new Exception("could not parse event expression.");
				}

				res.Add(t);
				expr = expr.TrimStart();
			}

			return res;
		}

		// expression = term | expression ("&"|"|") term
		// term = factor | term ("="|"~") factor
		// factor = atom | "!" factor
		// atom = NUMBER | BOOLEAN | *_FIELD | "(" expression ")"
		public static SyntaxNode parse(List<Token> tokens)
		{
			var e = tokens.GetEnumerator();
			tryMoveNext(ref e);

			return parseBinaryExpression(ref e);
		}

		static SyntaxNode parseBinaryExpression(ref List<Token>.Enumerator ts)
		{
			SyntaxNode lhs = parseEqualityExpression(ref ts);

			SyntaxNode last = lhs;
			while (true)
			{
				Token t = ts.Current;
				if (t == null)
					return last;

				switch (t.type)
				{
					case Token.Type.AND:
					case Token.Type.OR:
						tryMoveNext(ref ts);

						SyntaxNode rhs = parseEqualityExpression(ref ts);
						last = new BinaryNode(t, last, rhs);

						break;
					default:
						return last;
				}
			}
		}

		static SyntaxNode parseEqualityExpression(ref List<Token>.Enumerator ts)
		{
			SyntaxNode lhs = parseArithmeticExpression(ref ts);

			Token t = ts.Current;
			if (t == null)
				return lhs;

			switch (t.type)
			{
				case Token.Type.EQUALS:
				case Token.Type.NEQUALS:
				case Token.Type.LESS:
				case Token.Type.GREATER:
					tryMoveNext(ref ts);

					SyntaxNode rhs = parseArithmeticExpression(ref ts);
					return new BinaryNode(t, lhs, rhs);
				default:
					return lhs;
			}
		}

		static SyntaxNode parseArithmeticExpression(ref List<Token>.Enumerator ts)
		{
			SyntaxNode lhs = parseUnaryExpression(ref ts);

			SyntaxNode last = lhs;
			while (true)
			{
				Token t = ts.Current;
				if (t == null)
					return last;

				switch (t.type)
				{
					case Token.Type.PLUS:
					case Token.Type.MINUS:
						tryMoveNext(ref ts);

						SyntaxNode rhs = parseUnaryExpression(ref ts);
						last = new BinaryNode(t, last, rhs);

						break;
					default:
						return last;
				}
			}
		}

		static SyntaxNode parseUnaryExpression(ref List<Token>.Enumerator ts)
		{
			Token t = ts.Current;
			switch (t.type)
			{
				case Token.Type.MINUS:
				case Token.Type.NOT:
					tryMoveNext(ref ts);

					SyntaxNode child = parseAtomExpression(ref ts);
					return new UnaryNode(t, child);
				default:
					return parseAtomExpression(ref ts);
			}
		}

		static SyntaxNode parseAtomExpression(ref List<Token>.Enumerator ts)
		{
			Token t = ts.Current;
			switch (t.type)
			{
				case Token.Type.NUMBER:
				case Token.Type.BOOLEAN:
				case Token.Type.LOCATION_FIELD:
				case Token.Type.PERSON_FIELD:
				case Token.Type.UNIT_FIELD:
				case Token.Type.WORLD_FIELD:
				{
					SyntaxNode atom = new SyntaxNode(t);

					ts.MoveNext();
					return atom;
				}
				case Token.Type.VARIABLE:
				{
					t.value = t.value.Substring(1);
					SyntaxNode atom = new SyntaxNode(t);

					ts.MoveNext();
					return atom;
				}
				case Token.Type.LPAREN:
				{
					tryMoveNext(ref ts);
					SyntaxNode expr = parseBinaryExpression(ref ts);

					if (ts.Current.type != Token.Type.RPAREN)
						throw new Exception("missing closing parenthesis.");

					ts.MoveNext();
					return expr;
				}

				default: throw new Exception("unexpected token in atom.");
			}
		}

		static void tryMoveNext(ref List<Token>.Enumerator ts)
		{
			if (!ts.MoveNext())
				throw new Exception("unexpected end of token stream.");
		}
	}
}
