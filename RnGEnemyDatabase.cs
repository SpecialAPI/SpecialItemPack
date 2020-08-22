using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack
{
    public static class RnGEnemyDatabase
    {
        public static string GetRnGEnemyGuid(RnGEnemyDatabase.RnGEnemyType type)
        {
            return EnemyDatabase.GetOrLoadByName(type.ToString()).EnemyGuid;
        }

        public enum RnGEnemyType
        {
            BulletmanTitan_Boss,
            BulletMan_Pirate,
            BulletMan_Fish,
            DynaM80_Guy,
            SnakeGuy,
            Helicopter,
            BulletmanBroccoli,
            CactusGuy,
            Cylinder,
            Bird_Parrot,
            AngryBook_Tablet,
            BulletShotgunMan_West,
            BulletmanTitan,
            Chameleon,
            BabyShelleton,
            LeadMaiden_Fridge,
            Bullat_Gargoyle,
            BulletShotgunMan_Pirate,
            BulletMan_Knight,
            Cylinder_Red,
            BulletMan_Fish_Blue,
            Musketball_Man,
            BulletmanKaliber,
            Candle_Bulletman,
            BulletgalTitan_Boss,
            BulletManWest,
            BulletmanOfficeTie,
            BulletmanOfficePantsuit,
            Bulletman_Mech,
            AngryBook_Necronomicon,
            BulletManVest
        }
    }
}
