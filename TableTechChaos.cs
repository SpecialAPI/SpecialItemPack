using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using System.Collections;
using Dungeonator;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace SpecialItemPack
{
    class TableTechChaos : PassiveItem
    {
        public TableTechChaos()
        {
            TableFlipItem ttrage = PickupObjectDatabase.GetById(399) as TableFlipItem;
            TableFlipItem ttsight = PickupObjectDatabase.GetById(396) as TableFlipItem;
            TableFlipItem ttrocket = PickupObjectDatabase.GetById(398) as TableFlipItem;
            TableFlipItem ttheat = PickupObjectDatabase.GetById(666) as TableFlipItem;
            TableFlipItem ttblank = PickupObjectDatabase.GetById(400) as TableFlipItem;
            TableFlipItem ttshotgun = PickupObjectDatabase.GetById(633) as TableFlipItem;
            this.RageOverheadVFX = ttrage.RageOverheadVFX;
            this.ProjectileExplosionData = ttrocket.ProjectileExplosionData;
            this.CustomAccelerationCurve = ttrocket.CustomAccelerationCurve;
            this.Volley = ttshotgun.Volley;
            this.VolleyOverride = ttshotgun.VolleyOverrides[0];
            this.TableHeatEffect = ttheat.TableHeatEffect;
            this.TableHeatSynergyEffect = ttheat.TableHeatSynergyEffect;
            this.BeeProjectile = ttblank.BeeProjectile;
            this.RadialSlow = ttsight.RadialSlow;
            this.m_volleyElapsed = -1f;
        }

        public static void Init()
        {
            string itemName = "Table Tech Chaos";
            string resourceName = "SpecialItemPack/Resources/TableTechChaos";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<TableTechChaos>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Chaotic Flips";
            string longDesc = "This ancient technique does something when a table is flipped.\n\nThe forgotten chapter of the \"Tabla Sutra.\" You can't understand what's written in it.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.B;
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(407);
            item.AddItemToSynergy(CustomSynergyType.PAPERWORK);
            Hook hook = new Hook(
                typeof(FlippableCover).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
                typeof(TableTechChaos).GetMethod("TableStart")
            );
            SingularityObject = (PickupObjectDatabase.GetById(155) as SpawnObjectPlayerItem).objectToSpawn;
            RedpoofVFX = (PickupObjectDatabase.GetById(436) as BlinkPassiveItem).BlinkpoofVfx;
        }

        public static void TableStart(Action<FlippableCover> orig, FlippableCover self)
        {
            orig(self);
            self.gameObject.AddComponent<AdditionalGildedTableSynergyProcessor>();
        }

        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            player.OnTableFlipped += this.OnFlip;
            player.OnTableFlipCompleted += this.OnFlipCompleted;
        }

        private void OnFlip(FlippableCover obj)
        {
            bool ignoreFlocking = this.GetIgnoreFlocking(obj);
            TableTechChaosEffect effect = TableTechChaosEffect.PickRandomEffect(ignoreFlocking);
            if (effect.IsForFlipCompletion)
            {
                TableTechChaosFlipCompletionIdentifier completionIdentifier = obj.gameObject.AddComponent<TableTechChaosFlipCompletionIdentifier>();
                completionIdentifier.effect = effect;
            }
            else
            {
                this.HandleBlankEffect(effect.identifier, obj);
                this.HandleStunEffect(effect.identifier);
                this.HandleRageEffect(effect.identifier);
                this.HandleVolleyEffect(effect.identifier);
                base.StartCoroutine(this.HandleDelayedEffect(0.25f, this.HandleTableVolley, effect.identifier, obj));
                this.HandleTemporalEffects(effect.identifier);
                this.HandleHeatEffects(effect.identifier, obj);
                this.HandleStealthEffect(effect.identifier, obj);
                base.StartCoroutine(this.HandleDelayedEffect(0.25f, this.HandleBlackHoleEffect, effect.identifier, obj));
                if (effect.identifier == TableTechChaosEffectIdentifier.TIME_SLOW && this.m_owner && this.m_owner.PlayerHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS"))
                {
                    this.RadialSlow.DoRadialSlow(base.Owner.CenterPosition, base.Owner.CurrentRoom);
                }
            }
        }

        private void OnFlipCompleted(FlippableCover obj)
        {
            TableTechChaosFlipCompletionIdentifier completionIdentifier = obj.GetComponent<TableTechChaosFlipCompletionIdentifier>();
            if (completionIdentifier != null)
            {
                TableTechChaosEffect effect = completionIdentifier.effect;
                this.HandleMoneyEffect(effect.identifier, obj);
                this.HandleProjectileEffect(effect.identifier, obj);
                this.HandleTableFlocking(effect.identifier, obj);
                this.HandleMirrorEffect(effect.identifier, obj);
                Destroy(completionIdentifier);
            }
        }

        private void HandleBlackHoleEffect(TableTechChaosEffectIdentifier identifier, FlippableCover obj)
        {
            if(identifier == TableTechChaosEffectIdentifier.BLACK_HOLE)
            {
                Vector3 vector2 = obj.specRigidbody.UnitCenter;
                GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(SingularityObject, vector2, Quaternion.identity);
                tk2dBaseSprite component4 = gameObject2.GetComponent<tk2dBaseSprite>();
                if (component4)
                {
                    component4.PlaceAtPositionByAnchor(vector2, tk2dBaseSprite.Anchor.MiddleCenter);
                }
                DebrisObject debrisObject = LootEngine.DropItemWithoutInstantiating(gameObject2, gameObject2.transform.position, Vector2.zero, 0f, false, false, true, false);
                if (gameObject2.GetComponent<BlackHoleDoer>())
                {
                    debrisObject.PreventFallingInPits = true;
                    debrisObject.PreventAbsorption = true;
                }
                debrisObject.IsAccurateDebris = true;
                debrisObject.Priority = EphemeralObject.EphemeralPriority.Critical;
                debrisObject.bounceCount = 0;
                obj.DestroyCover();
            }
        }

        private IEnumerator HandleDelayedEffect(float delayTime, Action<TableTechChaosEffectIdentifier, FlippableCover> effect, TableTechChaosEffectIdentifier identifier, FlippableCover table)
        {
            yield return new WaitForSeconds(delayTime);
            effect(identifier, table);
            yield break;
        }

        private void HandleTableVolley(TableTechChaosEffectIdentifier identifier, FlippableCover table)
        {
            if (identifier == TableTechChaosEffectIdentifier.TABLE_VOLLEY)
            {
                IntVector2 intVector2FromDirection = DungeonData.GetIntVector2FromDirection(table.DirectionFlipped);
                ProjectileVolleyData sourceVolley = this.Volley;
                float d = 1f;
                if (this.m_owner && this.m_owner.PlayerHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS"))
                {
                    sourceVolley = this.VolleyOverride;
                    d = 2f;
                }
                VolleyUtility.FireVolley(sourceVolley, table.sprite.WorldCenter + intVector2FromDirection.ToVector2() * d, intVector2FromDirection.ToVector2(), this.m_owner, false);
            }
        }

        private void HandleTableFlocking(TableTechChaosEffectIdentifier identifier, FlippableCover table)
        {
            if (identifier == TableTechChaosEffectIdentifier.FLOCKING)
            {
                RoomHandler currentRoom = base.Owner.CurrentRoom;
                ReadOnlyCollection<IPlayerInteractable> roomInteractables = currentRoom.GetRoomInteractables();
                for (int i = 0; i < roomInteractables.Count; i++)
                {
                    if (currentRoom.IsRegistered(roomInteractables[i]))
                    {
                        FlippableCover flippableCover = roomInteractables[i] as FlippableCover;
                        if (flippableCover != null && !flippableCover.IsFlipped && !flippableCover.IsGilded)
                        {
                            if (flippableCover.flipStyle == FlippableCover.FlipStyle.ANY)
                            {
                                flippableCover.ForceSetFlipper(base.Owner);
                                flippableCover.Flip(table.DirectionFlipped);
                            }
                            else if (flippableCover.flipStyle == FlippableCover.FlipStyle.ONLY_FLIPS_LEFT_RIGHT)
                            {
                                if (table.DirectionFlipped == DungeonData.Direction.NORTH || table.DirectionFlipped == DungeonData.Direction.SOUTH)
                                {
                                    flippableCover.ForceSetFlipper(base.Owner);
                                    flippableCover.Flip((UnityEngine.Random.value <= 0.5f) ? DungeonData.Direction.WEST : DungeonData.Direction.EAST);
                                }
                                else
                                {
                                    flippableCover.ForceSetFlipper(base.Owner);
                                    flippableCover.Flip(table.DirectionFlipped);
                                }
                            }
                            else if (flippableCover.flipStyle == FlippableCover.FlipStyle.ONLY_FLIPS_UP_DOWN)
                            {
                                if (table.DirectionFlipped == DungeonData.Direction.EAST || table.DirectionFlipped == DungeonData.Direction.WEST)
                                {
                                    flippableCover.ForceSetFlipper(base.Owner);
                                    flippableCover.Flip((UnityEngine.Random.value <= 0.5f) ? DungeonData.Direction.SOUTH : DungeonData.Direction.NORTH);
                                }
                                else
                                {
                                    flippableCover.ForceSetFlipper(base.Owner);
                                    flippableCover.Flip(table.DirectionFlipped);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HandleMirrorEffect(TableTechChaosEffectIdentifier identifier, FlippableCover obj)
        {
            if(identifier == TableTechChaosEffectIdentifier.MIRROR)
            {
                AkSoundEngine.PostEvent("Play_WPN_kthulu_soul_01", obj.gameObject);
                obj.sprite.usesOverrideMaterial = true;
                tk2dSprite tk2dSprite = obj.sprite as tk2dSprite;
                tk2dSprite.GenerateUV2 = true;
                Material material = Instantiate<Material>(obj.sprite.renderer.material);
                material.DisableKeyword("TINTING_OFF");
                material.EnableKeyword("TINTING_ON");
                material.SetColor("_OverrideColor", new Color(0f, 1f, 1f));
                material.DisableKeyword("EMISSIVE_OFF");
                material.EnableKeyword("EMISSIVE_ON");
                material.SetFloat("_EmissivePower", 1.75f);
                material.SetFloat("_EmissiveColorPower", 1f);
                obj.sprite.renderer.material = material;
                Shader shader = Shader.Find("Brave/ItemSpecific/MetalSkinLayerShader");
                MeshRenderer component = obj.sprite.GetComponent<MeshRenderer>();
                Material[] sharedMaterials = component.sharedMaterials;
                for (int i = 0; i < sharedMaterials.Length; i++)
                {
                    if (sharedMaterials[i].shader == shader)
                    {
                        return;
                    }
                }
                Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
                Material material2 = new Material(shader);
                material2.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
                sharedMaterials[sharedMaterials.Length - 1] = material2;
                component.sharedMaterials = sharedMaterials;
                tk2dSprite.ForceBuild();
                obj.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(
                    delegate (SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)
                    {
                        if(otherRigidbody.projectile != null && !(otherRigidbody.projectile.Owner is PlayerController) && base.Owner != null)
                        {
                            PassiveReflectItem.ReflectBullet(otherRigidbody.projectile, true, base.Owner, 10f, 1f, 1f, 0f);
                            otherRigidbody.RegisterSpecificCollisionException(obj.specRigidbody);
                            PhysicsEngine.SkipCollision = true;
                        }
                    }
                );
                if (base.Owner.PlayerHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS"))
                {
                    obj.gameObject.AddComponent<MirrorBreakSynergyProcessor>().Initialize();
                }
            }
        }

        private void HandleProjectileEffect(TableTechChaosEffectIdentifier identifier, FlippableCover table)
        {
            if (identifier == TableTechChaosEffectIdentifier.PROJECTILE)
            {
                GameObject original = (GameObject)ResourceCache.Acquire("Global VFX/VFX_Table_Exhaust");
                Vector2 vector = DungeonData.GetIntVector2FromDirection(table.DirectionFlipped).ToVector2();
                float z = BraveMathCollege.Atan2Degrees(vector);
                Vector3 zero = Vector3.zero;
                switch (table.DirectionFlipped)
                {
                    case DungeonData.Direction.NORTH:
                        zero = Vector3.zero;
                        break;
                    case DungeonData.Direction.EAST:
                        zero = new Vector3(-0.5f, 0.25f, 0f);
                        break;
                    case DungeonData.Direction.SOUTH:
                        zero = new Vector3(0f, 0.5f, 1f);
                        break;
                    case DungeonData.Direction.WEST:
                        zero = new Vector3(0.5f, 0.25f, 0f);
                        break;
                }
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original, table.specRigidbody.UnitCenter.ToVector3ZisY(0f) + zero, Quaternion.Euler(0f, 0f, z));
                gameObject.transform.parent = table.specRigidbody.transform;
                Projectile projectile = table.specRigidbody.gameObject.AddComponent<Projectile>();
                projectile.Shooter = base.Owner.specRigidbody;
                projectile.Owner = base.Owner;
                projectile.baseData.damage = 30f;
                projectile.baseData.range = 1000f;
                projectile.baseData.speed = 20f;
                projectile.baseData.force = 50f;
                projectile.baseData.UsesCustomAccelerationCurve = true;
                projectile.baseData.AccelerationCurve = this.CustomAccelerationCurve;
                projectile.baseData.CustomAccelerationCurveDuration = 0.8f;
                projectile.shouldRotate = false;
                projectile.Start();
                projectile.SendInDirection(vector, true, true);
                projectile.collidesWithProjectiles = true;
                projectile.projectileHitHealth = 20;
                Action<Projectile> value = delegate (Projectile p)
                {
                    if (table && table.shadowSprite)
                    {
                        table.shadowSprite.renderer.enabled = false;
                    }
                };
                projectile.OnDestruction += value;
                ExplosiveModifier explosiveModifier = projectile.gameObject.AddComponent<ExplosiveModifier>();
                explosiveModifier.explosionData = this.ProjectileExplosionData;
                table.PreventPitFalls = true;
                if (base.Owner && base.Owner.PlayerHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS"))
                {
                    HomingModifier homingModifier = projectile.gameObject.AddComponent<HomingModifier>();
                    homingModifier.AssignProjectile(projectile);
                    homingModifier.HomingRadius = 20f;
                    homingModifier.AngularVelocity = 720f;
                    BounceProjModifier bounceProjModifier = projectile.gameObject.AddComponent<BounceProjModifier>();
                    bounceProjModifier.numberOfBounces = 4;
                    bounceProjModifier.onlyBounceOffTiles = true;
                }
            }
        }

        private void HandleBlankEffect(TableTechChaosEffectIdentifier identifier, FlippableCover table)
        {
            if (identifier == TableTechChaosEffectIdentifier.BLANK)
            {
                GameManager.Instance.StartCoroutine(this.DelayedBlankEffect(table));
            }
        }

        private IEnumerator DelayedBlankEffect(FlippableCover table)
        {
            yield return new WaitForSeconds(0.15f);
            if (this.Owner)
            {
                if (this.Owner.PlayerHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS"))
                {
                    this.m_beeCount = 0;
                    if (table && table.sprite)
                    {
                        this.InternalForceBlank(table.sprite.WorldCenter, 25f, 0.5f, false, true, true, -1f, this.PostProcessTableTechBees);
                    }
                }
                else if (table && table.sprite)
                {
                    this.InternalForceBlank(table.sprite.WorldCenter, 25f, 0.5f, false, true, true, -1f, null);
                }
            }
            yield break;
        }

        private void PostProcessTableTechBees(Projectile target)
        {
            for (int i = 0; i < UnityEngine.Random.Range(1, 5); i++)
            {
                if (target && base.Owner && this.m_beeCount < 49)
                {
                    this.m_beeCount++;
                    GameObject gameObject = SpawnManager.SpawnProjectile(this.BeeProjectile.gameObject, target.transform.position + UnityEngine.Random.insideUnitCircle.ToVector3ZisY(0f), target.transform.rotation, true);
                    Projectile component = gameObject.GetComponent<Projectile>();
                    component.Owner = base.Owner;
                    component.Shooter = base.Owner.specRigidbody;
                    component.collidesWithPlayer = false;
                    component.collidesWithEnemies = true;
                    component.collidesWithProjectiles = false;
                }
            }
        }

        private void InternalForceBlank(Vector2 center, float overrideRadius = 25f, float overrideTimeAtMaxRadius = 0.5f, bool silent = false, bool breaksWalls = true, bool breaksObjects = true, float overrideForce = -1f, Action<Projectile> customCallback = null)
        {
            GameObject silencerVFX = (!silent) ? ((GameObject)BraveResources.Load("Global VFX/BlankVFX", ".prefab")) : null;
            if (!silent)
            {
                AkSoundEngine.PostEvent("Play_OBJ_silenceblank_use_01", base.gameObject);
                AkSoundEngine.PostEvent("Stop_ENM_attack_cancel_01", base.gameObject);
            }
            GameObject gameObject = new GameObject("silencer");
            SilencerInstance silencerInstance = gameObject.AddComponent<SilencerInstance>();
            if (customCallback != null)
            {
                silencerInstance.UsesCustomProjectileCallback = true;
                silencerInstance.OnCustomBlankedProjectile = customCallback;
            }
            silencerInstance.TriggerSilencer(center, 50f, overrideRadius, silencerVFX, (!silent) ? 0.15f : 0f, (!silent) ? 0.2f : 0f, (float)((!silent) ? 50 : 0), (float)((!silent) ? 10 : 0), (!silent) ? ((overrideForce < 0f) ? 140f : overrideForce) : 0f, (float)((!breaksObjects) ? 0 : ((!silent) ? 15 : 5)), overrideTimeAtMaxRadius, base.Owner, breaksWalls, false);
            if (base.Owner)
            {
                base.Owner.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
            }
        }

        private void HandleStunEffect(TableTechChaosEffectIdentifier identifier)
        {
            if (identifier == TableTechChaosEffectIdentifier.STUN)
            {
                List<AIActor> activeEnemies = base.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
                if (activeEnemies != null)
                {
                    for (int i = 0; i < activeEnemies.Count; i++)
                    {
                        this.StunEnemy(activeEnemies[i]);
                    }
                }
            }
        }

        private void StunEnemy(AIActor enemy)
        {
            if (!enemy.healthHaver.IsBoss && enemy && enemy.behaviorSpeculator)
            {
                enemy.ClearPath();
                enemy.behaviorSpeculator.Interrupt();
                enemy.behaviorSpeculator.Stun(3f, true);
            }
        }

        private void HandleMoneyEffect(TableTechChaosEffectIdentifier identifier, FlippableCover sourceCover)
        {
            if (identifier == TableTechChaosEffectIdentifier.MONEY)
            {
                int amountToDrop = UnityEngine.Random.Range(1, 5);
                LootEngine.SpawnCurrency(sourceCover.specRigidbody.UnitCenter, amountToDrop, false);
            }
        }

        private void HandleTemporalEffects(TableTechChaosEffectIdentifier identifier)
        {
            if (identifier == TableTechChaosEffectIdentifier.TIME_SLOW && (!base.Owner || !base.Owner.PlayerHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS")))
            {
                base.Owner.StartCoroutine(this.HandleTimeSlowDuration());
            }
            if (identifier == TableTechChaosEffectIdentifier.INVULNERABILITY)
            {
                base.Owner.healthHaver.TriggerInvulnerabilityPeriod(3.5f);
                AkSoundEngine.PostEvent("Play_OBJ_shrine_accept_01", base.Owner.gameObject);
                base.Owner.PlayEffectOnActor(SoulOrbController.SoulFocusVFX, new Vector3(0f, -0.5f, 0f), true, false, false);
            }
        }

        private IEnumerator HandleTimeSlowDuration()
        {
            float newAdditionalTableFlipSlowTime = GetAdditionalTableFlipSlowTime() + 1.5f;
            SetAdditionalTableFlipSlowTime(newAdditionalTableFlipSlowTime);
            float newAdditionalTableFlipSlowTime2 = Mathf.Min(3f, GetAdditionalTableFlipSlowTime());
            SetAdditionalTableFlipSlowTime(newAdditionalTableFlipSlowTime2);
            if (GetTableFlipTimeIsActive())
            {
                yield break;
            }
            SetTableFlipTimeIsActive(true);
            BraveTime.RegisterTimeScaleMultiplier(0.5f, this.gameObject);
            float elapsed = 0f;
            while (elapsed < GetAdditionalTableFlipSlowTime())
            {
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }
            SetAdditionalTableFlipSlowTime(0f);
            SetTableFlipTimeIsActive(false);
            BraveTime.ClearMultiplier(this.gameObject);
            yield break;
        }

        private bool GetIgnoreFlocking(FlippableCover self)
        {
            if (base.Owner != null && base.Owner.CurrentRoom != null)
            {
                RoomHandler currentRoom = base.Owner.CurrentRoom;
                ReadOnlyCollection<IPlayerInteractable> roomInteractables = currentRoom.GetRoomInteractables();
                for (int i = 0; i < roomInteractables.Count; i++)
                {
                    if (currentRoom.IsRegistered(roomInteractables[i]))
                    {
                        FlippableCover flippableCover = roomInteractables[i] as FlippableCover;
                        if (flippableCover != null && !flippableCover.IsFlipped && !flippableCover.IsGilded && flippableCover != self)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void HandleStealthEffect(TableTechChaosEffectIdentifier identifier, FlippableCover obj)
        {
            if(identifier == TableTechChaosEffectIdentifier.STEALTH)
            {
                PlayerController owner = base.Owner;
                this.BreakStealth(owner);
                owner.OnItemStolen += this.BreakStealthOnSteal;
                owner.ChangeSpecialShaderFlag(1, 1f);
                owner.healthHaver.OnDamaged += this.OnDamaged;
                owner.SetIsStealthed(true, "table tech chaos");
                owner.SetCapableOfStealing(true, "table tech chaos", null);
                GameManager.Instance.StartCoroutine(this.Unstealthy());
                if (base.Owner.PlayerHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS"))
                {
                    float num;
                    RoomHandler room = obj.transform.position.GetAbsoluteRoom();
                    if(room != null)
                    {
                        AIActor aiactor = room.GetNearestEnemy(obj.specRigidbody.UnitCenter, out num, true, true);
                        if(aiactor != null)
                        {
                            aiactor.PlayEffectOnActor(RedpoofVFX, Vector3.zero, false, true, false);
                            aiactor.healthHaver.ApplyDamage(UnityEngine.Random.Range(15, 30), obj.specRigidbody.UnitCenter - aiactor.sprite.WorldCenter, "Hidden Tech Assassin", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, false);
                        }
                    }
                }
            }
        }

        private IEnumerator Unstealthy()
        {
            PlayerController player = base.Owner;
            yield return new WaitForSeconds(0.15f);
            player.OnDidUnstealthyAction += this.BreakStealth;
            yield break;
        }

        private void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            PlayerController owner = base.Owner;
            this.BreakStealth(owner);
        }

        private void BreakStealthOnSteal(PlayerController arg1, ShopItemController arg2)
        {
            this.BreakStealth(arg1);
        }

        private void BreakStealth(PlayerController player)
        {
            player.ChangeSpecialShaderFlag(1, 0f);
            player.OnItemStolen -= this.BreakStealthOnSteal;
            player.SetIsStealthed(false, "table tech chaos");
            player.healthHaver.OnDamaged -= this.OnDamaged;
            player.SetCapableOfStealing(false, "table tech chaos", null);
            player.OnDidUnstealthyAction -= this.BreakStealth;
            AkSoundEngine.PostEvent("Play_ENM_wizardred_appear_01", base.gameObject);
        }

        public static bool GetTableFlipTimeIsActive()
        {
            return (bool)timeSlowActiveInfo.GetValue(null);
        }

        public static void SetTableFlipTimeIsActive(bool value)
        {
            timeSlowActiveInfo.SetValue(null, value);
        }

        public static float GetAdditionalTableFlipSlowTime()
        {
            return (float)timeSlowDurationInfo.GetValue(null);
        }

        public static void SetAdditionalTableFlipSlowTime(float value)
        {
            timeSlowDurationInfo.SetValue(null, value);
        }

        private void HandleRageEffect(TableTechChaosEffectIdentifier identifier)
        {
            if (identifier == TableTechChaosEffectIdentifier.RAGE)
            {
                if (this.m_rageElapsed > 0f)
                {
                    this.m_rageElapsed = 5f;
                    if (base.Owner.HasActiveBonusSynergy(CustomSynergyType.ANGRIER_BULLETS, false))
                    {
                        this.m_rageElapsed *= 3f;
                    }
                    if (this.RageOverheadVFX && this.rageInstanceVFX == null)
                    {
                        this.rageInstanceVFX = base.Owner.PlayEffectOnActor(this.RageOverheadVFX, new Vector3(0f, 1.375f, 0f), true, true, false);
                    }
                }
                else
                {
                    base.Owner.StartCoroutine(this.HandleRageCooldown());
                }
            }
        }

        private IEnumerator HandleRageCooldown()
        {
            this.rageInstanceVFX = null;
            if (this.RageOverheadVFX)
            {
                this.rageInstanceVFX = this.Owner.PlayEffectOnActor(this.RageOverheadVFX, new Vector3(0f, 1.375f, 0f), true, true, false);
            }
            this.m_rageElapsed = 5f;
            if (this.Owner.HasActiveBonusSynergy(CustomSynergyType.ANGRIER_BULLETS, false))
            {
                this.m_rageElapsed *= 3f;
            }
            StatModifier damageStat = new StatModifier();
            damageStat.amount = 2f;
            damageStat.modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE;
            damageStat.statToBoost = PlayerStats.StatType.Damage;
            PlayerController cachedOwner = this.Owner;
            cachedOwner.ownerlessStatModifiers.Add(damageStat);
            cachedOwner.stats.RecalculateStats(cachedOwner, false, false);
            Color rageColor = new Color(0.5f, 0f, 0f, 0.75f);
            while (this.m_rageElapsed > 0f)
            {
                cachedOwner.baseFlatColorOverride = rageColor.WithAlpha(Mathf.Lerp(rageColor.a, 0f, 1f - Mathf.Clamp01(this.m_rageElapsed)));
                if (this.rageInstanceVFX && this.m_rageElapsed < 4f)
                {
                    this.rageInstanceVFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("rage_face_vfx_out", null);
                    this.rageInstanceVFX = null;
                }
                yield return null;
                this.m_rageElapsed -= BraveTime.DeltaTime;
            }
            if (this.rageInstanceVFX)
            {
                this.rageInstanceVFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("rage_face_vfx_out", null);
            }
            cachedOwner.ownerlessStatModifiers.Remove(damageStat);
            cachedOwner.stats.RecalculateStats(cachedOwner, false, false);
            yield break;
        }

        private void HandleVolleyEffect(TableTechChaosEffectIdentifier identifier)
        {
            if (identifier == TableTechChaosEffectIdentifier.VOLLEY)
            {
                if (this.m_volleyElapsed < 0f)
                {
                    base.Owner.StartCoroutine(this.HandleVolleyCooldown());
                }
                else
                {
                    this.m_volleyElapsed = 0f;
                }
            }
        }

        public void ModifyVolley(ProjectileVolleyData volleyToModify)
        {
            int count = volleyToModify.projectiles.Count;
            for (int i = 0; i < count; i++)
            {
                ProjectileModule projectileModule = volleyToModify.projectiles[i];
                float num = 2f * 10f * -1f / 2f;
                for (int j = 0; j < 2; j++)
                {
                    int sourceIndex = i;
                    if (projectileModule.CloneSourceIndex >= 0)
                    {
                        sourceIndex = projectileModule.CloneSourceIndex;
                    }
                    ProjectileModule projectileModule2 = ProjectileModule.CreateClone(projectileModule, false, sourceIndex);
                    float angleFromAim = num + 10f * (float)j;
                    projectileModule2.angleFromAim = angleFromAim;
                    projectileModule2.ignoredForReloadPurposes = true;
                    projectileModule2.ammoCost = 0;
                    volleyToModify.projectiles.Add(projectileModule2);
                }
            }
        }

        private IEnumerator HandleVolleyCooldown()
        {
            this.m_volleyElapsed = 0f;
            PlayerController cachedOwner = this.Owner;
            bool wasFiring = false;
            if (cachedOwner.CurrentGun != null && cachedOwner.CurrentGun.IsFiring)
            {
                cachedOwner.CurrentGun.CeaseAttack(true, null);
                wasFiring = true;
            }
            cachedOwner.stats.AdditionalVolleyModifiers += this.ModifyVolley;
            cachedOwner.stats.RecalculateStats(cachedOwner, false, false);
            if (wasFiring)
            {
                cachedOwner.CurrentGun.Attack(null, null);
                for (int i = 0; i < cachedOwner.CurrentGun.ActiveBeams.Count; i++)
                {
                    if (cachedOwner.CurrentGun.ActiveBeams[i] != null && cachedOwner.CurrentGun.ActiveBeams[i].beam is BasicBeamController)
                    {
                        (cachedOwner.CurrentGun.ActiveBeams[i].beam as BasicBeamController).ForceChargeTimer(10f);
                    }
                }
            }
            while (this.m_volleyElapsed < 2f)
            {
                this.m_volleyElapsed += BraveTime.DeltaTime;
                yield return null;
            }
            bool wasEndFiring = cachedOwner.CurrentGun != null && cachedOwner.CurrentGun.IsFiring;
            if (wasEndFiring)
            {
                cachedOwner.CurrentGun.CeaseAttack(true, null);
            }
            cachedOwner.stats.AdditionalVolleyModifiers -= this.ModifyVolley;
            cachedOwner.stats.RecalculateStats(cachedOwner, false, false);
            if (wasEndFiring)
            {
                cachedOwner.CurrentGun.Attack(null, null);
                for (int j = 0; j < cachedOwner.CurrentGun.ActiveBeams.Count; j++)
                {
                    if (cachedOwner.CurrentGun.ActiveBeams[j] != null && cachedOwner.CurrentGun.ActiveBeams[j].beam is BasicBeamController)
                    {
                        (cachedOwner.CurrentGun.ActiveBeams[j].beam as BasicBeamController).ForceChargeTimer(10f);
                    }
                }
            }
            this.m_volleyElapsed = -1f;
            yield return null;
            yield break;
        }

        private void HandleHeatEffects(TableTechChaosEffectIdentifier identifier, FlippableCover table)
        {
            if (identifier == TableTechChaosEffectIdentifier.HEAT && table)
            {
                table.StartCoroutine(this.HandleHeatEffectsCR(table));
            }
        }

        private IEnumerator HandleHeatEffectsCR(FlippableCover table)
        {
            this.HandleRadialIndicator(table);
            float elapsed = 0f;
            int ct = -1;
            bool hasSynergy = Toolbox.AnyoneHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS", out ct);
            RoomHandler r = table.transform.position.GetAbsoluteRoom();
            Vector3 tableCenter = (!table.sprite) ? table.transform.position : table.sprite.WorldCenter.ToVector3ZisY(0f);
            Action<AIActor, float> AuraAction = delegate (AIActor actor, float dist)
            {
                actor.ApplyEffect((!hasSynergy) ? this.TableHeatEffect : this.TableHeatSynergyEffect, 1f, null);
            };
            float modRadius = (!hasSynergy) ? 5f : 20f;
            while (elapsed < 10f)
            {
                elapsed += BraveTime.DeltaTime;
                r.ApplyActionToNearbyEnemies(tableCenter.XY(), modRadius, AuraAction);
                yield return null;
            }
            this.UnhandleRadialIndicator(table);
            yield break;
        }

        private void HandleRadialIndicator(FlippableCover table)
        {
            if (this.m_radialIndicators == null)
            {
                this.m_radialIndicators = new Dictionary<FlippableCover, HeatIndicatorController>();
            }
            if (!this.m_radialIndicators.ContainsKey(table))
            {
                Vector3 position = (!table.sprite) ? table.transform.position : table.sprite.WorldCenter.ToVector3ZisY(0f);
                this.m_radialIndicators.Add(table, ((GameObject)UnityEngine.Object.Instantiate(ResourceCache.Acquire("Global VFX/HeatIndicator"), position, Quaternion.identity, table.transform)).GetComponent<HeatIndicatorController>());
                int num = -1;
                float currentRadius = (!Toolbox.AnyoneHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS", out num)) ? 5f : 20f;
                this.m_radialIndicators[table].CurrentRadius = currentRadius;
            }
        }

        private void UnhandleRadialIndicator(FlippableCover table)
        {
            if (this.m_radialIndicators.ContainsKey(table))
            {
                HeatIndicatorController heatIndicatorController = this.m_radialIndicators[table];
                heatIndicatorController.EndEffect();
                this.m_radialIndicators.Remove(table);
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnTableFlipped -= this.OnFlip;
            player.OnTableFlipCompleted -= this.OnFlipCompleted;
            return base.Drop(player);
        }

        public GameObject RageOverheadVFX;
        public ExplosionData ProjectileExplosionData;
        public AnimationCurve CustomAccelerationCurve;
        public ProjectileVolleyData Volley;
        public ProjectileVolleyData VolleyOverride;
        public GameActorFireEffect TableHeatEffect;
        public GameActorFireEffect TableHeatSynergyEffect;
        public Projectile BeeProjectile;
        public RadialSlowInterface RadialSlow;
        private int m_beeCount;
        private float m_rageElapsed;
        private GameObject rageInstanceVFX;
        private float m_volleyElapsed;
        private Dictionary<FlippableCover, HeatIndicatorController> m_radialIndicators;
        public static FieldInfo timeSlowActiveInfo = typeof(TableFlipItem).GetField("TableFlipTimeIsActive", BindingFlags.NonPublic | BindingFlags.Static);
        public static FieldInfo timeSlowDurationInfo = typeof(TableFlipItem).GetField("AdditionalTableFlipSlowTime", BindingFlags.NonPublic | BindingFlags.Static);
        public static GameObject SingularityObject;
        public static GameObject RedpoofVFX;

        private enum TableTechChaosEffectIdentifier
        {
            BLANK,
            STUN,
            RAGE,
            VOLLEY,
            TABLE_VOLLEY,
            TIME_SLOW,
            INVULNERABILITY,
            HEAT,
            MONEY,
            PROJECTILE,
            FLOCKING,
            BLACK_HOLE,
            MIRROR,
            STEALTH
        }

        private class TableTechChaosFlipCompletionIdentifier : MonoBehaviour
        {
            public TableTechChaosEffect effect;
        }

        private struct TableTechChaosEffect
        {
            public TableTechChaosEffect(TableTechChaosEffectIdentifier identifier)
            {
                this.identifier = identifier;
            }

            public static TableTechChaosEffect PickRandomEffect(bool ignoreFlocking = false)
            {
                TableTechChaosEffectIdentifier identifier;
                if (ignoreFlocking)
                {
                    int num = UnityEngine.Random.Range(1, 14);
                    switch (num)
                    {
                        case 1:
                            identifier = TableTechChaosEffectIdentifier.BLANK;
                            break;
                        case 2:
                            identifier = TableTechChaosEffectIdentifier.STUN;
                            break;
                        case 3:
                            identifier = TableTechChaosEffectIdentifier.RAGE;
                            break;
                        case 4:
                            identifier = TableTechChaosEffectIdentifier.VOLLEY;
                            break;
                        case 5:
                            identifier = TableTechChaosEffectIdentifier.TABLE_VOLLEY;
                            break;
                        case 6:
                            identifier = TableTechChaosEffectIdentifier.TIME_SLOW;
                            break;
                        case 7:
                            identifier = TableTechChaosEffectIdentifier.HEAT;
                            break;
                        case 8:
                            identifier = TableTechChaosEffectIdentifier.MONEY;
                            break;
                        case 9:
                            identifier = TableTechChaosEffectIdentifier.PROJECTILE;
                            break;
                        case 10:
                            identifier = TableTechChaosEffectIdentifier.INVULNERABILITY;
                            break;
                        case 11:
                            identifier = TableTechChaosEffectIdentifier.BLACK_HOLE;
                            break;
                        case 12:
                            identifier = TableTechChaosEffectIdentifier.MIRROR;
                            break;
                        case 13:
                            identifier = TableTechChaosEffectIdentifier.STEALTH;
                            break;
                        default:
                            identifier = TableTechChaosEffectIdentifier.BLANK;
                            break;
                    }
                }
                else
                {
                    int num = UnityEngine.Random.Range(1, 15);
                    switch (num)
                    {
                        case 1:
                            identifier = TableTechChaosEffectIdentifier.BLANK;
                            break;
                        case 2:
                            identifier = TableTechChaosEffectIdentifier.STUN;
                            break;
                        case 3:
                            identifier = TableTechChaosEffectIdentifier.RAGE;
                            break;
                        case 4:
                            identifier = TableTechChaosEffectIdentifier.VOLLEY;
                            break;
                        case 5:
                            identifier = TableTechChaosEffectIdentifier.TABLE_VOLLEY;
                            break;
                        case 6:
                            identifier = TableTechChaosEffectIdentifier.TIME_SLOW;
                            break;
                        case 7:
                            identifier = TableTechChaosEffectIdentifier.HEAT;
                            break;
                        case 8:
                            identifier = TableTechChaosEffectIdentifier.MONEY;
                            break;
                        case 9:
                            identifier = TableTechChaosEffectIdentifier.PROJECTILE;
                            break;
                        case 10:
                            identifier = TableTechChaosEffectIdentifier.FLOCKING;
                            break;
                        case 11:
                            identifier = TableTechChaosEffectIdentifier.INVULNERABILITY;
                            break;
                        case 12:
                            identifier = TableTechChaosEffectIdentifier.BLACK_HOLE;
                            break;
                        case 13:
                            identifier = TableTechChaosEffectIdentifier.MIRROR;
                            break;
                        case 14:
                            identifier = TableTechChaosEffectIdentifier.STEALTH;
                            break;
                        default:
                            identifier = TableTechChaosEffectIdentifier.BLANK;
                            break;
                    }
                }
                TableTechChaosEffect effect = new TableTechChaosEffect(identifier);
                return effect;
            }

            public bool IsForFlipCompletion
            {
                get
                {
                    return identifier == TableTechChaosEffectIdentifier.MONEY || identifier == TableTechChaosEffectIdentifier.PROJECTILE || identifier == TableTechChaosEffectIdentifier.FLOCKING || identifier == TableTechChaosEffectIdentifier.MIRROR;
                }
            }

            public TableTechChaosEffectIdentifier identifier;
        }

        private class MirrorBreakSynergyProcessor : BraveBehaviour
        {
            public void Initialize()
            {
                base.GetComponentInChildren<MajorBreakable>().OnBreak += this.HandleEnemyDamage;
            }

            private void HandleEnemyDamage()
            {
                AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", base.gameObject);
                RoomHandler room = base.transform.position.GetAbsoluteRoom();
                if(room != null)
                {
                    room.ApplyActionToNearbyEnemies(base.sprite.WorldCenter, 15f, delegate (AIActor aiactor, float f)
                    {
                        aiactor.PlayEffectOnActor(ResourceCache.Acquire("Global VFX/VFX_Curse") as GameObject, Vector3.zero, false, false, false);
                        aiactor.healthHaver.ApplyDamage(20f, Vector2.zero, "Soul Burn", CoreDamageTypes.Void, DamageCategory.Unstoppable, false, null, false);
                    });
                }
            }
        }

        private class AdditionalGildedTableSynergyProcessor : BraveBehaviour
        {
            private void Start()
            {
                RoomHandler absoluteRoom = base.transform.position.GetAbsoluteRoom();
                if (absoluteRoom != null)
                {
                    absoluteRoom.Entered += this.HandleParentRoomEntered;
                    if (GameManager.Instance.BestActivePlayer && absoluteRoom == GameManager.Instance.BestActivePlayer.CurrentRoom)
                    {
                        this.m_hasRoomEnteredProcessed = true;
                    }
                }
            }

            private void HandleParentRoomEntered(PlayerController p)
            {
                if (this.m_hasRoomEnteredProcessed)
                {
                    return;
                }
                this.m_hasRoomEnteredProcessed = true;
                if (p && p.PlayerHasActiveSynergy("#HIDDEN_TECH_SUPER_CHAOS") && UnityEngine.Random.value < 0.15f && (bool)info.GetValue(base.GetComponent<FlippableCover>()) == false)
                {
                    info.SetValue(base.GetComponent<FlippableCover>(), true);
                    base.sprite.usesOverrideMaterial = true;
                    tk2dSprite tk2dSprite = base.sprite as tk2dSprite;
                    tk2dSprite.GenerateUV2 = true;
                    Material material = UnityEngine.Object.Instantiate<Material>(base.sprite.renderer.material);
                    material.DisableKeyword("TINTING_OFF");
                    material.EnableKeyword("TINTING_ON");
                    material.SetColor("_OverrideColor", new Color(1f, 0.77f, 0f));
                    material.DisableKeyword("EMISSIVE_OFF");
                    material.EnableKeyword("EMISSIVE_ON");
                    material.SetFloat("_EmissivePower", 1.75f);
                    material.SetFloat("_EmissiveColorPower", 1f);
                    base.sprite.renderer.material = material;
                    Shader shader = Shader.Find("Brave/ItemSpecific/MetalSkinLayerShader");
                    MeshRenderer component = base.sprite.GetComponent<MeshRenderer>();
                    Material[] sharedMaterials = component.sharedMaterials;
                    for (int i = 0; i < sharedMaterials.Length; i++)
                    {
                        if (sharedMaterials[i].shader == shader)
                        {
                            return;
                        }
                    }
                    Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
                    Material material2 = new Material(shader);
                    material2.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
                    sharedMaterials[sharedMaterials.Length - 1] = material2;
                    component.sharedMaterials = sharedMaterials;
                    tk2dSprite.ForceBuild();
                }
            }

            private bool m_hasRoomEnteredProcessed;
            public static FieldInfo info = typeof(FlippableCover).GetField("m_isGilded", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
