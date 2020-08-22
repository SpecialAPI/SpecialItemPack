using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialItemPack.Shapeshifting
{
    abstract class ShapeShiftBehaviour : BraveBehaviour
    {
        public void RemoveShapeshift()
        {
            this.ClearEffect();
            foreach(StatModifier mod in shapeShiftModifiers)
            {
                player.ownerlessStatModifiers.Remove(mod);
            }
            player.stats.RecalculateStats(player, true, false);
            Destroy(this);
        }

        public static ShapeShiftBehaviour AddToTarget(Type toAdd, PlayerController target)
        {
            ShapeShiftBehaviour behav = (ShapeShiftBehaviour)target.gameObject.AddComponent(toAdd);
            behav.player = target;
            foreach (StatModifier mod in behav.shapeShiftModifiers)
            {
                target.ownerlessStatModifiers.Add(mod);
            }
            target.stats.RecalculateStats(target, true, false);
            behav.OnApplied();
            return behav;
        }

        public abstract void OnApplied();

        public abstract void ClearEffect();

        public abstract List<StatModifier> shapeShiftModifiers { get; }

        public PlayerController player;

    }
}
