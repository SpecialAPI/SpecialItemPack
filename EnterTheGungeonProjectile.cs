using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpecialItemPack
{
    class EnterTheGungeonProjectile : MonoBehaviour
    {
        public void Start()
        {
            this.m_proj = base.GetComponent<Projectile>();
            this.m_owner = this.m_proj.Owner as PlayerController;
            this.m_identifier = (EtGProjectileIdentifier)UnityEngine.Random.Range(1, 114);
            this.m_data = EtGProjectileData.GetDataForIdentifier(this.m_identifier);
            this.m_proj.sprite.spriteId = this.m_proj.sprite.GetSpriteIdByName(this.m_data.spriteName);
            PixelCollider pixelCollider = this.m_proj.specRigidbody.PrimaryPixelCollider;
            this.m_proj.baseData.speed = 0;
            pixelCollider.ManualOffsetX = 0;
            pixelCollider.ManualOffsetY = 0;
            pixelCollider.ManualWidth = this.m_data.dimensions.x;
            pixelCollider.ManualHeight = this.m_data.dimensions.y;
            pixelCollider.Regenerate(this.m_proj.transform, true, true);
            this.m_proj.specRigidbody.PixelColliders = new List<PixelCollider>
            {
                pixelCollider
            };
            if (!this.m_data.canRotate)
            {
                this.m_proj.shouldRotate = false;
                this.m_proj.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        public EtGProjectileData Data
        {
            get
            {
                return this.m_data;
            }
        }

        public enum EtGProjectileIdentifier
        {
            Aged_Gunsinger,
            Agonizer,
            Ammomancer,
            Apprentice_Gunjurer,
            Arrowkin,
            Ashen_Bullet_Kin,
            Ashen_Shotgun_Kin,
            Bandana_Bullet_Kin,
            Beadie,
            Blizzbulon,
            Blobulin,
            Blobuloid,
            Blobulon,
            Bloodbulon,
            Blue_Bookllet,
            Blue_Shotgun_Kin,
            Bombshee,
            Bookllet,
            Bullat,
            Bullet,
            Bullet_Kin,
            Bullet_Shark,
            Cardinal,
            Chain_Gunner,
            Chance_Kin,
            Chancebulon,
            Coaler,
            Confirmed,
            Convict,
            Cop,
            Cormorant,
            Creech,
            Cubulead,
            Cubulon,
            Dead_Blow,
            Det,
            Executioner,
            Fallen_Bullet_Kin,
            Flesh_Cube,
            Fungun,
            Gat,
            Gigi,
            Great_Bullet_Shark,
            Green_Bookllet,
            Grenat,
            Gripmaster,
            Gummy,
            Gun_Cultist,
            Gun_Fairy,
            Gun_Nut,
            Gunjurer,
            Gunreaper,
            Gunsinger,
            Gunslinger,
            Gunzockie,
            Gunzookie,
            High_Gunjurer,
            Hollowpoint,
            Hunter,
            Jamerlengo,
            Jammomancer,
            Junkan,
            Keybullet_Kin,
            Killithid,
            King_Bullat,
            Knight_Ad,
            Lead_Cube,
            Lead_Maiden,
            Leadbulon,
            Lord_of_the_Jammed,
            Lore_Gunjurer,
            Marine,
            Mimic,
            Minelet,
            Misfire_Beast,
            Mountain_Cube,
            Mutant_Bullet_Kin,
            Mutant_Shotgun_Kin,
            Muzzle_Flare,
            Muzzle_Wisp,
            Nitra,
            Phaser_Spider,
            Pilot,
            Pinhead,
            Poisbulin,
            Poisbuloid,
            Poisbulon,
            Poopulon,
            Professional,
            Rat,
            Red_Shotgun_Kin,
            Revolvevant,
            Robot,
            Rubber_Kin,
            Shambling_Round,
            Shelleton,
            Shotgat,
            Shotgrub,
            Shroomer,
            Skullet,
            Skullmet,
            Skusket,
            Sniper_Shell,
            Spectral_Gun_Nut,
            Spent,
            Spirat,
            Spogre,
            Sprun,
            Tanker,
            Tarnisher,
            Tazie,
            Tombstoner,
            Veteran_Bullet_Kin,
            Veteran_Shotgun_Kin,
            Wizbang
        }

        private EtGProjectileIdentifier m_identifier;
        private EtGProjectileData m_data;
        private Projectile m_proj;
        private PlayerController m_owner;

        public class EtGProjectileData
        {
            public static EtGProjectileData GetDataForIdentifier(EtGProjectileIdentifier identifier)
            {
                EtGProjectileData data = new EtGProjectileData();
                data.spriteName = identifier.ToString();
                data.canRotate = true;
                data.canChangeSides = false;
                if(identifier == EtGProjectileIdentifier.Blobuloid || identifier == EtGProjectileIdentifier.Blobulin || identifier == EtGProjectileIdentifier.Bloodbulon || identifier == EtGProjectileIdentifier.Coaler ||
                    identifier == EtGProjectileIdentifier.Flesh_Cube || identifier == EtGProjectileIdentifier.Gunreaper || identifier == EtGProjectileIdentifier.Knight_Ad ||
                    identifier == EtGProjectileIdentifier.Lead_Cube || identifier == EtGProjectileIdentifier.Lord_of_the_Jammed  || identifier == EtGProjectileIdentifier.Blizzbulon || identifier == EtGProjectileIdentifier.Beadie ||
                    identifier == EtGProjectileIdentifier.Cubulead || identifier == EtGProjectileIdentifier.Cubulead || identifier == EtGProjectileIdentifier.Blue_Bookllet || identifier == EtGProjectileIdentifier.Green_Bookllet ||
                    identifier == EtGProjectileIdentifier.Bookllet || identifier == EtGProjectileIdentifier.Leadbulon || identifier == EtGProjectileIdentifier.Mountain_Cube || identifier == EtGProjectileIdentifier.Tarnisher)
                {
                    data.canRotate = false;
                }
                if(identifier == EtGProjectileIdentifier.Dead_Blow || identifier == EtGProjectileIdentifier.Knight_Ad || identifier == EtGProjectileIdentifier.Misfire_Beast)
                {
                    data.canChangeSides = true;
                }
                data.dimensions = EtGProjectileData.ReadyDimensionDictionary()[identifier];
                data.betrayalEnemyGuids = EtGProjectileData.ReadyGuidDictionary()[identifier];
                return data;
            }

            public static Dictionary<EtGProjectileIdentifier, IntVector2> ReadyDimensionDictionary()
            {
                Dictionary<EtGProjectileIdentifier, IntVector2> dict = new Dictionary<EtGProjectileIdentifier, IntVector2>();
                dict.Add(EtGProjectileIdentifier.Aged_Gunsinger, new IntVector2(25, 21));
                dict.Add(EtGProjectileIdentifier.Agonizer, new IntVector2(61, 46));
                dict.Add(EtGProjectileIdentifier.Ammomancer, new IntVector2(22, 14));
                dict.Add(EtGProjectileIdentifier.Apprentice_Gunjurer, new IntVector2(19, 20));
                dict.Add(EtGProjectileIdentifier.Arrowkin, new IntVector2(22, 15));
                dict.Add(EtGProjectileIdentifier.Ashen_Bullet_Kin, new IntVector2(23, 12));
                dict.Add(EtGProjectileIdentifier.Ashen_Shotgun_Kin, new IntVector2(29, 12));
                dict.Add(EtGProjectileIdentifier.Bandana_Bullet_Kin, new IntVector2(23, 14));
                dict.Add(EtGProjectileIdentifier.Beadie, new IntVector2(18, 21));
                dict.Add(EtGProjectileIdentifier.Blizzbulon, new IntVector2(19, 20));
                dict.Add(EtGProjectileIdentifier.Blobulin, new IntVector2(9, 8));
                dict.Add(EtGProjectileIdentifier.Blobuloid, new IntVector2(15, 14));
                dict.Add(EtGProjectileIdentifier.Blobulon, new IntVector2(19, 20));
                dict.Add(EtGProjectileIdentifier.Bloodbulon, new IntVector2(19, 20));
                dict.Add(EtGProjectileIdentifier.Blue_Bookllet, new IntVector2(13, 14));
                dict.Add(EtGProjectileIdentifier.Blue_Shotgun_Kin, new IntVector2(29, 12));
                dict.Add(EtGProjectileIdentifier.Bombshee, new IntVector2(27, 18));
                dict.Add(EtGProjectileIdentifier.Bookllet, new IntVector2(13, 14));
                dict.Add(EtGProjectileIdentifier.Bullat, new IntVector2(11, 33));
                dict.Add(EtGProjectileIdentifier.Bullet, new IntVector2(23, 14));
                dict.Add(EtGProjectileIdentifier.Bullet_Kin, new IntVector2(23, 12));
                dict.Add(EtGProjectileIdentifier.Bullet_Shark, new IntVector2(20, 10));
                dict.Add(EtGProjectileIdentifier.Cardinal, new IntVector2(28, 16));
                dict.Add(EtGProjectileIdentifier.Chain_Gunner, new IntVector2(45, 39));
                dict.Add(EtGProjectileIdentifier.Chance_Kin, new IntVector2(25, 14));
                dict.Add(EtGProjectileIdentifier.Chancebulon, new IntVector2(27, 27));
                dict.Add(EtGProjectileIdentifier.Coaler, new IntVector2(22, 21));
                dict.Add(EtGProjectileIdentifier.Confirmed, new IntVector2(27, 16));
                dict.Add(EtGProjectileIdentifier.Convict, new IntVector2(20, 16));
                dict.Add(EtGProjectileIdentifier.Cop, new IntVector2(21, 15));
                dict.Add(EtGProjectileIdentifier.Cormorant, new IntVector2(22, 18));
                dict.Add(EtGProjectileIdentifier.Creech, new IntVector2(29, 18));
                dict.Add(EtGProjectileIdentifier.Cubulead, new IntVector2(22, 20));
                dict.Add(EtGProjectileIdentifier.Dead_Blow, new IntVector2(61, 53));
                dict.Add(EtGProjectileIdentifier.Det, new IntVector2(26, 32));
                dict.Add(EtGProjectileIdentifier.Executioner, new IntVector2(33, 17));
                dict.Add(EtGProjectileIdentifier.Fallen_Bullet_Kin, new IntVector2(23, 14));
                dict.Add(EtGProjectileIdentifier.Flesh_Cube, new IntVector2(28, 38));
                dict.Add(EtGProjectileIdentifier.Fungun, new IntVector2(28, 25));
                dict.Add(EtGProjectileIdentifier.Gat, new IntVector2(18, 15));
                dict.Add(EtGProjectileIdentifier.Gigi, new IntVector2(20, 23));
                dict.Add(EtGProjectileIdentifier.Great_Bullet_Shark, new IntVector2(33, 16));
                dict.Add(EtGProjectileIdentifier.Green_Bookllet, new IntVector2(13, 14));
                dict.Add(EtGProjectileIdentifier.Grenat, new IntVector2(11, 33));
                dict.Add(EtGProjectileIdentifier.Gripmaster, new IntVector2(34, 59));
                dict.Add(EtGProjectileIdentifier.Gummy, new IntVector2(23, 12));
                dict.Add(EtGProjectileIdentifier.Gun_Cultist, new IntVector2(19, 15));
                dict.Add(EtGProjectileIdentifier.Gun_Fairy, new IntVector2(14, 18));
                dict.Add(EtGProjectileIdentifier.Gun_Nut, new IntVector2(50, 45));
                dict.Add(EtGProjectileIdentifier.Gunjurer, new IntVector2(19, 15));
                dict.Add(EtGProjectileIdentifier.Gunreaper, new IntVector2(58, 62));
                dict.Add(EtGProjectileIdentifier.Gunsinger, new IntVector2(22, 21));
                dict.Add(EtGProjectileIdentifier.Gunslinger, new IntVector2(19, 20));
                dict.Add(EtGProjectileIdentifier.Gunzockie, new IntVector2(16, 18));
                dict.Add(EtGProjectileIdentifier.Gunzookie, new IntVector2(16, 18));
                dict.Add(EtGProjectileIdentifier.High_Gunjurer, new IntVector2(18, 16));
                dict.Add(EtGProjectileIdentifier.Hollowpoint, new IntVector2(23, 12));
                dict.Add(EtGProjectileIdentifier.Hunter, new IntVector2(22, 14));
                dict.Add(EtGProjectileIdentifier.Jamerlengo, new IntVector2(40, 25));
                dict.Add(EtGProjectileIdentifier.Jammomancer, new IntVector2(34, 14));
                dict.Add(EtGProjectileIdentifier.Junkan, new IntVector2(14, 12));
                dict.Add(EtGProjectileIdentifier.Keybullet_Kin, new IntVector2(32, 12));
                dict.Add(EtGProjectileIdentifier.Killithid, new IntVector2(44, 28));
                dict.Add(EtGProjectileIdentifier.King_Bullat, new IntVector2(23, 50));
                dict.Add(EtGProjectileIdentifier.Knight_Ad, new IntVector2(50, 34));
                dict.Add(EtGProjectileIdentifier.Lead_Cube, new IntVector2(28, 36));
                dict.Add(EtGProjectileIdentifier.Lead_Maiden, new IntVector2(60, 29));
                dict.Add(EtGProjectileIdentifier.Leadbulon, new IntVector2(17, 14));
                dict.Add(EtGProjectileIdentifier.Lord_of_the_Jammed, new IntVector2(58, 64));
                dict.Add(EtGProjectileIdentifier.Lore_Gunjurer, new IntVector2(23, 23));
                dict.Add(EtGProjectileIdentifier.Marine, new IntVector2(20, 17));
                dict.Add(EtGProjectileIdentifier.Mimic, new IntVector2(23, 25));
                dict.Add(EtGProjectileIdentifier.Minelet, new IntVector2(21, 14));
                dict.Add(EtGProjectileIdentifier.Misfire_Beast, new IntVector2(79, 45));
                dict.Add(EtGProjectileIdentifier.Mountain_Cube, new IntVector2(36, 47));
                dict.Add(EtGProjectileIdentifier.Mutant_Bullet_Kin, new IntVector2(23, 15));
                dict.Add(EtGProjectileIdentifier.Mutant_Shotgun_Kin, new IntVector2(27, 15));
                dict.Add(EtGProjectileIdentifier.Muzzle_Flare, new IntVector2(28, 26));
                dict.Add(EtGProjectileIdentifier.Muzzle_Wisp, new IntVector2(28, 27));
                dict.Add(EtGProjectileIdentifier.Nitra, new IntVector2(32, 12));
                dict.Add(EtGProjectileIdentifier.Phaser_Spider, new IntVector2(51, 36));
                dict.Add(EtGProjectileIdentifier.Pilot, new IntVector2(21, 15));
                dict.Add(EtGProjectileIdentifier.Pinhead, new IntVector2(22, 16));
                dict.Add(EtGProjectileIdentifier.Poisbulin, new IntVector2(12, 7));
                dict.Add(EtGProjectileIdentifier.Poisbuloid, new IntVector2(20, 12));
                dict.Add(EtGProjectileIdentifier.Poisbulon, new IntVector2(19, 20));
                dict.Add(EtGProjectileIdentifier.Poopulon, new IntVector2(19, 18));
                dict.Add(EtGProjectileIdentifier.Professional, new IntVector2(35, 16));
                dict.Add(EtGProjectileIdentifier.Rat, new IntVector2(21, 16));
                dict.Add(EtGProjectileIdentifier.Red_Shotgun_Kin, new IntVector2(29, 12));
                dict.Add(EtGProjectileIdentifier.Revolvevant, new IntVector2(78, 35));
                dict.Add(EtGProjectileIdentifier.Robot, new IntVector2(19, 16));
                dict.Add(EtGProjectileIdentifier.Rubber_Kin, new IntVector2(19, 12));
                dict.Add(EtGProjectileIdentifier.Shambling_Round, new IntVector2(73, 50));
                dict.Add(EtGProjectileIdentifier.Shelleton, new IntVector2(38, 32));
                dict.Add(EtGProjectileIdentifier.Shotgat, new IntVector2(11, 33));
                dict.Add(EtGProjectileIdentifier.Shotgrub, new IntVector2(29, 15));
                dict.Add(EtGProjectileIdentifier.Shroomer, new IntVector2(22, 22));
                dict.Add(EtGProjectileIdentifier.Skullet, new IntVector2(23, 12));
                dict.Add(EtGProjectileIdentifier.Skullmet, new IntVector2(24, 15));
                dict.Add(EtGProjectileIdentifier.Skusket, new IntVector2(15, 18));
                dict.Add(EtGProjectileIdentifier.Sniper_Shell, new IntVector2(35, 12));
                dict.Add(EtGProjectileIdentifier.Spectral_Gun_Nut, new IntVector2(37, 40));
                dict.Add(EtGProjectileIdentifier.Spent, new IntVector2(18, 12));
                dict.Add(EtGProjectileIdentifier.Spirat, new IntVector2(11, 33));
                dict.Add(EtGProjectileIdentifier.Spogre, new IntVector2(50, 40));
                dict.Add(EtGProjectileIdentifier.Sprun, new IntVector2(10, 10));
                dict.Add(EtGProjectileIdentifier.Tanker, new IntVector2(23, 16));
                dict.Add(EtGProjectileIdentifier.Tarnisher, new IntVector2(38, 39));
                dict.Add(EtGProjectileIdentifier.Tazie, new IntVector2(18, 12));
                dict.Add(EtGProjectileIdentifier.Tombstoner, new IntVector2(21, 14));
                dict.Add(EtGProjectileIdentifier.Veteran_Bullet_Kin, new IntVector2(23, 12));
                dict.Add(EtGProjectileIdentifier.Veteran_Shotgun_Kin, new IntVector2(26, 12));
                dict.Add(EtGProjectileIdentifier.Wizbang, new IntVector2(33, 34));
                return dict;
            }

            public static Dictionary<EtGProjectileIdentifier, List<string>> ReadyGuidDictionary()
            {
                Dictionary<EtGProjectileIdentifier, List<string>> dict = new Dictionary<EtGProjectileIdentifier, List<string>>();
                dict.Add(EtGProjectileIdentifier.Aged_Gunsinger, new List<string> { "c50a862d19fc4d30baeba54795e8cb93" });
                dict.Add(EtGProjectileIdentifier.Agonizer, new List<string> { "3f6d6b0c4a7c4690807435c7b37c35a5" });
                dict.Add(EtGProjectileIdentifier.Ammomancer, new List<string> { "b1540990a4f1480bbcb3bea70d67f60d" });
                dict.Add(EtGProjectileIdentifier.Apprentice_Gunjurer, new List<string> { "206405acad4d4c33aac6717d184dc8d4" });
                dict.Add(EtGProjectileIdentifier.Arrowkin, new List<string> { "05891b158cd542b1a5f3df30fb67a7ff" });
                dict.Add(EtGProjectileIdentifier.Ashen_Bullet_Kin, new List<string> { "1a78cfb776f54641b832e92c44021cf2" });
                dict.Add(EtGProjectileIdentifier.Ashen_Shotgun_Kin, new List<string> { "1bd8e49f93614e76b140077ff2e33f2b" });
                dict.Add(EtGProjectileIdentifier.Bandana_Bullet_Kin, new List<string> { "88b6b6a93d4b4234a67844ef4728382c" });
                dict.Add(EtGProjectileIdentifier.Beadie, new List<string> { "7b0b1b6d9ce7405b86b75ce648025dd6" });
                dict.Add(EtGProjectileIdentifier.Blizzbulon, new List<string> { "022d7c822bc146b58fe3b0287568aaa2" });
                dict.Add(EtGProjectileIdentifier.Blobulin, new List<string> { "42be66373a3d4d89b91a35c9ff8adfec" });
                dict.Add(EtGProjectileIdentifier.Blobuloid, new List<string> { "042edb1dfb614dc385d5ad1b010f2ee3" });
                dict.Add(EtGProjectileIdentifier.Blobulon, new List<string> { "0239c0680f9f467dbe5c4aab7dd1eca6" });
                dict.Add(EtGProjectileIdentifier.Bloodbulon, new List<string> { "062b9b64371e46e195de17b6f10e47c8" });
                dict.Add(EtGProjectileIdentifier.Blue_Bookllet, new List<string> { "6f22935656c54ccfb89fca30ad663a64" });
                dict.Add(EtGProjectileIdentifier.Blue_Shotgun_Kin, new List<string> { "b54d89f9e802455cbb2b8a96a31e8259" });
                dict.Add(EtGProjectileIdentifier.Bombshee, new List<string> { "19b420dec96d4e9ea4aebc3398c0ba7a" });
                dict.Add(EtGProjectileIdentifier.Bookllet, new List<string> { "c0ff3744760c4a2eb0bb52ac162056e6" });
                dict.Add(EtGProjectileIdentifier.Bullat, new List<string> { "2feb50a6a40f4f50982e89fd276f6f15" });
                dict.Add(EtGProjectileIdentifier.Bullet_Kin, new List<string> { "01972dee89fc4404a5c408d50007dad5",
                "db35531e66ce41cbb81d507a34366dfe"});
                dict.Add(EtGProjectileIdentifier.Bullet_Shark, new List<string> { "72d2f44431da43b8a3bae7d8a114a46d" });
                dict.Add(EtGProjectileIdentifier.Cardinal, new List<string> { "8bb5578fba374e8aae8e10b754e61d62" });
                dict.Add(EtGProjectileIdentifier.Chain_Gunner, new List<string> { "463d16121f884984abe759de38418e48" });
                dict.Add(EtGProjectileIdentifier.Chance_Kin, new List<string> { "a446c626b56d4166915a4e29869737fd" });
                dict.Add(EtGProjectileIdentifier.Chancebulon, new List<string> { "1bc2a07ef87741be90c37096910843ab" });
                dict.Add(EtGProjectileIdentifier.Coaler, new List<string> { "9d50684ce2c044e880878e86dbada919" });
                dict.Add(EtGProjectileIdentifier.Confirmed, new List<string> { "844657ad68894a4facb1b8e1aef1abf9" });
                dict.Add(EtGProjectileIdentifier.Creech, new List<string> { "37340393f97f41b2822bc02d14654172" });
                dict.Add(EtGProjectileIdentifier.Cubulead, new List<string> { "0b547ac6b6fc4d68876a241a88f5ca6a" });
                dict.Add(EtGProjectileIdentifier.Dead_Blow, new List<string> { "a38e9dca103c4e4fa4bf478cf9a2f2de" });
                dict.Add(EtGProjectileIdentifier.Det, new List<string> { "ac986dabc5a24adab11d48a4bccf4cb1",
                "48d74b9c65f44b888a94f9e093554977"});
                dict.Add(EtGProjectileIdentifier.Executioner, new List<string> { "b1770e0f1c744d9d887cc16122882b4f" });
                dict.Add(EtGProjectileIdentifier.Fallen_Bullet_Kin, new List<string> { "5f3abc2d561b4b9c9e72b879c6f10c7e" });
                dict.Add(EtGProjectileIdentifier.Flesh_Cube, new List<string> { "3f2026dc3712490289c4658a2ba4a24b" });
                dict.Add(EtGProjectileIdentifier.Fungun, new List<string> { "f905765488874846b7ff257ff81d6d0c" });
                dict.Add(EtGProjectileIdentifier.Gat, new List<string> { "9b4fb8a2a60a457f90dcf285d34143ac" });
                dict.Add(EtGProjectileIdentifier.Gigi, new List<string> { "ed37fa13e0fa4fcf8239643957c51293" });
                dict.Add(EtGProjectileIdentifier.Great_Bullet_Shark, new List<string> { "b70cbd875fea498aa7fd14b970248920" });
                dict.Add(EtGProjectileIdentifier.Green_Bookllet, new List<string> { "a400523e535f41ac80a43ff6b06dc0bf" });
                dict.Add(EtGProjectileIdentifier.Grenat, new List<string> { "b4666cb6ef4f4b038ba8924fd8adf38f" });
                dict.Add(EtGProjectileIdentifier.Gripmaster, new List<string> { "22fc2c2c45fb47cf9fb5f7b043a70122" });
                dict.Add(EtGProjectileIdentifier.Gummy, new List<string> { "5288e86d20184fa69c91ceb642d31474" });
                dict.Add(EtGProjectileIdentifier.Gun_Cultist, new List<string> { "57255ed50ee24794b7aac1ac3cfb8a95" });
                dict.Add(EtGProjectileIdentifier.Gun_Fairy, new List<string> { "c182a5cb704d460d9d099a47af49c913" });
                dict.Add(EtGProjectileIdentifier.Gun_Nut, new List<string> { "ec8ea75b557d4e7b8ceeaacdf6f8238c" });
                dict.Add(EtGProjectileIdentifier.Gunjurer, new List<string> { "c4fba8def15e47b297865b18e36cbef8" });
                dict.Add(EtGProjectileIdentifier.Gunreaper, new List<string> { "88f037c3f93b4362a040a87b30770407" });
                dict.Add(EtGProjectileIdentifier.Gunsinger, new List<string> { "8a9e9bedac014a829a48735da6daf3da" });
                dict.Add(EtGProjectileIdentifier.Gunzockie, new List<string> { "6e972cd3b11e4b429b888b488e308551" });
                dict.Add(EtGProjectileIdentifier.Gunzookie, new List<string> { "" });
                dict.Add(EtGProjectileIdentifier.High_Gunjurer, new List<string> { "9b2cf2949a894599917d4d391a0b7394" });
                dict.Add(EtGProjectileIdentifier.Hollowpoint, new List<string> { "4db03291a12144d69fe940d5a01de376" });
                dict.Add(EtGProjectileIdentifier.Jamerlengo, new List<string> { "ba657723b2904aa79f9e51bce7d23872" });
                dict.Add(EtGProjectileIdentifier.Jammomancer, new List<string> { "8b4a938cdbc64e64822e841e482ba3d2" });
                dict.Add(EtGProjectileIdentifier.Keybullet_Kin, new List<string> { "699cd24270af4cd183d671090d8323a1" });
                dict.Add(EtGProjectileIdentifier.Killithid, new List<string> { "3e98ccecf7334ff2800188c417e67c15" });
                dict.Add(EtGProjectileIdentifier.King_Bullat, new List<string> { "1a4872dafdb34fd29fe8ac90bd2cea67" });
                dict.Add(EtGProjectileIdentifier.Lead_Cube, new List<string> { "33b212b856b74ff09252bf4f2e8b8c57" });
                dict.Add(EtGProjectileIdentifier.Lead_Maiden, new List<string> { "cd4a4b7f612a4ba9a720b9f97c52f38c" });
                dict.Add(EtGProjectileIdentifier.Leadbulon, new List<string> { "ccf6d241dad64d989cbcaca2a8477f01" });
                dict.Add(EtGProjectileIdentifier.Lord_of_the_Jammed, new List<string> { "0d3f7c641557426fbac8596b61c9fb45" });
                dict.Add(EtGProjectileIdentifier.Lore_Gunjurer, new List<string> { "56fb939a434140308b8f257f0f447829" });
                dict.Add(EtGProjectileIdentifier.Mimic, new List<string> { "2ebf8ef6728648089babb507dec4edb7",
                "d8d651e3484f471ba8a2daa4bf535ce6",
                "abfb454340294a0992f4173d6e5898a8",
                "6450d20137994881aff0ddd13e3d40c8",
                "d8fd592b184b4ac9a3be217bc70912a2",
                "479556d05c7c44f3b6abb3b2067fc778",
                "ac9d345575444c9a8d11b799e8719be0",
                "796a7ed4ad804984859088fc91672c7f"});
                dict.Add(EtGProjectileIdentifier.Minelet, new List<string> { "3cadf10c489b461f9fb8814abc1a09c1" });
                dict.Add(EtGProjectileIdentifier.Misfire_Beast, new List<string> { "45192ff6d6cb43ed8f1a874ab6bef316" });
                dict.Add(EtGProjectileIdentifier.Mountain_Cube, new List<string> { "f155fd2759764f4a9217db29dd21b7eb" });
                dict.Add(EtGProjectileIdentifier.Mutant_Bullet_Kin, new List<string> { "d4a9836f8ab14f3fadd0f597438b1f1f" });
                dict.Add(EtGProjectileIdentifier.Mutant_Shotgun_Kin, new List<string> { "7f665bd7151347e298e4d366f8818284" });
                dict.Add(EtGProjectileIdentifier.Muzzle_Flare, new List<string> { "d8a445ea4d944cc1b55a40f22821ae69" });
                dict.Add(EtGProjectileIdentifier.Muzzle_Wisp, new List<string> { "ffdc8680bdaa487f8f31995539f74265" });
                dict.Add(EtGProjectileIdentifier.Nitra, new List<string> { "c0260c286c8d4538a697c5bf24976ccf" });
                dict.Add(EtGProjectileIdentifier.Phaser_Spider, new List<string> { "98ca70157c364750a60f5e0084f9d3e2" });
                dict.Add(EtGProjectileIdentifier.Pinhead, new List<string> { "4d37ce3d666b4ddda8039929225b7ede" });
                dict.Add(EtGProjectileIdentifier.Poisbulin, new List<string> { "b8103805af174924b578c98e95313074" });
                dict.Add(EtGProjectileIdentifier.Poisbuloid, new List<string> { "fe3fe59d867347839824d5d9ae87f244" });
                dict.Add(EtGProjectileIdentifier.Poisbulon, new List<string> { "e61cab252cfb435db9172adc96ded75f" });
                dict.Add(EtGProjectileIdentifier.Poopulon, new List<string> { "116d09c26e624bca8cca09fc69c714b3" });
                dict.Add(EtGProjectileIdentifier.Professional, new List<string> { "c5b11bfc065d417b9c4d03a5e385fe2c" });
                dict.Add(EtGProjectileIdentifier.Rat, new List<string> { "6868795625bd46f3ae3e4377adce288b" });
                dict.Add(EtGProjectileIdentifier.Red_Shotgun_Kin, new List<string> { "128db2f0781141bcb505d8f00f9e4d47" });
                dict.Add(EtGProjectileIdentifier.Revolvevant, new List<string> { "d5a7b95774cd41f080e517bea07bf495" });
                dict.Add(EtGProjectileIdentifier.Rubber_Kin, new List<string> { "6b7ef9e5d05b4f96b04f05ef4a0d1b18" });
                dict.Add(EtGProjectileIdentifier.Shambling_Round, new List<string> { "98ea2fe181ab4323ab6e9981955a9bca" });
                dict.Add(EtGProjectileIdentifier.Shelleton, new List<string> { "21dd14e5ca2a4a388adab5b11b69a1e1" });
                dict.Add(EtGProjectileIdentifier.Shotgat, new List<string> { "2d4f8b5404614e7d8b235006acde427a" });
                dict.Add(EtGProjectileIdentifier.Shotgrub, new List<string> { "044a9f39712f456597b9762893fbc19c" });
                dict.Add(EtGProjectileIdentifier.Shroomer, new List<string> { "e5cffcfabfae489da61062ea20539887" });
                dict.Add(EtGProjectileIdentifier.Skullet, new List<string> { "336190e29e8a4f75ab7486595b700d4a" });
                dict.Add(EtGProjectileIdentifier.Skullmet, new List<string> { "95ec774b5a75467a9ab05fa230c0c143" });
                dict.Add(EtGProjectileIdentifier.Skusket, new List<string> { "af84951206324e349e1f13f9b7b60c1a" });
                dict.Add(EtGProjectileIdentifier.Sniper_Shell, new List<string> { "31a3ea0c54a745e182e22ea54844a82d" });
                dict.Add(EtGProjectileIdentifier.Spectral_Gun_Nut, new List<string> { "383175a55879441d90933b5c4e60cf6f" });
                dict.Add(EtGProjectileIdentifier.Spent, new List<string> { "249db525a9464e5282d02162c88e0357" });
                dict.Add(EtGProjectileIdentifier.Spirat, new List<string> { "7ec3e8146f634c559a7d58b19191cd43" });
                dict.Add(EtGProjectileIdentifier.Spogre, new List<string> { "eed5addcc15148179f300cc0d9ee7f94" });
                dict.Add(EtGProjectileIdentifier.Tanker, new List<string> { "df7fb62405dc4697b7721862c7b6b3cd",
                "47bdfec22e8e4568a619130a267eab5b"});
                dict.Add(EtGProjectileIdentifier.Tarnisher, new List<string> { "475c20c1fd474dfbad54954e7cba29c1" });
                dict.Add(EtGProjectileIdentifier.Tazie, new List<string> { "98fdf153a4dd4d51bf0bafe43f3c77ff" });
                dict.Add(EtGProjectileIdentifier.Tombstoner, new List<string> { "cf27dd464a504a428d87a8b2560ad40a" });
                dict.Add(EtGProjectileIdentifier.Veteran_Bullet_Kin, new List<string> { "70216cae6c1346309d86d4a0b4603045" });
                dict.Add(EtGProjectileIdentifier.Veteran_Shotgun_Kin, new List<string> { "2752019b770f473193b08b4005dc781f" });
                dict.Add(EtGProjectileIdentifier.Wizbang, new List<string> { "43426a2e39584871b287ac31df04b544" });
                return dict;
            }

            public IntVector2 dimensions;
            public string spriteName;
            public bool canRotate;
            public ProjectileData projData;
            public List<string> betrayalEnemyGuids;
            public bool canChangeSides = false;
        }
    }
}
