using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack
{
    public static class EnemyWorks
    {
        public static void Mount(this AIActor self, GameActor other, tk2dBaseSprite.Anchor attachAnchor = tk2dBaseSprite.Anchor.MiddleCenter, tk2dBaseSprite.Anchor sitAnchor = tk2dBaseSprite.Anchor.LowerCenter, MountedBehaviour.LayeringType layeringType = 
            MountedBehaviour.LayeringType.ABOVE, float heightDifference = 1.5f)
        {
            if (self == null || other == null)
            {
                return;
            }
            MountedBehaviour mountedBehaviour = self.gameObject.AddComponent<MountedBehaviour>();
            mountedBehaviour.mount = other;
            mountedBehaviour.attachAnchor = attachAnchor;
            mountedBehaviour.sitAnchor = sitAnchor;
            mountedBehaviour.layeringType = layeringType;
            mountedBehaviour.heightDifference = heightDifference;
        }

        public static void Dismount(this AIActor self)
        {
            if(self.GetComponent<MountedBehaviour>() != null)
            {
                UnityEngine.Object.Destroy(self.GetComponent<MountedBehaviour>());
            }
        }

        public class MountedBehaviour : BraveBehaviour
        {
            private void Start()
            {
                if(this.mount.GetComponent<HealthHaver>() != null)
                {
                    this.mount.GetComponent<HealthHaver>().OnPreDeath += this.OnMountPreDeath;
                }
            }

            private void OnMountPreDeath(Vector2 direction)
            {
                Destroy(this);
            }

            private void LateUpdate()
            {
                if(this.mount == null || this.mount.transform == null)
                {
                    Destroy(this);
                }
                else
                {
                    if (base.sprite != null)
                    {
                        //ETGModConsole.Log((this.mount.GetComponent<tk2dBaseSprite>() != null).ToString());
                        base.sprite.PlaceAtPositionByAnchor((this.mount.sprite != null) ? this.mount.sprite.transform.position.XY() + this.mount.sprite.GetRelativePositionFromAnchor(this.attachAnchor).Rotate(this.mount.sprite.transform.eulerAngles.z) : 
                            this.mount.transform.position.XY(), this.sitAnchor);
                        if (this.mount.sprite != null)
                        {
                            base.sprite.HeightOffGround = this.GetHeightOffGround(this.layeringType);
                            base.sprite.UpdateZDepth();
                        }
                    }
                    else
                    {
                        this.transform.position = (this.mount.sprite != null) ? this.mount.sprite.transform.position.XY() + this.mount.sprite.GetRelativePositionFromAnchor(this.attachAnchor).Rotate(this.mount.sprite.transform.eulerAngles.z) : 
                            this.mount.transform.position.XY();
                    }
                    base.specRigidbody.Reinitialize();
                    base.specRigidbody.RecheckTriggers = true;
                    base.specRigidbody.CollideWithOthers = false;
                    if(base.healthHaver != null)
                    {
                        base.healthHaver.PreventAllDamage = true;
                    }
                }
            }

            protected override void OnDestroy()
            {
                base.OnDestroy();
                if (base.sprite != null)
                {
                    base.sprite.PlaceAtPositionByAnchor((this.mount.sprite != null) ? this.mount.sprite.transform.position.XY() + this.mount.sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.LowerCenter).Rotate(this.mount.sprite.transform.eulerAngles.z) : 
                        this.mount.transform.position.XY(), tk2dBaseSprite.Anchor.LowerCenter);
                    if (this.mount.sprite != null)
                    {
                        base.sprite.HeightOffGround = this.GetHeightOffGround(LayeringType.CARELESS);
                    }
                }
                else
                {
                    this.transform.position = (this.mount.sprite != null) ? this.mount.sprite.transform.position.XY() + this.mount.sprite.GetRelativePositionFromAnchor(tk2dBaseSprite.Anchor.LowerCenter).Rotate(this.mount.sprite.transform.eulerAngles.z) : 
                        this.mount.transform.position.XY();
                }
                base.specRigidbody.Reinitialize();
                base.specRigidbody.RecheckTriggers = true;
                if (this.mount.GetComponent<HealthHaver>() != null)
                {
                    this.mount.GetComponent<HealthHaver>().OnPreDeath -= this.OnMountPreDeath;
                }
                base.specRigidbody.CollideWithOthers = true;
                base.healthHaver.PreventAllDamage = false;
            }

            private float GetHeightOffGround(LayeringType layeringType)
            {
                if(layeringType == LayeringType.ABOVE)
                {
                    return this.mount.sprite.HeightOffGround + this.heightDifference;
                }
                else if(layeringType == LayeringType.BELOW)
                {
                    return this.mount.sprite.HeightOffGround - this.heightDifference;
                }
                else
                {
                    return this.mount.sprite.HeightOffGround;
                }
            }

            public GameActor mount;
            public tk2dBaseSprite.Anchor attachAnchor;
            public tk2dBaseSprite.Anchor sitAnchor;
            public LayeringType layeringType;
            public float heightDifference;

            public enum LayeringType
            {
                ABOVE,
                BELOW,
                CARELESS
            }
        }
    }
}
