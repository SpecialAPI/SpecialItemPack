using System;
using System.Collections;
using Dungeonator;
using SpecialItemPack.ItemAPI;
using MonoMod.RuntimeDetour;
using UnityEngine;
using System.Collections.Generic;

namespace SpecialItemPack
{
    public class PurpleGuonStone : AdvancedPlayerOrbitalItem
    {
        public static void Init()
        {
            string name = "Purple Guon Stone";
            string resourcePath = "SpecialItemPack/Resources/PurpleGuonStone";
            GameObject gameObject = new GameObject(name);
            var item = gameObject.AddComponent<PurpleGuonStone>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject, true);
            string shortDesc = "Just In Time!";
            string longDesc = "Has a chance to teleport the owner away just before they get hit. Also points to secret rooms or chest mimics.\n\nThis Guon Stone has opened it third eye, being able to see the future and seek the trugh. Actually it " +
                "doesn't have any first or second eyes but who cares.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = PickupObject.ItemQuality.C;
            PurpleGuonStone.BuildPrefab();
            PurpleGuonStone.BuildSynergyPrefab();
            item.OrbitalPrefab = PurpleGuonStone.orbitalPrefab;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(270);
            item.HasAdvancedUpgradeSynergy = true;
            item.AdvancedUpgradeSynergy = "#PURPLER_GUON_STONE";
            item.AdvancedUpgradeOrbitalPrefab = PurpleGuonStone.upgradeOrbitalPrefab.gameObject;
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.healthHaver.ModifyDamage += this.BlinkAway;
            player.ImmuneToPits.AddOverride("purple guon");
        }

        private void BlinkAway(HealthHaver healthHaver, HealthHaver.ModifyDamageEventArgs args)
        {
            if (args == EventArgs.Empty)
            {
                return;
            }
            if (UnityEngine.Random.value <= (this.m_advancedSynergyUpgradeActive ? 0.3f : 0.15f))
            {
                args.ModifiedDamage = 0;
                LootEngine.DoDefaultItemPoof(healthHaver.gameActor.sprite.WorldCenter);
                IntVector2? vector = (healthHaver.gameActor as PlayerController).CurrentRoom.GetRandomAvailableCell(new IntVector2?(IntVector2.One * 2), CellTypes.FLOOR | CellTypes.PIT, false, null);
                if(vector != null)
                {
                    Vector2 vector2 = vector.Value.ToVector2();
                    (healthHaver.gameActor as PlayerController).WarpToPoint(vector2);
                    LootEngine.DoDefaultItemPoof(vector2);
                }
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.ModifyDamage -= this.BlinkAway;
            player.ImmuneToPits.RemoveOverride("purple guon");
            return base.Drop(player);
        }

        protected override void Update()
        {
            base.Update();
            if (this.m_extantOrbital != null)
            {
                this.cooldown -= BraveTime.DeltaTime;
                RoomHandler room = null;
                bool found = false;
                foreach(RoomHandler room1 in this.m_owner.CurrentRoom.connectedRooms)
                {
                    if(room1.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET)
                    {
                        found = true;
                        room = room1;
                        break;
                    }
                }
                if (found)
                {
                    if (!this.m_owner.IsInCombat && this.cooldown <= 0f)
                    {
                        GameObject obj = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(385) as Gun).DefaultModule.projectiles[0].gameObject, this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter, Quaternion.Euler(0, 0,
                            BraveMathCollege.Atan2Degrees(room.GetCenterCell().ToVector2() - this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter)));
                        Projectile proj = obj.GetComponent<Projectile>();
                        if (proj != null)
                        {
                            if (this.m_owner != null)
                            {
                                proj.Owner = this.m_owner;
                                proj.Shooter = this.m_owner.specRigidbody;
                            }
                            proj.baseData.damage = 0;
                            proj.damagesWalls = true;
                            PierceProjModifier pierceMod = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
                            pierceMod.penetratesBreakables = true;
                            pierceMod.penetration = 999;
                            pierceMod.preventPenetrationOfActors = false;
                            pierceMod.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
                            proj.CanTransmogrify = false;
                        }
                        this.cooldown = 0.15f;
                    }
                }
                foreach(IPlayerInteractable interactable in this.m_owner.CurrentRoom.GetRoomInteractables())
                {
                    if(interactable is Chest)
                    {
                        if((interactable as Chest).IsMimic)
                        {
                            if (this.cooldown <= 0f)
                            {
                                GameObject obj = SpawnManager.SpawnProjectile((PickupObjectDatabase.GetById(385) as Gun).DefaultModule.projectiles[0].gameObject, this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter, Quaternion.Euler(0, 0,
                                    BraveMathCollege.Atan2Degrees((interactable as Chest).sprite.WorldCenter - this.m_extantOrbital.GetComponent<tk2dSprite>().WorldCenter)));
                                Projectile proj = obj.GetComponent<Projectile>();
                                if (proj != null)
                                {
                                    if (this.m_owner != null)
                                    {
                                        proj.Owner = this.m_owner;
                                        proj.Shooter = this.m_owner.specRigidbody;
                                    }
                                    proj.baseData.damage = 0;
                                    proj.damagesWalls = true;
                                    PierceProjModifier pierceMod = proj.gameObject.GetOrAddComponent<PierceProjModifier>();
                                    pierceMod.penetratesBreakables = true;
                                    pierceMod.penetration = 999;
                                    pierceMod.preventPenetrationOfActors = false;
                                    pierceMod.BeastModeLevel = PierceProjModifier.BeastModeStatus.BEAST_MODE_LEVEL_ONE;
                                    proj.CanTransmogrify = false;
                                }
                                this.cooldown = 0.15f;
                            }
                        }
                    }
                }
            }
            if(this.m_advancedSynergyUpgradeActive != this.SynergyActiveLast)
            {
                if(this.m_owner != null)
                {
                    if (this.m_advancedSynergyUpgradeActive)
                    {
                        this.m_owner.SetIsFlying(true, "purple guon", true, false);
                        this.m_owner.AdditionalCanDodgeRollWhileFlying.AddOverride("purple guon");
                        Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(this.m_owner.sprite);
                        if (outlineMaterial != null)
                        {
                            outlineMaterial.SetColor("_OverrideColor", new Color(99f, 0f, 99f));
                        }
                    }
                    else
                    {
                        this.m_owner.SetIsFlying(false, "purple guon", true, false);
                        this.m_owner.AdditionalCanDodgeRollWhileFlying.RemoveOverride("purple guon");
                        Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(this.m_owner.sprite);
                        if (outlineMaterial != null)
                        {
                            outlineMaterial.SetColor("_OverrideColor", Color.black);
                        }
                    }
                }
                this.SynergyActiveLast = this.m_advancedSynergyUpgradeActive;
            }
        }

        public static void BuildPrefab()
        {
            bool flag = PurpleGuonStone.orbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonOrbitals/PurpleGuonStoneOrbital", null, true);
                gameObject.name = "Purple Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(9, 9));
                PurpleGuonStone.orbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                PurpleGuonStone.orbitalPrefab.shouldRotate = false;
                PurpleGuonStone.orbitalPrefab.orbitRadius = 2.5f;
                PurpleGuonStone.orbitalPrefab.orbitDegreesPerSecond = 90f;
                PurpleGuonStone.orbitalPrefab.orbitDegreesPerSecond = 120f;
                PurpleGuonStone.orbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static void BuildSynergyPrefab()
        {
            bool flag = PurpleGuonStone.upgradeOrbitalPrefab != null;
            bool flag2 = !flag;
            bool flag3 = flag2;
            if (flag3)
            {
                GameObject gameObject = SpriteBuilder.SpriteFromResource("SpecialItemPack/Resources/GuonStrongOrbitals/PurpleGuonStrongOrbital", null, true);
                gameObject.name = "Synergy Purple Guon";
                SpeculativeRigidbody speculativeRigidbody = gameObject.GetComponent<tk2dSprite>().SetUpSpeculativeRigidbody(IntVector2.Zero, new IntVector2(13, 13));
                PurpleGuonStone.upgradeOrbitalPrefab = gameObject.AddComponent<PlayerOrbital>();
                speculativeRigidbody.CollideWithTileMap = false;
                speculativeRigidbody.CollideWithOthers = true;
                speculativeRigidbody.PrimaryPixelCollider.CollisionLayer = CollisionLayer.EnemyBulletBlocker;
                PurpleGuonStone.upgradeOrbitalPrefab.shouldRotate = false;
                PurpleGuonStone.upgradeOrbitalPrefab.orbitRadius = 2.5f;
                PurpleGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 90f;
                PurpleGuonStone.upgradeOrbitalPrefab.orbitDegreesPerSecond = 120f;
                PurpleGuonStone.upgradeOrbitalPrefab.SetOrbitalTier(0);
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
                FakePrefab.MarkAsFakePrefab(gameObject);
                gameObject.SetActive(false);
            }
        }

        public static Hook guonHook;
        public static PlayerOrbital orbitalPrefab;
        public static PlayerOrbital upgradeOrbitalPrefab;
        private float cooldown = 0;
        private bool SynergyActiveLast = false;
    }
}
