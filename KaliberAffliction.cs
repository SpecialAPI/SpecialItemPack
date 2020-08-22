using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Dungeonator;
using Gungeon;

namespace SpecialItemPack
{
    class KaliberAffliction : PlayerItem
    {
        public static void Init()
        {
            string itemName = "Kaliber's Gaze";
            string resourceName = "SpecialItemPack/Resources/KaliberAffliction";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<KaliberAffliction>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "I know what you're thinking...";
            string longDesc = "Turns the bearer into a hellish being.\n\nThis eye seems to have lost it's look.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 300f);
            item.consumable = false;
            item.quality = ItemQuality.S;
            item.AddToBlacksmithShop();
            item.AddToCursulaShop();
            item.PlaceItemInAmmonomiconAfterItemById(489);
            KaliberAffliction.EraseVFX = (PickupObjectDatabase.GetById(570) as YellowChamberItem).EraseVFX;
            KaliberAffliction.MarkVFX = Toolbox.GetGunById(761).DefaultModule.projectiles[0].GetComponent<KthuliberProjectileController>().OverheadVFX;
            Game.Items.Rename("spapi:kaliber's_gaze", "spapi:kalibers_gaze");
        }

        public override void Update()
        {
            base.Update();
            if (this.m_pickedUp && this.LastOwner != null)
            {
                if (this.m_isCurrentlyActive)
                {
                    if(this.m_extantEye == null)
                    {
                        GameObject obj = Instantiate(CustomCreepyEyeController.eyePrefab, this.LastOwner.CenterPosition, Quaternion.identity);
                        CustomCreepyEyeController eye = obj.GetComponent<CustomCreepyEyeController>();
                        eye.ChangeScale(0.5f);
                        eye.transform.parent = this.LastOwner.transform;
                        this.m_extantEye = obj;
                    }
                    if(this.LastOwner.healthHaver != null)
                    {
                        this.LastOwner.healthHaver.IsVulnerable = false;
                    }
                    this.LastOwner.SetIsFlying(true, "KaliberAffliction", false, false);
                    this.LastOwner.ToggleRenderer(false, "KaliberAffliction");
                    this.LastOwner.ToggleGunRenderers(false, "KaliberAffliction");
                    this.LastOwner.ToggleHandRenderers(false, "KaliberAffliction");
                    this.LastOwner.IsGunLocked = true;
                    BraveInput input = BraveInput.GetInstanceForPlayer(this.LastOwner.PlayerIDX);
                    if(input != null)
                    {
                        if (this.m_extantBeam == null && this.LastOwner.PlayerHasActiveSynergy("#DEAD_GAZE"))
                        {
                            this.m_extantBeam = this.BeginFiringBeam(Toolbox.GetGunById(689).DefaultModule.GetCurrentProjectile(), this.LastOwner,
                                BraveMathCollege.Atan2Degrees(this.LastOwner.unadjustedAimPoint.XY() - this.m_extantEye.GetComponent<CustomCreepyEyeController>().poopil.GetComponent<tk2dBaseSprite>().WorldCenter),
                                this.m_extantEye.GetComponent<CustomCreepyEyeController>().poopil.GetComponent<tk2dBaseSprite>().WorldCenter);
                            this.m_extantBeam.projectile.baseData.damage *= 0.25f;
                        }
                        if(this.m_extantBeam != null)
                        {
                            this.ContinueFiringBeam(this.m_extantBeam, this.LastOwner,
                                    BraveMathCollege.Atan2Degrees(this.LastOwner.unadjustedAimPoint.XY() - this.m_extantEye.GetComponent<CustomCreepyEyeController>().poopil.GetComponent<tk2dBaseSprite>().WorldCenter),
                                    this.m_extantEye.GetComponent<CustomCreepyEyeController>().poopil.GetComponent<tk2dBaseSprite>().WorldCenter);
                        }
                        if (!input.ActiveActions.GetActionFromType(GungeonActions.GungeonActionType.Shoot).IsPressed)
                        {
                            this.mousePressedLast = false;
                        }
                        if(input.ActiveActions.GetActionFromType(GungeonActions.GungeonActionType.Shoot).IsPressed && !this.mousePressedLast)
                        {
                            this.mousePressedLast = true;
                            RoomHandler room = this.LastOwner.unadjustedAimPoint.XY().GetAbsoluteRoom();
                            if(room != null)
                            {
                                room.ApplyActionToNearbyEnemies(this.LastOwner.unadjustedAimPoint.XY(), 0.5f, this.MarkNearbyEnemies);
                            }
                        }
                    }
                    else
                    {
                        if(this.m_extantBeam != null)
                        {
                            this.CeaseBeam(this.m_extantBeam);
                            this.m_extantBeam = null;
                        }
                        this.mousePressedLast = false;
                    }
                }
                else
                {
                    if (this.m_extantBeam != null)
                    {
                        this.CeaseBeam(this.m_extantBeam);
                        this.m_extantBeam = null;
                    }
                }
            }
            else
            {
                if (this.m_extantBeam != null)
                {
                    this.CeaseBeam(this.m_extantBeam);
                    this.m_extantBeam = null;
                }
            }
            if (this.cooldownRemaining > 0)
            {
                this.cooldownRemaining = Mathf.Max(0, this.cooldownRemaining - BraveTime.DeltaTime);
            }
            if (this.cooldownRemaining <= 0)
            {
                if (this.markedEnemies.Count > 0)
                {
                    this.EatEnemy(this.markedEnemies[0]);
                    this.markedEnemies.RemoveAt(0);
                    this.cooldownRemaining = 5f;
                }
            }
        }

        private BeamController BeginFiringBeam(Projectile projectileToSpawn, PlayerController source, float targetAngle, Vector2? overrideSpawnPoint)
        {
            Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
            GameObject gameObject = SpawnManager.SpawnProjectile(projectileToSpawn.gameObject, vector, Quaternion.identity, true);
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = source;
            BeamController component2 = gameObject.GetComponent<BeamController>();
            component2.Owner = source;
            component2.HitsPlayers = false;
            component2.HitsEnemies = true;
            Vector3 v = BraveMathCollege.DegreesToVector(targetAngle, 1f);
            component2.Direction = v;
            component2.Origin = vector;
            return component2;
        }

        private void ContinueFiringBeam(BeamController beam, PlayerController source, float angle, Vector2? overrideSpawnPoint)
        {
            Vector2 vector = (overrideSpawnPoint == null) ? source.CenterPosition : overrideSpawnPoint.Value;
            beam.Direction = BraveMathCollege.DegreesToVector(angle, 1f);
            beam.Origin = vector;
            beam.LateUpdatePosition(vector);
        }

        private void CeaseBeam(BeamController beam)
        {
            beam.CeaseAttack();
        }

        private void MarkNearbyEnemies(AIActor target, float f)
        {
            if(!this.markedEnemies.Contains(target) && !this.enemiesToEat.Contains(target))
            {
                target.PlayEffectOnActor(KaliberAffliction.MarkVFX, new Vector3(0.0625f, target.specRigidbody.HitboxPixelCollider.UnitDimensions.y, 0f), true, false, true);
                this.markedEnemies.Add(target);
                AkSoundEngine.PostEvent("Play_WPN_kthulu_soul_01", base.gameObject);
            }
        }

        private void EatEnemy(AIActor target)
        {
            if (!target)
            {
                return;
            }
            if (target.behaviorSpeculator)
            {
                target.behaviorSpeculator.Stun(1f, true);
            }
            if (target.knockbackDoer)
            {
                target.knockbackDoer.SetImmobile(true, "KaliberAffliction");
            }
            this.enemiesToEat.Add(target);
            GameObject gameObject = target.PlayEffectOnActor(KaliberAffliction.EraseVFX, new Vector3(0f, -1f, 0f), false, false, false);
            target.StartCoroutine(this.DelayedDestroyEnemy(target, gameObject.GetComponent<tk2dSpriteAnimator>()));
        }

        private IEnumerator DelayedDestroyEnemy(AIActor enemy, tk2dSpriteAnimator vfxAnimator)
        {
            if (vfxAnimator)
            {
                vfxAnimator.sprite.IsPerpendicular = false;
                vfxAnimator.sprite.HeightOffGround = -1f;
            }
            while (enemy && vfxAnimator && vfxAnimator.sprite.GetCurrentSpriteDef().name != "kthuliber_tentacles_010")
            {
                vfxAnimator.sprite.UpdateZDepth();
                yield return null;
            }
            if (vfxAnimator)
            {
                vfxAnimator.sprite.IsPerpendicular = true;
                vfxAnimator.sprite.HeightOffGround = 1.5f;
                vfxAnimator.sprite.UpdateZDepth();
            }
            this.enemiesToEat.Remove(enemy);
            if (enemy)
            {
                if(enemy.healthHaver != null && enemy.healthHaver.IsBoss)
                {
                    enemy.healthHaver.ApplyDamage(100, Vector2.zero, "Erasure", CoreDamageTypes.None, DamageCategory.Unstoppable, true, null, true);
                }
                else
                {
                    enemy.EraseFromExistence(false);
                }
            }
            yield break;
        }

        protected override void DoEffect(PlayerController user)
        {
            base.DoEffect(user);
            GameObject obj = Instantiate(CustomCreepyEyeController.eyePrefab, user.CenterPosition, Quaternion.identity);
            CustomCreepyEyeController eye = obj.GetComponent<CustomCreepyEyeController>();
            eye.ChangeScale(0.5f);
            eye.transform.parent = user.transform;
            this.m_extantEye = obj;
            base.StartCoroutine(ItemBuilder.HandleDuration(this, 15f, user, this.OnFinish));
        }

        protected override void OnPreDrop(PlayerController user)
        {
            if (this.m_isCurrentlyActive)
            {
                this.OnFinish(user);
            }
            base.OnPreDrop(user);
        }

        private void OnFinish(PlayerController user)
        {
            if (user.healthHaver != null)
            {
                user.healthHaver.IsVulnerable = true;
            }
            user.SetIsFlying(false, "KaliberAffliction", false, false);
            user.ToggleRenderer(true, "KaliberAffliction");
            user.ToggleGunRenderers(true, "KaliberAffliction");
            user.ToggleHandRenderers(true, "KaliberAffliction");
            user.IsGunLocked = false;
            Destroy(this.m_extantEye);
        }

        private GameObject m_extantEye;
        private bool mousePressedLast = false;
        public static GameObject EraseVFX;
        public static GameObject MarkVFX;
        private List<AIActor> markedEnemies = new List<AIActor>();
        private List<AIActor> enemiesToEat = new List<AIActor>();
        private float cooldownRemaining;
        private BeamController m_extantBeam;
    }
}