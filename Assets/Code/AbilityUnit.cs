using UnityEngine;

namespace Assets.Code
{
    public abstract class AbilityUnit : Ability
    {
        public override void cast(Map map, Unit unit)
        {
            map.overmind.power -= getCost();
            World.log("Cast " + this.ToString() + " " + this.getName());
            turnLastCast = map.turn;
            castInner(map, unit);

            unit.lastTurnActionTaken = map.turn;
            if (map.param.useAwareness == 1)
            {
                map.overmind.increasePanicFromPower(getCost(), this);
            }
        }
    }
}