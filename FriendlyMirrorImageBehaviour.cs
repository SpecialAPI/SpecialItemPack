using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack
{
    public class FriendlyMirrorImageBehavior : BasicAttackBehavior
    {
        public FriendlyMirrorImageBehavior()
        {
            this.NumImages = 2;
            this.MaxImages = 5;
            this.MirrorHealth = 15f;
            this.SpawnDelay = 0.5f;
            this.SplitDelay = 1f;
            this.SplitDistance = 1f;
            this.m_actorsToSplit = new List<AIActor>();
            this.m_allImages = new List<AIActor>();
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Upkeep()
        {
            base.Upkeep();
            base.DecrementTimer(ref this.m_timer, false);
            for (int i = this.m_allImages.Count - 1; i >= 0; i--)
            {
                if (!this.m_allImages[i] || !this.m_allImages[i].healthHaver || this.m_allImages[i].healthHaver.IsDead)
                {
                    this.m_allImages.RemoveAt(i);
                }
            }
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
            {
                return behaviorResult;
            }
            if (!this.IsReady())
            {
                return BehaviorResult.Continue;
            }
            this.m_enemyPrefab = EnemyDatabase.GetOrLoadByGuid(this.m_aiActor.EnemyGuid);
            this.m_aiAnimator.PlayUntilFinished(this.Anim, true, null, -1f, false);
            if (this.AnimRequiresTransparency)
            {
                this.m_cachedShader = this.m_aiActor.renderer.material.shader;
                this.m_aiActor.sprite.usesOverrideMaterial = true;
                this.m_aiActor.SetOutlines(false);
                this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            }
            this.m_aiActor.ClearPath();
            this.m_timer = this.SpawnDelay;
            if (this.m_aiActor && this.m_aiActor.knockbackDoer)
            {
                this.m_aiActor.knockbackDoer.SetImmobile(true, "MirrorImageBehavior");
            }
            this.m_aiActor.IsGone = true;
            this.m_aiActor.specRigidbody.CollideWithOthers = false;
            this.m_actorsToSplit.Clear();
            this.m_actorsToSplit.Add(this.m_aiActor);
            this.m_state = FriendlyMirrorImageBehavior.State.Summoning;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.m_state == FriendlyMirrorImageBehavior.State.Summoning)
            {
                if (this.m_timer <= 0f)
                {
                    int num = Mathf.Min(this.NumImages, this.MaxImages - this.m_allImages.Count);
                    for (int i = 0; i < num; i++)
                    {
                        AIActor aiactor = UnityEngine.Object.Instantiate(this.m_enemyPrefab.gameObject, this.m_aiActor.transform.position, Quaternion.identity).GetComponent<AIActor>();
                        CompanionController orAddComponent = aiactor.gameObject.GetOrAddComponent<CompanionController>();
                        if (IsBlackPhantom)
                        {
                            aiactor.gameObject.GetComponent<AIActor>().BecomeBlackPhantom();
                        }
                        orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
                        orAddComponent.Initialize(base.m_aiActor.CompanionOwner);
                        orAddComponent.behaviorSpeculator.MovementBehaviors.Add(new CompanionFollowPlayerBehavior());
                        if (orAddComponent.healthHaver != null)
                        {
                            orAddComponent.healthHaver.PreventAllDamage = true;
                        }
                        if (orAddComponent.bulletBank != null)
                        {
                            orAddComponent.bulletBank.OnProjectileCreated += PokeballItem.CatchProjectileBehaviour.OnPostProcessProjectile;
                        }
                        if (orAddComponent.aiShooter != null)
                        {
                            orAddComponent.aiShooter.PostProcessProjectile += PokeballItem.CatchProjectileBehaviour.OnPostProcessProjectile;
                        }
                        aiactor.HitByEnemyBullets = true;
                        aiactor.transform.position = this.m_aiActor.transform.position;
                        aiactor.specRigidbody.Reinitialize();
                        aiactor.IsGone = true;
                        aiactor.specRigidbody.CollideWithOthers = false;
                        if (!string.IsNullOrEmpty(this.MirrorDeathAnim))
                        {
                            aiactor.aiAnimator.OtherAnimations.Find((AIAnimator.NamedDirectionalAnimation a) => a.name == "death").anim.Prefix = this.MirrorDeathAnim;
                        }
                        aiactor.PreventBlackPhantom = true;
                        if (aiactor.IsBlackPhantom)
                        {
                            aiactor.UnbecomeBlackPhantom();
                        }
                        this.m_actorsToSplit.Add(aiactor);
                        this.m_allImages.Add(aiactor);
                        aiactor.aiAnimator.healthHaver.SetHealthMaximum(this.MirrorHealth * AIActor.BaseLevelHealthModifier, null, false);
                        MirrorImageController mirrorImageController = aiactor.gameObject.AddComponent<MirrorImageController>();
                        mirrorImageController.SetHost(this.m_aiActor);
                        for (int j = 0; j < this.MirroredAnims.Length; j++)
                        {
                            mirrorImageController.MirrorAnimations.Add(this.MirroredAnims[j]);
                        }
                        if (this.AnimRequiresTransparency)
                        {
                            aiactor.sprite.usesOverrideMaterial = true;
                            aiactor.procedurallyOutlined = false;
                            aiactor.SetOutlines(false);
                            aiactor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
                        }
                    }
                    this.m_startAngle = UnityEngine.Random.Range(0f, 360f);
                    this.m_state = FriendlyMirrorImageBehavior.State.Splitting;
                    this.m_timer = this.SplitDelay;
                    return ContinuousBehaviorResult.Continue;
                }
            }
            else if (this.m_state == FriendlyMirrorImageBehavior.State.Splitting)
            {
                float num2 = 360f / (float)this.m_actorsToSplit.Count;
                for (int k = 0; k < this.m_actorsToSplit.Count; k++)
                {
                    this.m_actorsToSplit[k].BehaviorOverridesVelocity = true;
                    this.m_actorsToSplit[k].BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_startAngle + num2 * (float)k, this.SplitDistance / this.SplitDelay);
                }
                if (this.m_timer <= 0f)
                {
                    return ContinuousBehaviorResult.Finished;
                }
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (this.AnimRequiresTransparency && this.m_cachedShader)
            {
                for (int i = 0; i < this.m_actorsToSplit.Count; i++)
                {
                    AIActor aiactor = this.m_actorsToSplit[i];
                    if (aiactor)
                    {
                        aiactor.sprite.usesOverrideMaterial = false;
                        aiactor.procedurallyOutlined = true;
                        aiactor.SetOutlines(true);
                        aiactor.renderer.material.shader = this.m_cachedShader;
                    }
                }
                this.m_cachedShader = null;
            }
            if (!string.IsNullOrEmpty(this.Anim))
            {
                this.m_aiAnimator.EndAnimationIf(this.Anim);
            }
            if (this.m_aiActor && this.m_aiActor.knockbackDoer)
            {
                this.m_aiActor.knockbackDoer.SetImmobile(false, "MirrorImageBehavior");
            }
            for (int j = 0; j < this.m_actorsToSplit.Count; j++)
            {
                AIActor aiactor2 = this.m_actorsToSplit[j];
                if (aiactor2)
                {
                    aiactor2.BehaviorOverridesVelocity = false;
                    aiactor2.IsGone = false;
                    aiactor2.specRigidbody.CollideWithOthers = true;
                    if (aiactor2 != this.m_aiActor)
                    {
                        aiactor2.PreventBlackPhantom = false;
                        if (this.m_aiActor.IsBlackPhantom)
                        {
                            aiactor2.BecomeBlackPhantom();
                        }
                    }
                }
            }
            this.m_actorsToSplit.Clear();
            this.m_state = FriendlyMirrorImageBehavior.State.Idle;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        public override bool IsReady()
        {
            return (this.MaxImages <= 0 || this.m_allImages.Count < this.MaxImages) && base.IsReady();
        }

        public int NumImages;
        public int MaxImages;
        public float MirrorHealth;
        public float SpawnDelay;
        public float SplitDelay;
        public float SplitDistance;
        public string Anim;
        public bool AnimRequiresTransparency;
        public string MirrorDeathAnim;
        public string[] MirroredAnims;
        private FriendlyMirrorImageBehavior.State m_state;
        private Shader m_cachedShader;
        private AIActor m_enemyPrefab;
        private float m_timer;
        private float m_startAngle;
        private List<AIActor> m_actorsToSplit;
        private List<AIActor> m_allImages;
        private enum State
        {
            Idle,
            Summoning,
            Splitting
        }
    }
}
