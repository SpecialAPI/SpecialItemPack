using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SpecialItemPack.ItemAPI;

namespace SpecialItemPack
{
    class CustomCreepyEyeController : MonoBehaviour
    {
        public CustomCreepyEyeController()
        {
            this.MaxPupilRadius = 2.5f;
            this.scale = 1f;
        }

        public static void Build()
        {
            GameObject eye = UnityEngine.Object.Instantiate(((GameObject)BraveResources.Load("Global Prefabs/CreepyEye_Room", ".prefab")).GetComponentInChildren<CreepyEyeController>().gameObject);
            eye.SetActive(false);
            FakePrefab.MarkAsFakePrefab(eye);
            UnityEngine.Object.DontDestroyOnLoad(eye);
            CreepyEyeController controller = eye.GetComponent<CreepyEyeController>();
            CustomCreepyEyeController customController = eye.gameObject.AddComponent<CustomCreepyEyeController>();
            customController.MaxPupilRadius = controller.MaxPupilRadius;
            customController.layers = controller.layers;
            customController.poopil = controller.poopil;
            customController.baseSprite = controller.baseSprite;
            if(eye.GetComponent<SpeculativeRigidbody>() != null)
            {
                eye.GetComponent<SpeculativeRigidbody>().CollideWithOthers = false;
                eye.GetComponent<SpeculativeRigidbody>().CollideWithTileMap = false;
            }
            Destroy(controller);
            CustomCreepyEyeController.eyePrefab = eye;
        }

        public void ChangeScale(float newScale)
        {
            this.scale = newScale;
        }

        private void LateUpdate()
        {
            if (Application.isPlaying)
            {
                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(GameManager.Instance.PrimaryPlayer.PlayerIDX);
                bool flag2 = instanceForPlayer == null;
                if (!flag2)
                {
                    Vector2 vector = GameManager.Instance.PrimaryPlayer.unadjustedAimPoint.XY() - base.transform.position.XY();
                    float d = Mathf.Lerp(0f, (this.MaxPupilRadius * this.scale), vector.magnitude / 7f);
                    this.poopil.transform.localPosition = d * vector.normalized;
                }
            }
            this.baseSprite.scale = new Vector3(this.scale, this.scale, this.scale);
            float x = this.baseSprite.GetBounds().extents.x;
            float x2 = this.poopil.GetComponent<tk2dSprite>().GetBounds().extents.x;
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
                this.layers[i].sprite.HeightOffGround = ((float)i * 0.1f + 0.1f);
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
        private float scale;
    }
}
