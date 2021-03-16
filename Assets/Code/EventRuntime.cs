using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class EventRuntime
    {
        public enum Type { DOUBLE, INT, BOOL }
		abstract class Atom
		{
			public Type type;

			protected void deduceType<T>()
			{
				var t = default(T);
				if (t is double _)
					type = Type.DOUBLE;
				else if (t is int _)
					type = Type.INT;
				else if (t is bool _)
					type = Type.BOOL;
				else
					throw new Exception("invalid event runtime type.");
			}
		}

		abstract class Value : Atom {}
		class TypedValue<T> : Value
		{
			public T data;

			public TypedValue(T d)
			{
				deduceType<T>();
				data = d;
			}
		}

		abstract class Field : Atom { public abstract Value getValue(EventContext _); }
		class TypedField<T> : Field
		{
			public delegate T Accessor(EventContext _);
			public Accessor accessor;

			public TypedField(Accessor a)
			{
				var t = default(T);
				if (t is double)
					type = Type.DOUBLE;
				else if (t is int)
					type = Type.INT;
				else if (t is bool)
					type = Type.BOOL;
				else
					throw new Exception("invalid EventSymbol field type.");

				accessor = a;
			}

			public override Value getValue(EventContext ctx)
			{
				return new TypedValue<T>(accessor(ctx));
			}
		}

		static Dictionary<string, Field> fields = new Dictionary<string, Field>
		{
			{ "turn",         new TypedField<int>   (c => { return c.map.turn; })                  },
			{ "enshadowment", new TypedField<double>(c => { return c.map.data_avrgEnshadowment; }) },
			{ "awareness",    new TypedField<double>(c => { return c.map.data_awarenessSum; })     },
			{ "temperature",  new TypedField<double>(c => { return c.map.data_globalTempSum; })    },
			{ "panic",        new TypedField<double>(c => { return c.map.worldPanic; })            },
			{ "has_agents",   new TypedField<bool>  (c => { return c.map.hasEnthralledAnAgent; })  }
		};

		public static bool evaluate(EventParser.SyntaxNode root, EventContext ctx)
		{
			Value res = evaluateNode(root, ctx);
			if (res is TypedValue<bool> resb)
				return resb.data;
			else
				throw new Exception("event conditional does not evaluate to boolean.");
		}

		static Value evaluateNode(EventParser.SyntaxNode n, EventContext ctx)
		{
			if (n is EventParser.BinaryNode bn)
			{
				Value a = evaluateNode(bn.lhs, ctx);
				Value b = evaluateNode(bn.rhs, ctx);

				(a, b) = promoteTypes(a, b);
				switch (a.type)
				{
					case Type.DOUBLE:
					{
						double av = (a as TypedValue<double>).data;
						double bv = (b as TypedValue<double>).data;

						switch (bn.token.type)
						{
							case EventParser.Token.Type.EQUALS:  return new TypedValue<bool>(av == bv);
							case EventParser.Token.Type.NEQUALS: return new TypedValue<bool>(av != bv);
						
							default: throw new Exception("invalid operation on double types.");
						}
					}
					case Type.INT:
					{
						int av = (a as TypedValue<int>).data;
						int bv = (b as TypedValue<int>).data;

						switch (bn.token.type)
						{
							case EventParser.Token.Type.EQUALS:  return new TypedValue<bool>(av == bv);
							case EventParser.Token.Type.NEQUALS: return new TypedValue<bool>(av != bv);
						
							default: throw new Exception("invalid operation on int types.");
						}
					}
					case Type.BOOL:
					{
						bool av = (a as TypedValue<bool>).data;
						bool bv = (b as TypedValue<bool>).data;

						switch (bn.token.type)
						{
							case EventParser.Token.Type.EQUALS:  return new TypedValue<bool>(av == bv);
							case EventParser.Token.Type.NEQUALS: return new TypedValue<bool>(av != bv);
						
							case EventParser.Token.Type.AND: return new TypedValue<bool>(av && bv);
							case EventParser.Token.Type.OR:  return new TypedValue<bool>(av || bv);

							default: throw new Exception("invalid operation on bool types.");
						}
					}
				}
			}
			else if (n is EventParser.UnaryNode un)
			{
				Value e = evaluateNode(un.child, ctx);
				switch (e.type)
				{
					case Type.DOUBLE:
						throw new Exception("invalid operation on double type.");
					case Type.INT:
						throw new Exception("invalid operation on int type.");
					case Type.BOOL:
					{
						bool ev = (e as TypedValue<bool>).data;

						switch (un.token.type)
						{
							case EventParser.Token.Type.NOT: return new TypedValue<bool>(!ev);

							default: throw new Exception("invalid operation on bool type.");
						}
					}
				}
			}
			else
			{
				switch (n.token.type)
				{
					case EventParser.Token.Type.NUMBER:
						if (n.token.value.Contains('.'))
							return new TypedValue<double>(Convert.ToDouble(n.token.value));
						else
							return new TypedValue<int>(Convert.ToInt32(n.token.value));
					case EventParser.Token.Type.BOOLEAN:
						return new TypedValue<bool>(Convert.ToBoolean(n.token.value));

					case EventParser.Token.Type.LOCATION_FIELD:
					case EventParser.Token.Type.UNIT_FIELD:
					case EventParser.Token.Type.WORLD_FIELD:
						if (!fields.ContainsKey(n.token.value))
							throw new Exception("unknown field name.");
						else
							return fields[n.token.value].getValue(ctx);

					default: throw new Exception("invalid atom type.");
				}
			}

			throw new Exception("unable to evaluate node.");
		}

		static (Value, Value) promoteTypes(Value a, Value b)
		{
			if (a.type == b.type)
				return (a, b);

			switch (a.type)
			{
				case Type.DOUBLE:
					if (b.type == Type.INT)
						return (a, new TypedValue<double>((b as TypedValue<int>).data));
					else
						throw new Exception("cannot convert bool to double.");
				case Type.INT:
					if (b.type == Type.DOUBLE)
						return (new TypedValue<double>((a as TypedValue<int>).data), b);
					else
						throw new Exception("cannot convert bool to int.");

				default: throw new Exception("incompatible types in binary operands.");
			}
		}
    }
}
