using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    public partial class EventRuntime
    {

        static Dictionary<string, Field> fields = new Dictionary<string, Field>
        {
			//Map Fields
			{ "turn",         new TypedField<int>   (c => { return c.map.turn; })                  },
            { "seed",         new TypedField<int>   (c => { return (int)c.map.seed; })                  },
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
            { "is_sane",   new TypedField<bool>  (c => {return c.person.madness is Insanity_Sane; })},
            { "shadow",   new TypedField<int>  (c => {return (int)(c.person.shadow*100); })},

			//Unit 

			//Location
            { "is_coastal",   new TypedField<bool>  (c => {return (c.location.isCoastal); })},
            { "infiltration",   new TypedField<int>  (c => {if (c.location == null || c.location.settlement == null){return 0; } return (int)(100*c.location.settlement.infiltration); })},
            { "is_church",   new TypedField<bool>  (c => {if (c.location == null || c.location.settlement == null){return false; }return c.location.settlement is Set_Abbey; })},
            { "is_ruins",   new TypedField<bool>  (c => {if (c.location == null || c.location.settlement == null){return false; }return (c.location.settlement is Set_Ruins || c.location.settlement is Set_CityRuins); })},
            { "is_town",   new TypedField<bool>  (c => {if (c.location == null || c.location.settlement == null){return false; }return c.location.settlement is Set_City city && city.getLevel() == Set_City.LEVEL_TOWN; })},
            { "is_village",   new TypedField<bool>  (c => {if (c.location == null || c.location.settlement == null){return false; }return c.location.settlement is Set_City city && city.getLevel() == Set_City.LEVEL_VILLAGE; })},
            { "is_hamlet",   new TypedField<bool>  (c => {if (c.location == null || c.location.settlement == null){return false; }return c.location.settlement is Set_City city && city.getLevel() == Set_City.LEVEL_HAMLET; })},
            { "is_city",   new TypedField<bool>  (c => {if (c.location == null || c.location.settlement == null){return false; }return c.location.settlement is Set_City city && city.getLevel() == Set_City.LEVEL_CITY; })},
            { "is_metropole",   new TypedField<bool>  (c => {if (c.location == null || c.location.settlement == null){return false; }return c.location.settlement is Set_City city && city.getLevel() == Set_City.LEVEL_METROPOLE; })},
            { "is_empty",   new TypedField<bool>  (c => {return (c.location.settlement == null); })},
            { "is_desert",   new TypedField<bool>  (c => {return (c.location.hex.terrain == Hex.terrainType.DESERT); })},
            { "is_dry",   new TypedField<bool>  (c => {return (c.location.hex.terrain == Hex.terrainType.DRY); })},
            { "is_snow",   new TypedField<bool>  (c => {return (c.location.hex.terrain == Hex.terrainType.SNOW); })},
            { "is_under_fog",   new TypedField<bool>  (c => {return (c.location.hex.cloud is Cloud_Fog); })},
            { "is_enthralled_location",   new TypedField<bool>  (c => {if (c.location == null || c.location.settlement == null){return false; }return c.location.settlement.title != null && c.location.settlement.title.heldBy != null && c.location.settlement.title.heldBy.state == Person.personState.enthralled; })},
            { "location_shadow", new TypedField<int>(c =>{if (c.location.person() == null){return (int)(100*(1-c.location.hex.purity)); } return (int)(c.location.person().shadow*100); }) },
            { "location_index",   new TypedField<int>  (c => {return (c.location.index); })},

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
                    if (c.map.overmind.enthralled.evidence < 0){c.map.overmind.enthralled.evidence = 0; }
                } }
            )},
            { "ENTHRALLED_GAINS_PRESTIGE", new TypedProperty<int>((c, v) => {
                if (c.map.overmind.enthralled != null)
                {
                    c.map.overmind.enthralled.prestige += v;
                    if (c.map.overmind.enthralled.prestige < 1){c.map.overmind.enthralled.prestige = 1; }
                } }
            )},

			//Person effects
			{ "LOSE_SANITY", new TypedProperty<int>((c, v) => { c.person.sanity -= v;if(c.person.sanity < 0){c.person.sanity = 0; } }) },
            { "GAIN_LIKING_FOR_ENTHRALLED", new TypedProperty<int>((c, v) => {
                if (c.map.overmind.enthralled != null){
                    c.person.getRelation(c.map.overmind.enthralled).addLiking(v,"From Event",c.map.turn);
                } }) },
            { "GAIN_SUSPICION_FOR_ENTHRALLED", new TypedProperty<int>((c, v) => {
                if (c.map.overmind.enthralled != null){
                    RelObj rel = c.person.getRelation(c.map.overmind.enthralled);
                    rel.suspicion += v/100d;
                    if (rel.suspicion > 1){rel.suspicion = 1; }
                    if (rel.suspicion < 1){rel.suspicion = 0; }
                } }) },
            { "GAIN_SHADOW", new TypedProperty<int>((c, v) => {
            c.person.shadow += v/100d;
                if (c.person.shadow > 1)
                {
                    c.person.shadow = 1;
                }else if (c.person.shadow < 0)
                {
                    c.person.shadow = 0;
                }
            }) },
            { "GAIN_EVIDENCE", new TypedProperty<int>((c, v) => {
            c.person.evidence += v/100d;
                if (c.person.evidence > 1)
                {
                    c.person.evidence = 1;
                }else if (c.person.evidence < 0)
                {
                    c.person.evidence = 0;
                }
            }) },
            { "RANDOM_SUSPICION", new TypedProperty<int>((c, v) => {
                    int k = 0;
                    Person vic = null;
                foreach (Person p2 in c.person.society.people)
                {
                    if (p2 == c.person){continue; }
                    if (c.person.getRelation(p2).suspicion >= 1){continue; }
                    k += 1;
                    if (Eleven.random.Next(k) == 0)
                    {
                        vic = p2;
                    }
                }
                if (vic != null)
                {
                    c.person.getRelation(vic).suspicion += v/100d;
                    if (c.person.getRelation(vic).suspicion > 1){c.person.getRelation(vic).suspicion = 1; }
                }
            }) },
            { "KILL_SOVEREIGN", new TypedProperty<string>((c, v) => {
                if (c.person.society.getSovereign() != null)
                {
                    c.person.society.getSovereign().die(v,false);
                }
            }) },
            { "KILL_PERSON", new TypedProperty<string>((c, v) => {
                c.person.die(v,false);
            }) },

			//Location effects
			{ "GAIN_INFILTRATION", new TypedProperty<int>((c, v) => {
                if (c.location.settlement != null){
            c.location.settlement.infiltration += v/100d;
                if (c.location.settlement.infiltration > 1)
                {
                    c.location.settlement.infiltration = 1;
                }else if (c.location.settlement.infiltration < 0)
                {
                    c.location.settlement.infiltration = 0;
                }
                }
            }) },
            { "LOCATION_GAIN_SHADOW", new TypedProperty<int>((c, v) => {
                if (c.location.person() != null)
                {
                    Person p = c.location.person();
                    p.shadow += v/100d;
                    if (p.shadow > 1)
                    {
                        p.shadow = 1;
                    }else if (p.shadow < 0)
                    {
                        p.shadow = 0;
                    }
                }
            }) },
            { "DESTROY_LOCATION", new TypedProperty<string>((c, v) => {
                if (c.location.person() != null)
                {
                    c.location.person().die(v,true);
                }
                if (c.location.settlement != null)
                {
                    if (c.location.settlement is SettlementHuman)
                    {
                        c.location.settlement.fallIntoRuin();
                    }
                    else
                    {
                        c.location.settlement = null;
                    }
                    
                }
            }) },
            { "START_ROTTING_SICKNESS", new TypedProperty<string>((c, v) => {
                 Assets.Code.Property.addProperty(c.map,c.location,"Rotting Sickness");
            }) },

            //Unit effects
            
            { "LEAVE_MINOR_EVIDENCE", new TypedProperty<string>((c, v) => {
                double amount = c.map.param.unit_minorEvidence;
                Evidence e2 = new Evidence(c.map.turn);
                e2.pointsTo = c.unit;
                e2.weight = amount;
                c.unit.location.evidence.Add(e2);
            }) },

            { "SHOW_EVENT", new TypedProperty<string>((c, v) => {
                if (EventManager.events.ContainsKey(v))
                    c.map.world.prefabStore.popEvent(EventManager.events[v].data, c);
            }) }
        };
    }
}
