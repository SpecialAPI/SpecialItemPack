using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using Gungeon;
using System.Collections;
using Dungeonator;

namespace SpecialItemPack
{
    class BigChamber : PassiveItem
    {
        public static void Init()
        {
            string itemName = "Big Chamber";
            string resourceName = "SpecialItemPack/Resources/BigChamber";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<BigChamber>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "A Chamber That's Big";
            string longDesc = "A big chamber.\n\nIn comparison with normal gun chambers, this one is bigger.";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "spapi");
            item.quality = ItemQuality.S;
            BigChamber.InitEnemyTypes();
            BigChamber.InitCustomEnemyTypes();
            item.AddToTrorkShop();
            item.AddToBlacksmithShop();
            item.PlaceItemInAmmonomiconAfterItemById(570);
        }

        protected override void Update()
        {
            base.Update();
            if(this.m_pickedUp && this.m_owner != null)
            {
                if(this.m_owner.CurrentGun != null && (this.m_owner.CurrentGun.IsReloading || this.m_owner.CurrentGun.ammo <= 0) && !this.m_owner.CurrentGun.InfiniteAmmo)
                {
                    if (this.m_owner.CurrentRoom != null)
                    {
                        if(this.m_owner.CurrentGun.DefaultModule.ammoType == GameUIAmmoType.AmmoType.CUSTOM && this.m_owner.CurrentGun.DefaultModule.customAmmoType == "pot")
                        {
                            this.SuckPots();
                            this.m_owner.CurrentRoom.ApplyActionToNearbyEnemies(this.m_owner.CenterPosition, 4, new Action<AIActor, float>(this.ProcessEnemy));
                        }
                        else
                        {
                            this.m_owner.CurrentRoom.ApplyActionToNearbyEnemies(this.m_owner.CenterPosition, 4, new Action<AIActor, float>(this.ProcessEnemy));
                        }
                    }
                }
            }
        }

        private void SuckPots()
        {
            List<MinorBreakable> allMinorBreakables = StaticReferenceManager.AllMinorBreakables;
            for (int k = allMinorBreakables.Count - 1; k >= 0; k--)
            {
                MinorBreakable minorBreakable = allMinorBreakables[k];
                if (minorBreakable && minorBreakable.specRigidbody)
                {
                    if (!minorBreakable.IsBroken && minorBreakable.sprite)
                    {
                        Vector2 vector = minorBreakable.sprite.WorldCenter - this.m_owner.sprite.WorldCenter;
                        bool flag = (vector.sqrMagnitude < (6 * 6));
                        if (flag)
                        {
                            GameManager.Instance.Dungeon.StartCoroutine(this.HandlePotSuck(minorBreakable));
                            minorBreakable.Break();
                        }   
                    }
                }
            }
        }

        private void ProcessEnemy(AIActor target, float distance)
        {
            if (target.GetComponent<AliveBullets.DumbEnemyBehavior>() == null)
            {
                if (this.m_owner.CurrentGun.AmmoType == GameUIAmmoType.AmmoType.CUSTOM)
                {
                    if (BigChamber.customTypesEnemies.ContainsKey(this.m_owner.CurrentGun.DefaultModule.customAmmoType))
                    {
                        if (BigChamber.customTypesEnemies[this.m_owner.CurrentGun.DefaultModule.customAmmoType].Contains(target.EnemyGuid))
                        {
                            GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                            target.EraseFromExistence(true);
                        }
                    }
                    else if(this.m_owner.CurrentGun.DefaultModule.customAmmoType != "pot")
                    {
                        if (typesEnemies[GameUIAmmoType.AmmoType.SMALL_BULLET].Contains(target.EnemyGuid))
                        {
                            GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                            target.EraseFromExistence(true);
                        }
                    }
                }
                else
                {

                    if (BigChamber.typesEnemies[this.m_owner.CurrentGun.DefaultModule.ammoType].Contains(target.EnemyGuid))
                    {
                        if (BigChamber.typesEnemies[this.m_owner.CurrentGun.DefaultModule.ammoType].Contains(target.EnemyGuid))
                        {
                            GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                            target.EraseFromExistence(true);
                        }
                        else
                        {
                            if (typesEnemies[GameUIAmmoType.AmmoType.SMALL_BULLET].Contains(target.EnemyGuid))
                            {
                                GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                                target.EraseFromExistence(true);
                            }
                        }
                    }
                }
            }
            else
            {
                if (target.GetComponent<AliveBullets.DumbEnemyBehavior>().usesNewUsesSynergy)
                {
                    if (this.m_owner.CurrentGun.DefaultModule.ammoType == target.GetComponent<AliveBullets.DumbEnemyBehavior>().sourceAmmoType)
                    {
                        if (this.m_owner.CurrentGun.DefaultModule.ammoType == GameUIAmmoType.AmmoType.CUSTOM)
                        {
                            if (target.GetComponent<AliveBullets.DumbEnemyBehavior>().customAmmoType != null)
                            {
                                if (this.m_owner.CurrentGun.DefaultModule.customAmmoType == target.GetComponent<AliveBullets.DumbEnemyBehavior>().customAmmoType)
                                {
                                    GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                                    target.EraseFromExistence(true);
                                }
                            }
                        }
                        else
                        {
                            GameManager.Instance.Dungeon.StartCoroutine(this.HandleEnemySuck(target));
                            target.EraseFromExistence(true);
                        }
                    }
                }
            }
        }

        private IEnumerator HandleEnemySuck(AIActor target)
        {
            Transform copySprite = this.CreateEmptySprite(target);
            if (this.m_owner.PlayerHasActiveSynergy("#RELODIN_-_EXPLODIN"))
            {
                ExplosionData explosionData = new ExplosionData();
                explosionData.CopyFrom(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData);
                explosionData.damageToPlayer = 0f;
                Exploder.Explode(target.sprite.WorldCenter, explosionData, new Vector2());
            }
            if (this.m_owner.PlayerHasActiveSynergy("RECYCLE_COLLECT_AND_USE") && UnityEngine.Random.value <= 0.05f)
            {
                GenericLootTable singleItemRewardTable = GameManager.Instance.RewardManager.CurrentRewardData.SingleItemRewardTable;
                LootEngine.SpawnItem(singleItemRewardTable.SelectByWeight(false), this.m_owner.CenterPosition, Vector2.up, 1f, true, false, false);
            }
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                if (this.m_owner.CurrentGun && copySprite)
                {
                    Vector3 position = this.m_owner.CurrentGun.PrimaryHandAttachPoint.position;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                }
                yield return null;
            }
            if (copySprite)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
            if (this.m_owner.CurrentGun)
            {
                this.m_owner.CurrentGun.GainAmmo(1);
                if (this.m_owner.PlayerHasActiveSynergy("#OILER_CYLINDER") && UnityEngine.Random.value <= 0.25f)
                {
                    this.m_owner.CurrentGun.GainAmmo(1);
                }
            }
            if (this.m_owner.PlayerHasActiveSynergy("#SIXTHER_CHAMBER"))
            {
                DevilishSynergy();
            }
            if (this.m_owner.PlayerHasActiveSynergy("#YELLOWER_CHAMBER"))
            {
                KaliberSynergy();
            }
            yield break;
        }

        private void KaliberSynergy()
        {
            AIActor orLoadByGuid;
            bool isHoleyKin = false;
            if (UnityEngine.Random.value <= 0.5f)
            {
                orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(RnGEnemyDatabase.GetRnGEnemyGuid(RnGEnemyDatabase.RnGEnemyType.BulletmanBroccoli));
            }
            else
            {
                orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(RnGEnemyDatabase.GetRnGEnemyGuid(RnGEnemyDatabase.RnGEnemyType.BulletmanKaliber));
                isHoleyKin = true;
            }
            IntVector2? nearestAvailableCell = this.m_owner.CurrentRoom.GetNearestAvailableCell(this.m_owner.transform.position.XY(), new IntVector2?(orLoadByGuid.Clearance), new CellTypes?(CellTypes.FLOOR), false, null);
            if (nearestAvailableCell == null)
            {
                return;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, (nearestAvailableCell.Value.ToVector2()).ToVector3ZUp(0f), Quaternion.identity);
            CompanionController orAddComponent = gameObject.GetOrAddComponent<CompanionController>();
            orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
            orAddComponent.Initialize(this.m_owner);
            orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
            AIActor aiactor = gameObject.GetComponent<AIActor>();
            aiactor.HitByEnemyBullets = true;
            aiactor.healthHaver.ModifyDamage += this.ModifyDamageForCompanions;
            if(UnityEngine.Random.value <= 0.05f)
            {
                aiactor.BecomeBlackPhantom();
                aiactor.aiShooter.GetBulletEntry((aiactor.behaviorSpeculator.AttackBehaviors[0] as ShootGunBehavior).OverrideBulletName).ProjectileData.damage = isHoleyKin ? 5 : 10;
            }
            else
            {
                aiactor.aiShooter.GetBulletEntry((aiactor.behaviorSpeculator.AttackBehaviors[0] as ShootGunBehavior).OverrideBulletName).ProjectileData.damage = isHoleyKin ? 7 : 15;
            }
        }

        private void DevilishSynergy()
        {
            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid("5f3abc2d561b4b9c9e72b879c6f10c7e");
            IntVector2? nearestAvailableCell = this.m_owner.CurrentRoom.GetNearestAvailableCell(this.m_owner.transform.position.XY(), new IntVector2?(orLoadByGuid.Clearance), new CellTypes?(CellTypes.FLOOR), false, null);
            if (nearestAvailableCell == null)
            {
                return;
            }
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, (nearestAvailableCell.Value.ToVector2()).ToVector3ZUp(0f), Quaternion.identity);
            CompanionController orAddComponent = gameObject.GetOrAddComponent<CompanionController>();
            orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
            orAddComponent.Initialize(this.m_owner);
            orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
            AIActor aiactor = gameObject.GetComponent<AIActor>();
            aiactor.HitByEnemyBullets = true;
            aiactor.healthHaver.ModifyDamage += this.ModifyDamageForCompanions;
            foreach (AIBulletBank.Entry entry in orAddComponent.bulletBank.Bullets)
            {
                if (aiactor.IsBlackPhantom)
                {
                    entry.BulletObject.GetComponent<Projectile>().baseData.damage = 15;
                }
                else
                {
                    entry.BulletObject.GetComponent<Projectile>().baseData.damage = 10;
                }   
            }
        }

        private void ModifyDamageForCompanions(HealthHaver hh, HealthHaver.ModifyDamageEventArgs args)
        {
            if(args == HealthHaver.ModifyDamageEventArgs.Empty)
            {
                return;
            }
            args.ModifiedDamage *= 5f;
        }

        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            if (target.optionalPalette != null)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }

        private IEnumerator HandlePotSuck(MinorBreakable target)
        {
            Transform copySprite = this.CreatePotSprite(target);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                if (this.m_owner.CurrentGun && copySprite)
                {
                    Vector3 position = this.m_owner.CurrentGun.PrimaryHandAttachPoint.position;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                }
                yield return null;
            }
            if (copySprite)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }
            if (this.m_owner.CurrentGun)
            {
                this.m_owner.CurrentGun.GainAmmo(1);
            }
            yield break;
        }

        private Transform CreatePotSprite(MinorBreakable target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            return gameObject2.transform;
        }

        private static void InitEnemyTypes()
        {
            typesEnemies.Add(GameUIAmmoType.AmmoType.SMALL_BULLET, new List<string>
            {
                "db35531e66ce41cbb81d507a34366dfe",
                "01972dee89fc4404a5c408d50007dad5",
                "70216cae6c1346309d86d4a0b4603045",
                "88b6b6a93d4b4234a67844ef4728382c",
                "df7fb62405dc4697b7721862c7b6b3cd",
                "47bdfec22e8e4568a619130a267eab5b",
                "3cadf10c489b461f9fb8814abc1a09c1",
                "8bb5578fba374e8aae8e10b754e61d62",
                "e5cffcfabfae489da61062ea20539887",
                "1a78cfb776f54641b832e92c44021cf2",
                "d4a9836f8ab14f3fadd0f597438b1f1f",
                "5f3abc2d561b4b9c9e72b879c6f10c7e",
                "844657ad68894a4facb1b8e1aef1abf9",
                "2feb50a6a40f4f50982e89fd276f6f15"
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.MEDIUM_BULLET, new List<string>
            {
                "db35531e66ce41cbb81d507a34366dfe",
                "01972dee89fc4404a5c408d50007dad5",
                "70216cae6c1346309d86d4a0b4603045",
                "88b6b6a93d4b4234a67844ef4728382c",
                "df7fb62405dc4697b7721862c7b6b3cd",
                "47bdfec22e8e4568a619130a267eab5b",
                "3cadf10c489b461f9fb8814abc1a09c1",
                "8bb5578fba374e8aae8e10b754e61d62",
                "e5cffcfabfae489da61062ea20539887",
                "1a78cfb776f54641b832e92c44021cf2",
                "d4a9836f8ab14f3fadd0f597438b1f1f",
                "5f3abc2d561b4b9c9e72b879c6f10c7e",
                "844657ad68894a4facb1b8e1aef1abf9",
                "2feb50a6a40f4f50982e89fd276f6f15"
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.GRENADE, new List<string>
            {
                "4d37ce3d666b4ddda8039929225b7ede",
                "19b420dec96d4e9ea4aebc3398c0ba7a",
                "b4666cb6ef4f4b038ba8924fd8adf38f",
                "566ecca5f3b04945ac6ce1f26dedbf4f",
                "c0260c286c8d4538a697c5bf24976ccf"
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.SHOTGUN, new List<string>
            {
                "128db2f0781141bcb505d8f00f9e4d47",
                "b54d89f9e802455cbb2b8a96a31e8259",
                "2752019b770f473193b08b4005dc781f",
                "7f665bd7151347e298e4d366f8818284",
                "b1770e0f1c744d9d887cc16122882b4f",
                "1bd8e49f93614e76b140077ff2e33f2b",
                "044a9f39712f456597b9762893fbc19c",
                "37340393f97f41b2822bc02d14654172",
                "2d4f8b5404614e7d8b235006acde427a"
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.MUSKETBALL, new List<string>
            {
                EnemyDatabase.GetOrLoadByName("Musketball_Man").EnemyGuid
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.ARROW, new List<string>
            {
                "05891b158cd542b1a5f3df30fb67a7ff"
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.MAGIC, new List<string>
            {
                "8bb5578fba374e8aae8e10b754e61d62",
                "43426a2e39584871b287ac31df04b544"
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.BLUE_SHOTGUN, new List<string>
            {
                "128db2f0781141bcb505d8f00f9e4d47",
                "b54d89f9e802455cbb2b8a96a31e8259",
                "2752019b770f473193b08b4005dc781f",
                "7f665bd7151347e298e4d366f8818284",
                "b1770e0f1c744d9d887cc16122882b4f",
                "1bd8e49f93614e76b140077ff2e33f2b",
                "044a9f39712f456597b9762893fbc19c",
                "37340393f97f41b2822bc02d14654172",
                "2d4f8b5404614e7d8b235006acde427a"
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.SKULL, new List<string>
            {
                "af84951206324e349e1f13f9b7b60c1a",
                "336190e29e8a4f75ab7486595b700d4a",
                "95ec774b5a75467a9ab05fa230c0c143",
                "c2f902b7cbe745efb3db4399927eab34",
                "1cec0cdf383e42b19920787798353e46"
            });
            typesEnemies.Add(GameUIAmmoType.AmmoType.FISH, new List<string>
            {
                EnemyDatabase.GetOrLoadByName("BulletMan_Fish").EnemyGuid,
                EnemyDatabase.GetOrLoadByName("BulletMan_Fish_Blue").EnemyGuid
            });
        }

        private static void InitCustomEnemyTypes()
        {
            customTypesEnemies.Add("genie", new List<string>
            {
                "43426a2e39584871b287ac31df04b544"
            });
            customTypesEnemies.Add("Rifle", new List<string>
            {
                "31a3ea0c54a745e182e22ea54844a82d",
                "c5b11bfc065d417b9c4d03a5e385fe2c"
            });
            customTypesEnemies.Add("ghost_small", new List<string>
            {
                "4db03291a12144d69fe940d5a01de376"
            });
            customTypesEnemies.Add("burning_hand", new List<string>
            {
                "d8a445ea4d944cc1b55a40f22821ae69",
                "ffdc8680bdaa487f8f31995539f74265"
            });
            customTypesEnemies.Add("hate", new List<string>
            {
                "af84951206324e349e1f13f9b7b60c1a",
                "336190e29e8a4f75ab7486595b700d4a",
                "95ec774b5a75467a9ab05fa230c0c143",
                "c2f902b7cbe745efb3db4399927eab34",
                "1cec0cdf383e42b19920787798353e46"
            });
            customTypesEnemies.Add("poison_blob", new List<string>
            {
                "e61cab252cfb435db9172adc96ded75f",
                "fe3fe59d867347839824d5d9ae87f244",
                "b8103805af174924b578c98e95313074"
            });
            customTypesEnemies.Add("urn", new List<string>
            {
                "4db03291a12144d69fe940d5a01de376"
            });
            customTypesEnemies.Add("big_shotgun", new List<string>
            {
                "128db2f0781141bcb505d8f00f9e4d47",
                "b54d89f9e802455cbb2b8a96a31e8259",
                "2752019b770f473193b08b4005dc781f",
                "7f665bd7151347e298e4d366f8818284",
                "b1770e0f1c744d9d887cc16122882b4f",
                "1bd8e49f93614e76b140077ff2e33f2b",
                "044a9f39712f456597b9762893fbc19c",
                "37340393f97f41b2822bc02d14654172",
                "2d4f8b5404614e7d8b235006acde427a"
            });
            customTypesEnemies.Add("bomb", new List<string>
            {
                "4d37ce3d666b4ddda8039929225b7ede",
                "19b420dec96d4e9ea4aebc3398c0ba7a",
                "b4666cb6ef4f4b038ba8924fd8adf38f",
                "566ecca5f3b04945ac6ce1f26dedbf4f",
                "c0260c286c8d4538a697c5bf24976ccf"
            });
            customTypesEnemies.Add("feather", new List<string>
            {
                "76bc43539fc24648bff4568c75c686d1"
            });
            customTypesEnemies.Add("slow_arrow", new List<string>
            {
                "05891b158cd542b1a5f3df30fb67a7ff"
            });
            customTypesEnemies.Add("rock", new List<string>
            {
                "9d50684ce2c044e880878e86dbada919"
            });
            customTypesEnemies.Add("duck", new List<string>
            {
                "8b43a5c59b854eb780f9ab669ec26b7a"
            });
            customTypesEnemies.Add("kirkcannon", new List<string>
            {
                EnemyDatabase.GetOrLoadByName("BulletmanTitan_Boss").EnemyGuid,
                EnemyDatabase.GetOrLoadByName("BulletmanTitan").EnemyGuid,
                EnemyDatabase.GetOrLoadByName("BulletgalTitan_Boss").EnemyGuid,
            });
            customTypesEnemies.Add("tachyon", new List<string>
            {
                "0239c0680f9f467dbe5c4aab7dd1eca6",
                "042edb1dfb614dc385d5ad1b010f2ee3",
                "42be66373a3d4d89b91a35c9ff8adfec",
                "e61cab252cfb435db9172adc96ded75f",
                "fe3fe59d867347839824d5d9ae87f244",
                "b8103805af174924b578c98e95313074",
                "022d7c822bc146b58fe3b0287568aaa2",
                "8a9e9bedac014a829a48735da6daf3da",
                "116d09c26e624bca8cca09fc69c714b3",
                "062b9b64371e46e195de17b6f10e47c8",
                "d1c9781fdac54d9e8498ed89210a0238"
            });
            customTypesEnemies.Add("burning hand green", new List<string>
            {
                "d8a445ea4d944cc1b55a40f22821ae69",
                "ffdc8680bdaa487f8f31995539f74265"
            });
            customTypesEnemies.Add("bomb gold", new List<string>
            {
                "4d37ce3d666b4ddda8039929225b7ede",
                "19b420dec96d4e9ea4aebc3398c0ba7a",
                "b4666cb6ef4f4b038ba8924fd8adf38f",
                "566ecca5f3b04945ac6ce1f26dedbf4f",
                "c0260c286c8d4538a697c5bf24976ccf"
            });
            customTypesEnemies.Add("finished small", new List<string>
            {
                "db35531e66ce41cbb81d507a34366dfe",
                "01972dee89fc4404a5c408d50007dad5",
                "70216cae6c1346309d86d4a0b4603045",
                "88b6b6a93d4b4234a67844ef4728382c",
                "df7fb62405dc4697b7721862c7b6b3cd",
                "47bdfec22e8e4568a619130a267eab5b",
                "3cadf10c489b461f9fb8814abc1a09c1",
                "8bb5578fba374e8aae8e10b754e61d62",
                "e5cffcfabfae489da61062ea20539887",
                "1a78cfb776f54641b832e92c44021cf2",
                "d4a9836f8ab14f3fadd0f597438b1f1f",
                "5f3abc2d561b4b9c9e72b879c6f10c7e",
                "844657ad68894a4facb1b8e1aef1abf9",
                "2feb50a6a40f4f50982e89fd276f6f15"
            });
            customTypesEnemies.Add("golden_gun", new List<string>
            {
                "72d2f44431da43b8a3bae7d8a114a46d",
                "b70cbd875fea498aa7fd14b970248920"
            });
            customTypesEnemies.Add("rail", new List<string>
            {
                "72d2f44431da43b8a3bae7d8a114a46d",
                "b70cbd875fea498aa7fd14b970248920"
            });
        }

        private static Dictionary<GameUIAmmoType.AmmoType, List<string>> typesEnemies = new Dictionary<GameUIAmmoType.AmmoType, List<string>>();
        private static Dictionary<string, List<string>> customTypesEnemies = new Dictionary<string, List<string>>();
    }
}
