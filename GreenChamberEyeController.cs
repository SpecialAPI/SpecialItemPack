using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;
using System.Collections;

namespace SpecialItemPack
{
    public class GreenChamberEyeController : BraveBehaviour
    {
        public GreenChamberEyeController()
        {
            this.MaxPupilRadius = 2.5f;
        }

        public static void Build()
        {
            GameObject eye = UnityEngine.Object.Instantiate(((GameObject)BraveResources.Load("Global Prefabs/CreepyEye_Room", ".prefab")).GetComponentInChildren<CreepyEyeController>().gameObject);
            eye.SetActive(false);
            FakePrefab.MarkAsFakePrefab(eye);
            UnityEngine.Object.DontDestroyOnLoad(eye);
            CreepyEyeController controller = eye.GetComponent<CreepyEyeController>();
            GreenChamberEyeController customController = eye.gameObject.AddComponent<GreenChamberEyeController>();
            customController.MaxPupilRadius = controller.MaxPupilRadius;
            customController.layers = controller.layers;
            customController.poopil = controller.poopil;
            customController.baseSprite = controller.baseSprite;
            customController.isHallucination = false;
            Destroy(controller);
            GreenChamberEyeController.eyePrefab = eye;
            GameObject eye2 = UnityEngine.Object.Instantiate(((GameObject)BraveResources.Load("Global Prefabs/CreepyEye_Room", ".prefab")).GetComponentInChildren<CreepyEyeController>().gameObject);
            eye2.SetActive(false);
            FakePrefab.MarkAsFakePrefab(eye2);
            UnityEngine.Object.DontDestroyOnLoad(eye2);
            CreepyEyeController controller2 = eye2.GetComponent<CreepyEyeController>();
            GreenChamberEyeController customController2 = eye2.gameObject.AddComponent<GreenChamberEyeController>();
            customController2.MaxPupilRadius = controller2.MaxPupilRadius;
            customController2.layers = controller2.layers;
            customController2.poopil = controller2.poopil;
            customController2.baseSprite = controller2.baseSprite;
            customController2.isHallucination = true;
            Destroy(controller2);
            GreenChamberEyeController.hallucinationEyePrefab = eye2;
        }

        public void ChangeScale(float newScale)
        {
            this.scale = newScale;
        }

        private void Start()
        {
            if (this.isHallucination)
            {
                base.specRigidbody.enabled = false;
                this.baseSprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage");
                this.baseSprite.renderer.sharedMaterial.SetFloat("_EmissivePower", 0);
                this.baseSprite.renderer.sharedMaterial.SetFloat("_Opacity", 1f);
                this.baseSprite.renderer.sharedMaterial.SetColor("_DashColor", new Color(0.502f, 0.502f, 0.502f, 0.502f));
                for (int i = 0; i < this.layers.Length; i++)
                {
                    this.layers[i].sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage");
                    this.layers[i].sprite.renderer.sharedMaterial.SetFloat("_EmissivePower", 0);
                    this.layers[i].sprite.renderer.sharedMaterial.SetFloat("_Opacity", 1f);
                    this.layers[i].sprite.renderer.sharedMaterial.SetColor("_DashColor", new Color(0.502f, 0.502f, 0.502f, 0.502f));
                }
                this.poopil.GetComponent<tk2dBaseSprite>().renderer.material.shader = ShaderCache.Acquire("Brave/Internal/HighPriestAfterImage");
                this.poopil.GetComponent<tk2dBaseSprite>().renderer.sharedMaterial.SetFloat("_EmissivePower", 0);
                this.poopil.GetComponent<tk2dBaseSprite>().renderer.sharedMaterial.SetFloat("_Opacity", 1f);
                this.poopil.GetComponent<tk2dBaseSprite>().renderer.sharedMaterial.SetColor("_DashColor", new Color(0.502f, 0.502f, 0.502f, 0.502f));
            }
        }

        public void BindWithRoom(Dungeonator.RoomHandler room)
        {
            room.Entered += delegate (PlayerController p)
            {
                base.StartCoroutine(this.Vanish(p));
            };
        }

        public IEnumerator Vanish(PlayerController p)
        {
            AkSoundEngine.PostEvent("Play_ENM_darken_world_01", base.gameObject);
            while (this.baseSprite.renderer.material.GetFloat("_Opacity") > 0)
            {
                this.baseSprite.renderer.material.SetFloat("_Opacity", this.baseSprite.renderer.material.GetFloat("_Opacity") - BraveTime.DeltaTime/2);
                for (int i = 0; i < this.layers.Length; i++)
                {
                    this.layers[i].sprite.renderer.material.SetFloat("_Opacity", this.baseSprite.renderer.material.GetFloat("_Opacity"));
                }
                this.poopil.GetComponent<tk2dBaseSprite>().renderer.material.SetFloat("_Opacity", this.baseSprite.renderer.material.GetFloat("_Opacity"));
                yield return null;
            }
            Destroy(base.gameObject);
            PlatformInterface.SetAlienFXColor(new Color(1f, 0f, 0f, 1f), 1f);
            p.DoVibration(Vibration.Time.Quick, Vibration.Strength.Medium);
            Pixelator.Instance.HandleDamagedVignette(Vector2.zero);
            ScreenShakeSettings shakesettings = new ScreenShakeSettings(0.25f, 7f, 0.1f, 0.3f);
            GameManager.Instance.MainCameraController.DoScreenShake(shakesettings, new Vector2?(p.specRigidbody.UnitCenter), false);
            AkSoundEngine.PostEvent("Play_WPN_kthulu_blast_01", p.gameObject);
            yield break;
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
            {
                Vector2 vector = GameManager.Instance.PrimaryPlayer.CenterPosition - base.transform.position.XY();
                float d = Mathf.Lerp(0f, this.MaxPupilRadius * this.scale, vector.magnitude / 7f);
                this.poopil.transform.localPosition = d * vector.normalized;
            }
            float x = this.baseSprite.GetBounds().extents.x;
            float x2 = this.poopil.GetComponent<tk2dSprite>().GetBounds().extents.x;
            this.baseSprite.scale = new Vector3(this.scale, this.scale, this.scale);
            for (int i = 0; i < this.layers.Length; i++)
            {
                if (this.layers[i].sprite == null)
                {
                    this.layers[i].sprite = this.layers[i].xform.GetComponent<tk2dSprite>();
                }
                float x3 = this.layers[i].sprite.GetBounds().extents.x;
                float num = 1f - x3 / x;
                float num2 = (float)i / ((float)this.layers.Length - 1f);
                num = Mathf.Pow(num, Mathf.Lerp(0.75f, 1f, 1f - num2));
                float d2 = this.poopil.localPosition.magnitude / (x - x2);
                this.layers[i].xform.localPosition = this.poopil.localPosition * num + this.poopil.localPosition.normalized * x2 * d2 * num;
                this.layers[i].xform.localPosition = this.layers[i].xform.localPosition.Quantize(0.0625f);
                this.layers[i].sprite.HeightOffGround = (float)i * 0.1f + 0.1f;
                this.layers[i].sprite.UpdateZDepth();
                this.layers[i].sprite.scale = new Vector3(this.scale, this.scale, this.scale);
            }
            this.poopil.GetComponent<tk2dSprite>().HeightOffGround = 1f;
            this.poopil.GetComponent<tk2dSprite>().UpdateZDepth();
            this.poopil.GetComponent<tk2dSprite>().scale = new Vector3(this.scale, this.scale, this.scale);
        }

        public float MaxPupilRadius;
        public CreepyEyeLayer[] layers;
        public Transform poopil;
        public tk2dSprite baseSprite;
        public static GameObject eyePrefab;
        public static GameObject hallucinationEyePrefab;
        public bool isHallucination;
        private float scale = 1f;
    }
}
