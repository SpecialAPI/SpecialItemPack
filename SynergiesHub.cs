using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MonoMod.RuntimeDetour;
using System.Collections;
using UnityEngine;
using Items;


namespace SpecialItemPack
{
    class SynergiesHub
    {
        public static bool PlayerHasPickup(PlayerController p, int pickupID)
        {
            if (p && p.inventory != null && p.inventory.AllGuns != null)
            {
                for (int i = 0; i < p.inventory.AllGuns.Count; i++)
                {
                    if (p.inventory.AllGuns[i].PickupObjectId == pickupID)
                    {
                        return true;
                    }
                }
            }
            if (p)
            {
                for (int j = 0; j < p.activeItems.Count; j++)
                {
                    if (p.activeItems[j].PickupObjectId == pickupID)
                    {
                        return true;
                    }
                }
                for (int k = 0; k < p.passiveItems.Count; k++)
                {
                    if (p.passiveItems[k].PickupObjectId == pickupID)
                    {
                        return true;
                    }
                }
                if (pickupID == GlobalItemIds.Map && p.EverHadMap)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool PlayerHasCompletionItem(PlayerController p)
        {
            if (p)
            {
                for (int i = 0; i < p.passiveItems.Count; i++)
                {
                    if (p.passiveItems[i] is SynergyCompletionItem)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool PlayerHasCompletionGun(PlayerController p)
        {
            if (p)
            {
                for (int i = 0; i < p.inventory.AllGuns.Count; i++)
                {
                    if (p.inventory.AllGuns[i].GetComponent<LichsFavoriteController>() != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool SynergyAvailableHook(Func<AdvancedSynergyEntry, PlayerController, PlayerController, int, bool> orig, AdvancedSynergyEntry self, PlayerController p, PlayerController p2, int additionalID = -1)
        {
            if (self.ActivationStatus == SynergyEntry.SynergyActivation.INACTIVE)
            {
                return false;
            }
            if (self.ActivationStatus == SynergyEntry.SynergyActivation.DEMO)
            {
                return false;
            }
            bool flag = SynergiesHub.PlayerHasCompletionItem(p) || SynergiesHub.PlayerHasCompletionItem(p);
            bool playerHasCompletionGun = SynergiesHub.PlayerHasCompletionGun(p) || SynergiesHub.PlayerHasCompletionGun(p);
            
            if (self.IgnoreLichEyeBullets)
            {
                flag = false;
            }
            int num = 0;
            int num2 = 0;
            for (int i = 0; i < self.MandatoryGunIDs.Count; i++)
            {
                if (SynergiesHub.PlayerHasPickup(p, self.MandatoryGunIDs[i]) || SynergiesHub.PlayerHasPickup(p2, self.MandatoryGunIDs[i]) || self.MandatoryGunIDs[i] == additionalID)
                {
                    num++;
                }
            }
            for (int j = 0; j < self.MandatoryItemIDs.Count; j++)
            {
                if (SynergiesHub.PlayerHasPickup(p, self.MandatoryItemIDs[j]) || SynergiesHub.PlayerHasPickup(p2, self.MandatoryItemIDs[j]) || self.MandatoryItemIDs[j] == additionalID)
                {
                    num2++;
                }
            }
            int num3 = 0;
            int num4 = 0;
            for (int k = 0; k < self.OptionalGunIDs.Count; k++)
            {
                if (SynergiesHub.PlayerHasPickup(p, self.OptionalGunIDs[k]) || SynergiesHub.PlayerHasPickup(p2, self.OptionalGunIDs[k]) || self.OptionalGunIDs[k] == additionalID)
                {
                    num3++;
                }
            }
            for (int l = 0; l < self.OptionalItemIDs.Count; l++)
            {
                if (SynergiesHub.PlayerHasPickup(p, self.OptionalItemIDs[l]) || SynergiesHub.PlayerHasPickup(p2, self.OptionalItemIDs[l]) || self.OptionalItemIDs[l] == additionalID)
                {
                    num4++;
                }
            }
            bool flag2 = self.MandatoryItemIDs.Count > 0 && self.MandatoryGunIDs.Count == 0 && self.OptionalGunIDs.Count > 0 && self.OptionalItemIDs.Count == 0;
            bool flag3 = self.MandatoryGunIDs.Count > 0 && self.MandatoryItemIDs.Count == 0 && self.OptionalItemIDs.Count > 0 && self.OptionalGunIDs.Count == 0;
            if (((self.MandatoryGunIDs.Count > 0 && num > 0) || (flag2 && num3 > 0)) && flag)
            {
                num++;
                num2++;
            }
            if (((self.MandatoryItemIDs.Count > 0 && num2 > 0) || (flag3 && num4 > 0)) && playerHasCompletionGun)
            {
                num++;
                num2++;
            }
            if (num < self.MandatoryGunIDs.Count || num2 < self.MandatoryItemIDs.Count)
            {
                return false;
            }
            int num5 = self.MandatoryItemIDs.Count + self.MandatoryGunIDs.Count + num3 + num4;
            int num6 = self.MandatoryGunIDs.Count + num3;
            int num7 = self.MandatoryItemIDs.Count + num4;
            if (num6 > 0 && (self.MandatoryGunIDs.Count > 0 || flag2 || (self.RequiresAtLeastOneGunAndOneItem && num6 > 0)) && flag)
            {
                num7++;
                num6++;
                num5 += 2;
            }
            if (num7 > 0 && (self.MandatoryItemIDs.Count > 0 || flag3 || (self.RequiresAtLeastOneGunAndOneItem && num7 > 0)) && playerHasCompletionGun)
            {
                num7++;
                num6++;
                num5 += 2;
            }
            if (self.RequiresAtLeastOneGunAndOneItem && self.OptionalGunIDs.Count + self.MandatoryGunIDs.Count > 0 && self.OptionalItemIDs.Count + self.MandatoryItemIDs.Count > 0 && (num6 < 1 || num7 < 1))
            {
                return false;
            }
            int num8 = Mathf.Max(2, self.NumberObjectsRequired);
            return num5 >= num8;
        }

        public static void TransformSpren(Action<SprunButBetter> orig, SprunButBetter self)
        {
            if (self.Owner.HasPickupID(ETGMod.Databases.Items["The Sprun Bullet"].PickupObjectId))
            {
                bool flag2 = self.Owner && self.Owner.CurrentRoom != null && self.Owner.CurrentRoom.IsWinchesterArcadeRoom;
                if (!flag2)
                {
                    bool flag3 = self.Owner && !self.Owner.IsGhost;
                    if (flag3)
                    {
                        foreach (PlayerItem active in self.Owner.activeItems)
                        {
                            if (active is SprenThing)
                            {
                                if (!active.IsCurrentlyActive)
                                {
                                    active.ClearCooldowns();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                orig(self);
            }   
        }

        public static void DeTransformSprenNormal(Action<SprenOrbitalItem> orig, SprenOrbitalItem self)
        {
            orig(self);
        }

        public static void DeTransformSpren(Action<SprunButBetter> orig, SprunButBetter self)
        {
            orig(self);
        }

        public static void TransformSprenNormal(Action<SprenOrbitalItem> orig, SprenOrbitalItem self)
        {
            if (self.Owner.HasPickupID(ETGMod.Databases.Items["The Sprun Bullet"].PickupObjectId))
            {
                if (self.Owner && self.Owner.CurrentRoom != null && self.Owner.CurrentRoom.IsWinchesterArcadeRoom)
                {
                    return;
                }
                if (self.Owner && !self.Owner.IsGhost)
                {
                    foreach (PlayerItem active in self.Owner.activeItems)
                    {
                        if (active is SprenThing)
                        {
                            if (!active.IsCurrentlyActive)
                            {
                                active.ClearCooldowns();
                            }
                        }
                    }
                }
            }
            else
            {
                orig(self);
            }
        }

        public static void AddModdedSynergies()
        {
            if (ETGMod.Databases.Items["Suspiscious Strongbox"] != null)
            {
                SynergiesHub.CreateSynergy("Bowler's Approval", new List<int> { Toolbox.GetModdedItemId("jail_npc_shotgun"), Toolbox.GetModdedItemId("Suspiscious Strongbox") });
            }
            if (ETGMod.Databases.Items["Recyclinder"] != null)
            {
                SynergiesHub.CreateSynergy("Recycle, Collect and Use", new List<int> { Toolbox.GetModdedItemId("Big Chamber"), Toolbox.GetModdedItemId("Recyclinder") });
            }
            if (ETGMod.Databases.Items["Nitroglycylinder"] != null)
            {
                SynergiesHub.CreateSynergy("Relodin' - Explodin'", new List<int> { Toolbox.GetModdedItemId("Big Chamber"), Toolbox.GetModdedItemId("Nitroglycylinder") });
            }
        }

        public static AdvancedSynergyEntry CreateSynergy(string name, List<int> mandatoryIds, List<int> optionalIds = default, bool activeWhenGunsUnequipped = true, List<StatModifier> statModifiers = default, bool ignoreLichsEyeBullets = false,
            int numberObjectsRequired = 2, bool suppressVfx = false, bool requiresAtLeastOneGunAndOneItem = false, List<CustomSynergyType> bonusSynergies = default)
        {
            AdvancedSynergyEntry entry = new AdvancedSynergyEntry();
            string key = "#" + name.ToUpper().Replace(" ", "_").Replace("'", "").Replace(",", "").Replace(".", "");
            entry.NameKey = key;
            SpecialItemModule.Strings.Synergies.Set(key, name);
            foreach (int id in mandatoryIds)
            {
                bool isGun = false;
                if (PickupObjectDatabase.GetById(id) is Gun)
                {
                    isGun = true;
                }
                if (isGun)
                {
                    entry.MandatoryGunIDs.Add(id);
                }
                else
                {
                    entry.MandatoryItemIDs.Add(id);
                }
            }
            if (optionalIds != null)
            {
                foreach (int id in optionalIds)
                {
                    bool isGun = false;
                    if (PickupObjectDatabase.GetById(id) is Gun)
                    {
                        isGun = true;
                    }
                    if (isGun)
                    {
                        entry.OptionalGunIDs.Add(id);
                    }
                    else
                    {
                        entry.OptionalItemIDs.Add(id);
                    }
                }
            }
            entry.ActiveWhenGunUnequipped = activeWhenGunsUnequipped;
            entry.statModifiers = new List<StatModifier>();
            if (statModifiers != null)
            {
                foreach (StatModifier mod in statModifiers)
                {
                    if (mod != null)
                    {
                        entry.statModifiers.Add(mod);
                    }
                }
            }
            entry.IgnoreLichEyeBullets = ignoreLichsEyeBullets;
            entry.NumberObjectsRequired = numberObjectsRequired;
            entry.SuppressVFX = suppressVfx;
            entry.RequiresAtLeastOneGunAndOneItem = requiresAtLeastOneGunAndOneItem;
            entry.bonusSynergies = new List<CustomSynergyType>();
            if (bonusSynergies != null)
            {
                foreach (CustomSynergyType type in bonusSynergies)
                {
                    entry.bonusSynergies.Add(type);
                }
            }
            synergies.Add(entry);
            GameManager.Instance.SynergyManager.synergies = GameManager.Instance.SynergyManager.synergies.Concat(new AdvancedSynergyEntry[] { entry }).ToArray();
            return entry;
        }

        public static void Init()
        {
            SynergiesHub.SetupSynergies();
            Hook synergyAvailableHook = new Hook(
                 typeof(AdvancedSynergyEntry).GetMethod("SynergyIsAvailable", BindingFlags.Public | BindingFlags.Instance),
                typeof(SynergiesHub).GetMethod("SynergyAvailableHook")
            );
            Hook transformationHook = new Hook(
                typeof(SprenOrbitalItem).GetMethod("TransformSpren", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(SynergiesHub).GetMethod("TransformSprenNormal")
            );
            Hook deTransformationHook = new Hook(
                    typeof(SprenOrbitalItem).GetMethod("DetransformSpren", BindingFlags.NonPublic | BindingFlags.Instance),
                    typeof(SynergiesHub).GetMethod("DeTransformSprenNormal")
            );
        }

        public static void SetupSynergies()
        {
            SynergiesHub.CreateSynergy("boring", new List<int> { Toolbox.GetModdedItemId("Synergracing Bullets"), 815 });
            SynergiesHub.CreateSynergy("Double Double Sprun", new List<int> { Toolbox.GetModdedItemId("The Sprun Bullet"), 578 });
            SynergiesHub.CreateSynergy("2-Hit Obliterator", new List<int> { Toolbox.GetModdedItemId("Glass Heart (SpecialItemPack)") }, new List<int> { 421, 422, 423, 424, 425, 364, 164 }, statModifiers: new List<StatModifier>
                {
                    Toolbox.SetupStatModifier(PlayerStats.StatType.Accuracy, 0.75f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.Damage, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.Curse, -0.5f, StatModifier.ModifyMethod.ADDITIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.Coolness, 0.5f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.KnockbackMultiplier, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.MoneyMultiplierFromEnemies, 1.125f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.MovementSpeed, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.PlayerBulletScale, 1.15f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.RateOfFire, 1.25f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.ReloadSpeed, 0.75f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.ShadowBulletChance, 0.5f, StatModifier.ModifyMethod.ADDITIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.ExtremeShadowBulletChance, 0.5f, StatModifier.ModifyMethod.ADDITIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.GlobalPriceMultiplier, 0.925f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.ProjectileSpeed, 1.1f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.ThrownGunDamage, 1.5f, StatModifier.ModifyMethod.MULTIPLICATIVE)
                });
            SynergiesHub.CreateSynergy("Hi, Tech", new List<int> { Toolbox.GetModdedItemId("Quantum Bullets") }, new List<int> { 273, 228, 661 });
            SynergiesHub.CreateSynergy("Dumber Dumb Bullets", new List<int> { Toolbox.GetModdedItemId("Alive Bullets"), 241 });
            SynergiesHub.CreateSynergy("Literally", new List<int> { ETGMod.Databases.Items["hot_dog"].PickupObjectId, 492 });
            SynergiesHub.CreateSynergy("MORE RAINBOW?", new List<int> { Toolbox.GetModdedItemId("jail_npc_shotgun") }, new List<int> { 433, 569, 100 });
            SynergiesHub.CreateSynergy("No Time To Break", new List<int> { Toolbox.GetModdedItemId("Diamond Guon Stone") }, new List<int> { Toolbox.GetModdedItemId("Glass Bullets (SpecialItemPack)"), Toolbox.GetModdedItemId("Glass Chamber (SpecialItemPack)") });
            SynergiesHub.CreateSynergy("Liches No More", new List<int> { Toolbox.GetModdedItemId("lich_slayer") }, new List<int> { 281, 271 });
            SynergiesHub.CreateSynergy("Lich's Full Hand", new List<int> { Toolbox.GetModdedItemId("lich_slayer"), 213 }, statModifiers: new List<StatModifier>
                {
                    new StatModifier
                    {
                        amount = 1,
                        modifyType = StatModifier.ModifyMethod.ADDITIVE,
                        statToBoost = PlayerStats.StatType.Curse
                    },
                    new StatModifier
                    {
                        amount = 0.5f,
                        modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE,
                        statToBoost = PlayerStats.StatType.Accuracy
                    },
                    new StatModifier
                    {
                        amount = 0.15f,
                        modifyType = StatModifier.ModifyMethod.ADDITIVE,
                        statToBoost = PlayerStats.StatType.Damage
                    },
                    new StatModifier
                    {
                        amount = 0.75f,
                        modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE,
                        statToBoost = PlayerStats.StatType.AmmoCapacityMultiplier
                    }
                }, activeWhenGunsUnequipped: false);
            SynergiesHub.CreateSynergy("Sixther Chamber", new List<int> { Toolbox.GetModdedItemId("Big Chamber"), 407 });
            SynergiesHub.CreateSynergy("Yellower Chamber", new List<int> { Toolbox.GetModdedItemId("Big Chamber"), 570 });
            SynergiesHub.CreateSynergy("Oiler Cylinder", new List<int> { Toolbox.GetModdedItemId("Big Chamber"), 165 });
            SynergiesHub.CreateSynergy("The Smart Companions", new List<int> { Toolbox.GetModdedItemId("Lock-On Bullets"), 527, 529 });
            SynergiesHub.CreateSynergy("Be Serious", new List<int> { Toolbox.GetModdedItemId("Alive Bullets") }, new List<int> { 284, Toolbox.GetModdedItemId("Lock-On Bullets") });
            SynergiesHub.CreateSynergy("boring", new List<int> { Toolbox.GetModdedItemId("Synergracing Bullets"), Toolbox.GetModdedItemId("lichs_favorite") }).MandatoryGunIDs =
                new List<int> { Toolbox.GetModdedItemId("Synergracing Bullets"), Toolbox.GetModdedItemId("lichs_favorite") };
            SynergiesHub.CreateSynergy("Gungeon Wizardry School", new List<int> { Toolbox.GetModdedItemId("Gunjurer Ring"), 395 });
            SynergiesHub.CreateSynergy("Perfected It", new List<int> { Toolbox.GetModdedItemId("Penetrating Bullets"), 172 }, statModifiers: new List<StatModifier> { Toolbox.SetupStatModifier(PlayerStats.StatType.Damage, 0.25f,
                StatModifier.ModifyMethod.ADDITIVE) });
            SynergiesHub.CreateSynergy("Snipin' Bros", new List<int> { Toolbox.GetModdedItemId("Invisibullets") }, new List<int> { 102, 273 }, true,
                new List<StatModifier> { Toolbox.SetupStatModifier(PlayerStats.StatType.Accuracy, 0f, StatModifier.ModifyMethod.MULTIPLICATIVE) });
            SynergiesHub.CreateSynergy("Finding New Uses", new List<int> { Toolbox.GetModdedItemId("Alive Bullets"), Toolbox.GetModdedItemId("Big Chamber") });
            SynergiesHub.CreateSynergy("2 Compasses, One Exit", new List<int> { Toolbox.GetModdedItemId("Gungeon Compass"), 209 });
            SynergiesHub.CreateSynergy("Gunderstorm", new List<int> { Toolbox.GetModdedItemId("eye") }, new List<int> { 298, 156, 330, 153, 13 });
            SynergiesHub.CreateSynergy(">=(", new List<int> { Toolbox.GetModdedItemId("Raging Bullets") }, new List<int> { 323, 524 });
            SynergiesHub.CreateSynergy("Big Gun Shotgun-Gun", new List<int> { Toolbox.GetModdedItemId("big_gun"), 601 });
            SynergiesHub.CreateSynergy("A Balanced Diet", new List<int> { }, new List<int> { 369, 478, 445, 197, 510, Toolbox.GetModdedItemId("hot_dog"), 291, 485, 110, 258, 637 }, true,
                new List<StatModifier> { Toolbox.SetupStatModifier(PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE) });
            SynergiesHub.CreateSynergy("Clover's Flowers", new List<int> { Toolbox.GetModdedItemId("Ammo Flower"), 289 });
            SynergiesHub.CreateSynergy("Our Goals Reached", new List<int> { Toolbox.GetModdedItemId("Pastkiller's Plan") }, new List<int> { 491, 492, 493, 494, 572, 573 });
            SynergiesHub.CreateSynergy("?Lucky Dice", new List<int> { Toolbox.GetModdedItemId("(Un)Lucky Dice"), 289 });
            SynergiesHub.CreateSynergy("Gen 2", new List<int> { Toolbox.GetModdedItemId("Pokebullet"), 110 });
            SynergiesHub.CreateSynergy("Chicken Friends", new List<int> { Toolbox.GetModdedItemId("Ring of Live Ammo"), 572 });
            SynergiesHub.CreateSynergy("Elemental Guns... And Bullets", new List<int> { Toolbox.GetModdedItemId("sequencer"), 295, 204, 298 });
            SynergiesHub.CreateSynergy("Wonda-wonder... Wonder-wonda!!!", new List<int> { Toolbox.GetModdedItemId("Scroll of Wonder") }, new List<int> { 115, 396, 397, 398, 399, 400, 465, 633, 666, 137, 281, 632 });
            SynergiesHub.CreateSynergy("Perfecter Charge", new List<int> { Toolbox.GetModdedItemId("Car Battery") }, new List<int> { 298, 318, 153 });
            SynergiesHub.CreateSynergy("Ugly By Nature", new List<int> { Toolbox.GetModdedItemId("ugly_gun") }, new List<int> { 311, 453, 454, 607 });
            SynergiesHub.CreateSynergy("Forbidden Ammolet of Confusion", new List<int> { Toolbox.GetModdedItemId("Crimstone Ammolet"), 465 });
            SynergiesHub.CreateSynergy("Forbidden Ammolet of Resistance", new List<int> { Toolbox.GetModdedItemId("Ebonstone Ammolet"), 255 });
            SynergiesHub.CreateSynergy("Key of Battle", new List<int> { Toolbox.GetModdedItemId("Key of Chaos") }, new List<int> { 95, 166 });
            SynergiesHub.CreateSynergy("There is only chaos.", new List<int> { Toolbox.GetModdedItemId("Key of Chaos") }, new List<int> { 325, 569 });
            SynergiesHub.CreateSynergy("Arcane Gunflower", new List<int> { Toolbox.GetModdedItemId("Ammo Flower"), 462 });
            SynergiesHub.CreateSynergy("Magneto-Lotus", new List<int> { Toolbox.GetModdedItemId("Ammo Flower"), 536 });
            SynergiesHub.CreateSynergy("Ammo Cloak", new List<int> { Toolbox.GetModdedItemId("Ammo Flower"), 433 });
            SynergiesHub.CreateSynergy("Firework Master", new List<int> { Toolbox.GetModdedItemId("mk1"), Toolbox.GetModdedItemId("Fireworks") });
            SynergiesHub.CreateSynergy("Deadly Surprise", new List<int> { Toolbox.GetModdedItemId("mimigun") }, new List<int> { 293, 294, 664 });
            SynergiesHub.CreateSynergy("Brothers In Arms", new List<int> { 580 }, new List<int> { Toolbox.GetModdedItemId("junk1_sword"), Toolbox.GetModdedItemId("junken") });
            SynergiesHub.CreateSynergy("I'm Helping!", new List<int> { Toolbox.GetModdedItemId("Armor-Heart Friendship") }, new List<int> { 164, 450 });
            SynergiesHub.CreateSynergy("You lied, I believed.", new List<int> { Toolbox.GetModdedItemId("Stone Junk"), 148 });
            SynergiesHub.CreateSynergy("Will you sacrifice it?", new List<int> { Toolbox.GetModdedItemId("Stone Junk"), 641 });
            SynergiesHub.CreateSynergy("Can't Talk, But Still Fun.", new List<int> { Toolbox.GetModdedItemId("Stone Junk"), 580 });
            SynergiesHub.CreateSynergy("Prosthetic Heart", new List<int> { Toolbox.GetModdedItemId("Shell'tan's Heart"), 116 });
            SynergiesHub.CreateSynergy("Problems Require Solutions", new List<int> { Toolbox.GetModdedItemId("Relode-Lode"), Toolbox.GetModdedItemId("How To Throw Guns") }, statModifiers: new List<StatModifier>
                {
                    Toolbox.SetupStatModifier(PlayerStats.StatType.ReloadSpeed, 0.75f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                    Toolbox.SetupStatModifier(PlayerStats.StatType.RateOfFire, 1.25f, StatModifier.ModifyMethod.MULTIPLICATIVE),
                });
            SynergiesHub.CreateSynergy("Shield Are Cool!", new List<int> { Toolbox.GetModdedItemId("Shield of Gunkh") }, new List<int> { Toolbox.GetModdedItemId("aegis"), 380 }, false);
            SynergiesHub.CreateSynergy("JW2", new List<int> { Toolbox.GetModdedItemId("Junk Armor"), 545 });
            SynergiesHub.CreateSynergy("Shield Bros.", new List<int> { Toolbox.GetModdedItemId("aegis"), 380 });
            SynergiesHub.CreateSynergy("Dynamo-Machine", new List<int> { Toolbox.GetModdedItemId("Tesla Coil (SpecialItemPack)"), 13 });
            SynergiesHub.CreateSynergy("Giant's Playtoyllets", new List<int> { Toolbox.GetModdedItemId("Flat Bullets"), Toolbox.GetModdedItemId("Straight Bullets") });
            SynergiesHub.CreateSynergy("2, Repeat It", new List<int> { Toolbox.GetModdedItemId("Peashooter Seeds"), 197 });
            SynergiesHub.CreateSynergy("Two Trees Are Better Than One", new List<int> { Toolbox.GetModdedItemId("Mahoguny Sapling"), 339 });
            SynergiesHub.CreateSynergy("I Planted You!!!", new List<int> { Toolbox.GetModdedItemId("Mahoguny Sapling"), 122 });
            SynergiesHub.CreateSynergy("I'm The Boss Here.", new List<int> { Toolbox.GetModdedItemId("Minigun Stand"), 84 });
            SynergiesHub.CreateSynergy("Gunthrowing Expert", new List<int> { Toolbox.GetModdedItemId("Muscle"), Toolbox.GetModdedItemId("How To Throw Guns") }, statModifiers:
               new List<StatModifier> { Toolbox.SetupStatModifier(PlayerStats.StatType.ThrownGunDamage, 2, StatModifier.ModifyMethod.MULTIPLICATIVE) });
            SynergiesHub.CreateSynergy("Woodener Guon Stone", new List<int> { Toolbox.GetModdedItemId("Wooden Guon Stone (pavlov.andrei.d)") }, new List<int> { 286, 158, 127 });
            SynergiesHub.CreateSynergy("Diamonder Guon Stone", new List<int> { Toolbox.GetModdedItemId("Diamond Guon Stone") }, new List<int> { 286, 158, 199 });
            SynergiesHub.CreateSynergy("Magenter Guon Stone", new List<int> { Toolbox.GetModdedItemId("Magenta Guon Stone") }, new List<int> { 286, 158, 595 });
            SynergiesHub.CreateSynergy("Darker Green Guon Stone", new List<int> { Toolbox.GetModdedItemId("Dark Green Guon Stone") }, new List<int> { 286, 158, Toolbox.GetModdedItemId("Ammo Flower") });
            SynergiesHub.CreateSynergy("Purpler Guon Stone", new List<int> { Toolbox.GetModdedItemId("Purple Guon Stone") }, new List<int> { 286, 158, 190 });
            SynergiesHub.CreateSynergy("Blacker Guon Stone", new List<int> { Toolbox.GetModdedItemId("Black Guon Stone") }, new List<int> { 286, 158, 489 });
            SynergiesHub.CreateSynergy("Polarizinger Guon Stone", new List<int> { Toolbox.GetModdedItemId("Polarizing Guon Stone") }, new List<int> { 286, 158, 540 });
            SynergiesHub.CreateSynergy("Philosopher's Great Guon Stone", new List<int> { Toolbox.GetModdedItemId("Philosopher's Guon Stone") }, new List<int> { 286, 158, 145 });
            SynergiesHub.CreateSynergy("I've Always Been The Bullet.", new List<int> { Toolbox.GetModdedItemId("Gungeoneer Bullet"), Toolbox.GetModdedItemId("The Sprun Bullet") });
            SynergiesHub.CreateSynergy("The Living Ones", new List<int> { Toolbox.GetModdedItemId("Gungeoneer Bullet") }, new List<int> { 599, 338, 598, 566 });
            SynergiesHub.CreateSynergy("Dead Gaze", new List<int> { Toolbox.GetModdedItemId("Kaliber's Gaze") }, new List<int> { 570, 569, 631 });
            SynergiesHub.CreateSynergy("Grim Superreaper", new List<int> { Toolbox.GetModdedItemId("Crown of the Jammed"), Toolbox.GetModdedItemId("jamm_scythe") });
            SynergiesHub.CreateSynergy("Mine Too!", new List<int> { Toolbox.GetModdedItemId("jamm_scythe"), 365 });
            SynergiesHub.CreateSynergy("True King", new List<int> { Toolbox.GetModdedItemId("Round King (SpecialItemPack)") }, new List<int> { 551, 214, Toolbox.GetModdedItemId("Crown of the Jammed") });
            SynergiesHub.CreateSynergy("Swordgeon Awaits, %PLAYER_NICK", new List<int> { Toolbox.GetModdedItemId("Ocarina of Time") }, new List<int> { 572, 506, Toolbox.GetModdedItemId("Ring of Live Ammo") });
            SynergiesHub.CreateSynergy("The Success and The Failure", new List<int> { Toolbox.GetModdedItemId("hd"), Toolbox.GetModdedItemId("ugly_gun") });
            SynergiesHub.CreateSynergy("Low-Quality Help", new List<int> { Toolbox.GetModdedItemId("hd"), 38 });
            SynergiesHub.CreateSynergy("Gundead Geometry", new List<int> { }, new List<int> { Toolbox.GetModdedItemId("decagun"), 385, 175, 595 }, false, new List<StatModifier> 
            {
                Toolbox.SetupStatModifier(PlayerStats.StatType.Damage, 0.25f, StatModifier.ModifyMethod.ADDITIVE, false),
                Toolbox.SetupStatModifier(PlayerStats.StatType.AmmoCapacityMultiplier, 0.25f, StatModifier.ModifyMethod.ADDITIVE, false),
                Toolbox.SetupStatModifier(PlayerStats.StatType.Accuracy, -0.25f, StatModifier.ModifyMethod.ADDITIVE, false),
                Toolbox.SetupStatModifier(PlayerStats.StatType.RateOfFire, 0.25f, StatModifier.ModifyMethod.ADDITIVE, false),
                Toolbox.SetupStatModifier(PlayerStats.StatType.ReloadSpeed, -0.25f, StatModifier.ModifyMethod.ADDITIVE, false),
            });
            SynergiesHub.CreateSynergy("\"the synergy\"", new List<int> { Toolbox.GetModdedItemId("Green Chamber"), 570 });
            SynergiesHub.CreateSynergy("Hidden Tech Super Chaos", new List<int> { Toolbox.GetModdedItemId("Table Tech Chaos") }, new List<int> { 325, 569 });
            SynergiesHub.CreateSynergy("King Bullat Shooter", new List<int> { Toolbox.GetModdedItemId("batlauncher") }, new List<int> { 532, 214, 551 });
        }

        public static List<AdvancedSynergyEntry> synergies = new List<AdvancedSynergyEntry>();
    }
}
