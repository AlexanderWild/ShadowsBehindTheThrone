using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Property
    {
        public Location location;
        public Property_Prototype proto;
        public int charge;
        public GraphicalProperty outer;

        public Property(Property_Prototype proto,Location loc)
        {
            this.proto = proto;
            this.location = loc;
        }

        public virtual void endProperty(Location l)
        {
        }

        public static Property addProperty(Map map, Location location, string name)
        {
            if (map.globalist.propertyMap.ContainsKey(name) == false)
            {
                throw new Exception("Unable to find property named: " + name);
            }
            
            Property_Prototype proto = map.globalist.propertyMap.lookup(name);

            //Some, but not many, properties can be added multiply. Stackable defaults to false
            if (proto.stackStyle != Property_Prototype.stackStyleEnum.NONE)
            {
                foreach (Property p in location.properties)
                {
                    if (p.proto.name != name) { continue; }
                    if (proto.stackStyle == Property_Prototype.stackStyleEnum.TO_MAX_CHARGE)
                    {
                        //Found matching name. Set timer to whatever's largest
                        p.charge = Math.Max(p.charge, proto.baseCharge);
                        return p;
                    }else if (proto.stackStyle == Property_Prototype.stackStyleEnum.ADD_CHARGE)
                    {
                        p.charge += proto.baseCharge;
                        return p;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            Property prop = new Property(proto,location);
            prop.charge = proto.baseCharge;
            location.properties.Add(prop);
            return prop;
        }
    }
}
