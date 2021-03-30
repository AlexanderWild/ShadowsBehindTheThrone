using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class EventRuntime
    {
        enum Type { DOUBLE, INT, BOOL, STRING }
		abstract class Atom
		{
			public Type type;

			protected void deduceType<T>()
			{
				var t = typeof(T);
				if (t == typeof(double))
					type = Type.DOUBLE;
				else if (t == typeof(int))
					type = Type.INT;
				else if (t == typeof(bool))
					type = Type.BOOL;
				else if (t == typeof(string))
					type = Type.STRING;
				else
					throw new Exception("invalid event runtime type.");
			}
		}

		abstract class Value : Atom { public abstract string ToString(); }
		class TypedValue<T> : Value
		{
			public T data;

			public TypedValue(T d)
			{
				deduceType<T>();
				data = d;
			}

			public override string ToString()
			{
				return data.ToString();
			}
		}

		abstract class Field : Atom { public abstract Value getValue(EventContext ctx); }
		class TypedField<T> : Field
		{
			public delegate T Getter(EventContext c);
			public Getter getter;

			public TypedField(Getter g)
			{
				deduceType<T>();
				getter = g;
			}

			public override Value getValue(EventContext ctx)
			{
				return new TypedValue<T>(getter(ctx));
			}
		}

		abstract class Property : Atom { public abstract void setValue(EventContext ctx, string s); }
		class TypedProperty<T> : Property
		{
			public delegate void Setter(EventContext c, T v);
			public Setter setter;

			public TypedProperty(Setter s)
			{
				deduceType<T>();
				setter = s;
			}

			public override void setValue(EventContext ctx, string s)
			{
				var value = (T)Convert.ChangeType(s, typeof(T));
				setter(ctx, value);
			}
		}

		static Dictionary<string, Field> fields = new Dictionary<string, Field>
		{
			//Map Fields
			{ "turn",         new TypedField<int>   (c => { return c.map.turn; })                  },
			{ "enshadowment", new TypedField<double>(c => { return c.map.data_avrgEnshadowment; }) },
			{ "awareness",    new TypedField<double>(c => { return c.map.data_awarenessSum; })     },
			{ "temperature",  new TypedField<double>(c => { return c.map.data_globalTempSum; })    },
			{ "panic",        new TypedField<double>(c => { return c.map.worldPanic; })            },
			{ "has_agents",   new TypedField<bool>  (c => { return c.map.hasEnthralledAnAgent; })  },
			{ "has_enthralled",   new TypedField<bool>  (c => { return c.map.overmind.enthralled != null; })  },

			//Person Fields
			{ "liking_for_enthralled",   new TypedField<double>  (c => {
				if (c.map.overmind.enthralled == null){return 0; }
				return c.person.getRelation(c.map.overmind.enthralled).getLiking(); })  },
			{ "is_in_enthralled_nation",   new TypedField<bool>  (c => {
				if (c.map.overmind.enthralled == null){return false; }
				return c.person.society == c.map.overmind.enthralled.society; })  },
			{ "is_landed",   new TypedField<bool>  (c => {return c.person.title_land != null; })},
		};

		static Dictionary<string, Property> properties = new Dictionary<string, Property>
		{
			{ "ADD_POWER", new TypedProperty<int>((c, v) => { c.map.overmind.power += v; }) },
			{ "SUB_POWER", new TypedProperty<int>((c, v) => { c.map.overmind.power -= v; }) },

			{ "ENTHRALLED_GAINS_EVIDENCE", new TypedProperty<int>((c, v) => {
				if (c.map.overmind.enthralled != null)
				{
					c.map.overmind.enthralled.evidence += (v/100d);
					if (c.map.overmind.enthralled.evidence > 1){c.map.overmind.enthralled.evidence = 1; }
				} }
			)},

			//Person effects
			{ "LOSE_SANITY", new TypedProperty<int>((c, v) => { c.person.sanity -= v;if(c.person.sanity < 0){c.person.sanity = 0; } }) },
			{ "GAIN_LIKING_FOR_ENTHRALLED", new TypedProperty<int>((c, v) => {
				if (c.map.overmind.enthralled != null){
					c.person.getRelation(c.map.overmind.enthralled).addLiking(v,"From Event",c.map.turn);
				} }) },
			{ "GAIN_LIKING_FOR_ENTHRALLED", new TypedProperty<int>((c, v) => {
				if (c.map.overmind.enthralled != null){
					RelObj rel = c.person.getRelation(c.map.overmind.enthralled);
					rel.suspicion += v/100d;
					if (rel.suspicion > 1){rel.suspicion = 1; }
				} }) },

			{ "SHOW_EVENT", new TypedProperty<string>((c, v) => {
				foreach (EventManager.ActiveEvent ev in EventManager.events)
				{
					if (ev.data.id == v)
					{
						World.self.prefabStore.popEvent(ev.data, c);
						break;
					}
				}
			}) }
		};

		public static bool evaluate(EventParser.SyntaxNode root, EventContext ctx)
		{
			if (evaluateNode(root, ctx) is TypedValue<bool> resb)
				return resb.data;
			else
				throw new Exception("expression does not evaluate to boolean.");
		}

		public static void evaluate(EventData.Effect e, EventContext ctx)
		{
			if (!properties.ContainsKey(e.command))
				throw new Exception("unknown property name.");
			else
				properties[e.command].setValue(ctx, e.argument);
		}

		public static string evaluateAny(EventParser.SyntaxNode root, EventContext ctx)
		{
			return evaluateNode(root, ctx).ToString();
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
							case EventParser.Token.Type.PLUS:  return new TypedValue<double>(av + bv);
							case EventParser.Token.Type.MINUS: return new TypedValue<double>(av - bv);

							case EventParser.Token.Type.EQUALS:  return new TypedValue<bool>(av == bv);
							case EventParser.Token.Type.NEQUALS: return new TypedValue<bool>(av != bv);
							case EventParser.Token.Type.LESS:    return new TypedValue<bool>(av <  bv);
							case EventParser.Token.Type.GREATER: return new TypedValue<bool>(av >  bv);
						
							default: throw new Exception("invalid operation on double types.");
						}
					}
					case Type.INT:
					{
						int av = (a as TypedValue<int>).data;
						int bv = (b as TypedValue<int>).data;

						switch (bn.token.type)
						{
							case EventParser.Token.Type.PLUS:  return new TypedValue<int>(av + bv);
							case EventParser.Token.Type.MINUS: return new TypedValue<int>(av - bv);

							case EventParser.Token.Type.EQUALS:  return new TypedValue<bool>(av == bv);
							case EventParser.Token.Type.NEQUALS: return new TypedValue<bool>(av != bv);
							case EventParser.Token.Type.LESS:    return new TypedValue<bool>(av <  bv);
							case EventParser.Token.Type.GREATER: return new TypedValue<bool>(av >  bv);
						
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
					{
						double ev = (e as TypedValue<double>).data;

						switch (un.token.type)
						{
							case EventParser.Token.Type.MINUS: return new TypedValue<double>(-ev);
							
							default: throw new Exception("invalid operation on double type.");
						}
					}
					case Type.INT:
					{
						int ev = (e as TypedValue<int>).data;

						switch (un.token.type)
						{
							case EventParser.Token.Type.MINUS: return new TypedValue<int>(-ev);
							
							default: throw new Exception("invalid operation on in type.");
						}
					}
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
					case EventParser.Token.Type.PERSON_FIELD:
					case EventParser.Token.Type.UNIT_FIELD:
					case EventParser.Token.Type.WORLD_FIELD:
						if (!fields.ContainsKey(n.token.value))
							throw new Exception("unknown field name.");
						else
							return fields[n.token.value].getValue(ctx);

					case EventParser.Token.Type.VARIABLE:
						return new TypedValue<string>(ctx.readEnvironment(n.token.value));

					default: throw new Exception("invalid atom type.");
				}
			}

			throw new Exception("unable to evaluate node.");
		}

		static (Value, Value) promoteTypes(Value a, Value b)
		{
			if (a.type == Type.STRING)
				a = coerceType(a as TypedValue<string>, b.type);
			if (b.type == Type.STRING)
				b = coerceType(b as TypedValue<string>, a.type);

			if (a.type == b.type)
				return (a, b);

			switch (a.type)
			{
				case Type.DOUBLE:
					if (b.type == Type.INT)
						return (a, new TypedValue<double>((b as TypedValue<int>).data));
					else
						throw new Exception("cannot convert operand to double.");
				case Type.INT:
					if (b.type == Type.DOUBLE)
						return (new TypedValue<double>((a as TypedValue<int>).data), b);
					else
						throw new Exception("cannot convert operand to int.");

				default: throw new Exception("incompatible types in binary operands.");
			}
		}

		static Value coerceType(TypedValue<string> v, Type hint)
		{
			switch (hint)
			{
				case Type.DOUBLE:
					if (v.data == "")
						return new TypedValue<double>(0.0);
					else
						return new TypedValue<double>(Convert.ToDouble(v.data));
				case Type.INT:
					if (v.data == "")
						return new TypedValue<int>(0);
					else
						return new TypedValue<int>(Convert.ToInt32(v.data));
				case Type.BOOL:
					if (v.data == "")
						return new TypedValue<bool>(false);
					else
						return new TypedValue<bool>(Convert.ToBoolean(v.data));
				default:
					return v;
			}
		}
    }
}
