using BepInEx;
using SlugBase.Features;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using static SlugBase.Features.FeatureTypes;
namespace applecat
{
    // 声明插件
    [BepInPlugin(MOD_ID, "applecat", "0.1.0")]
    class Plugin : BaseUnityPlugin
    {
        private const string MOD_ID = "applecat";
        // 声明特性
        public static readonly PlayerFeature<bool> leaf = PlayerBool("applecat/leaf");//叶子
        public static readonly GameFeature<float> MeanLizards = GameFloat("applecat/mean_lizards");
        public static readonly PlayerFeature<bool> discoloration = PlayerBool("applecat/discoloration");
        public static readonly PlayerFeature<bool> Produceandy = PlayerBool("applecat/Produceandy");

        //声明modules和模型
        public static ConditionalWeakTable<Player, AppleCat> modulesP = new ConditionalWeakTable<Player, AppleCat>();
        public static int index;
        // 添加钩子
        public void OnEnable()
        {
            On.RainWorld.OnModsInit += Extras.WrapInit(LoadResources);
            On.Player.Update += Photosynthesis;
            On.Lizard.ctor += Lizard_ctor;
            On.PlayerGraphics.DrawSprites += Discoloration;
            On.Player.Update += Producecandy;
            On.Player.ctor += AppleSlug_ctor;
            On.Player.ctor += Player_ctor;
            On.PlayerGraphics.AddToContainer += PlayerGraphics_AddToContainer;
            On.PlayerGraphics.InitiateSprites += PlayerGraphics_InitiateSprites;
            On.PlayerGraphics.DrawSprites += PlayerGraphics_DrawSprites;
            DrawMiniAppleHook.Hook();
        }
        private void AppleSlug_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            if (self.room.world.game.session.characterStats.name.value == "applecat")
            {
                modulesP.Add(self, new AppleCat());
            }
        }
        private float timeStacker = 0f;
        private void Producecandy(On.Player.orig_Update orig, Player self, bool eu)
        {
            // 调用原始的 orig 方法
            orig(self, eu);

            // 检查角色是否为 "Applecat"
            if (self.room.world.game.session.characterStats.name.value == "applecat")
            {
                modulesP.TryGetValue(self, out var appleCat);
                {
                    bool noHold = self.grasps[0] == null && self.grasps[1] == null;//没拿
                    bool pickbutton = self.input[0].pckp && self.input[0].y == 0;//按按钮
                    bool eated = self.FoodInStomach > 0;//有饱食度
                    if (self.input[0].y > 0 && self.input[0].pckp)//跳舞
                    {
                        self.bodyChunkConnections[0].distance *= Mathf.Sin(Time.time * (20 + appleCat.swallowAndRegurgitateCounter / 40)) * 0.2f + 0.8f;
                    }
                    if (!(noHold && pickbutton && eated))
                    {
                        appleCat.swallowAndRegurgitateCounter = 0;//用来计数吐不吐
                        return;
                    }
                    if (self.objectInStomach == null)//增加阳光苹果的计数器
                    {
                        appleCat.swallowAndRegurgitateCounter++;
                        (self.graphicsModule as PlayerGraphics).swallowing = 2;
                        self.bodyChunkConnections[0].distance *= Mathf.Sin(Time.time * (20 + appleCat.swallowAndRegurgitateCounter / 40)) * 0.2f + 0.8f;
                    }
                    if (self.objectInStomach == null && appleCat.swallowAndRegurgitateCounter > 150)
                    {
                        self.SubtractFood(2);
                        appleCat.swallowAndRegurgitateCounter = 0;
                        (self.graphicsModule as PlayerGraphics).swallowing = 20;
                        var apple = new AbstractPhysicalObject(self.room.world, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null, self.room.GetWorldCoordinate(self.mainBodyChunk.pos), self.room.game.GetNewID());
                        var realApple = new DangleFruit(apple);
                        DrawMiniAppleHook.modules.Add(realApple, new MiniApple(Color.red));
                        realApple.firstChunk.pos = self.mainBodyChunk.pos;
                        self.room.AddObject(realApple);
                    }
                }
            }
        }
        // 玩家受光合作用影响
        private void Photosynthesis(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            //检测角色是不是applecat
            if (self.room.world.game.session.characterStats.name.value == "applecat")
            {
                if (modulesP.TryGetValue(self, out var AppleCat))
                {
                    AppleCat.WhenInLight(self, eu, 4);
                }
            }
        }
        // 改变蜥蜴的凶残度
        private void Lizard_ctor(On.Lizard.orig_ctor orig, Lizard self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            if (MeanLizards.TryGet(world.game, out float meanness))
            {
                self.spawnDataEvil = Mathf.Min(self.spawnDataEvil, meanness);
            }
        }

        // 玩家颜色变化
        private void Discoloration(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig(self, sLeaser, rCam, timeStacker, camPos);
            float satiation = Mathf.InverseLerp(0, self.player.MaxFoodInStomach, self.player.FoodInStomach);
            //这里的颜色赋值是0和1的RGBA数值
            Color leaves_green = new Color(0.416f, 0.945f, 0.0f, 1.0f);
            Color apple_red = new Color(0.894f, 0.125f, 0.125f, 1.0f);
            Color lerpedColor = Color.Lerp(leaves_green, apple_red, satiation); // 根据饱食度计算出颜色
            if (discoloration.TryGet(self.player, out bool discplo) && discplo) // 如果开启了颜色变化特性
            {
                for (int i = 0; i < sLeaser.sprites.Length; i++)
                {
                    if (i == 9) // 玩家眼睛颜色不变
                    {
                        continue;
                    }
                    sLeaser.sprites[i].color = Color.Lerp(sLeaser.sprites[i].color, lerpedColor, 0.05f); // 通过插值改变颜色
                    var tile = sLeaser.sprites[i] as TriangleMesh;
                    if (tile != null && tile.customColor)
                    {
                        tile._color = Color.Lerp(sLeaser.sprites[i].color, lerpedColor, 0.05f); // 通过插值改变颜色
                    }
                }
            }
        }
        private void PlayerGraphics_DrawSprites(On.PlayerGraphics.orig_DrawSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self, sLeaser, rCam, timeStacker, camPos);
            if (!leaf.TryGet(self.player, out var flag) && flag) { return; }

            var triangleMesh = (sLeaser.sprites[index] as TriangleMesh);
            var headPos = sLeaser.sprites[3].GetPosition();

            triangleMesh.MoveVertice(0, headPos + Vector2.up * 5);
            triangleMesh.MoveVertice(1, headPos + Vector2.up * 17);
            triangleMesh.MoveVertice(2, headPos + Vector2.right * 10);
        }
        private void PlayerGraphics_AddToContainer(On.PlayerGraphics.orig_AddToContainer orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer newContatiner)
        {
            orig.Invoke(self, sLeaser, rCam, newContatiner);
            if (!leaf.TryGet(self.player, out var flag) && flag) { return; }
            bool flag2 = index > 0 && sLeaser.sprites.Length > index;

            if (flag2)
            {
                FContainer fContainer = new FContainer();


                fContainer = rCam.ReturnFContainer("Midground");

                fContainer.AddChild(sLeaser.sprites[index]);
            }
        }
        private void PlayerGraphics_InitiateSprites(On.PlayerGraphics.orig_InitiateSprites orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
        {
            orig.Invoke(self, sLeaser, rCam);
            if (!leaf.TryGet(self.player, out var flag) && flag) { return; }


            index = sLeaser.sprites.Length;
            //给原本的身体精灵扩容
            Array.Resize<FSprite>(ref sLeaser.sprites, sLeaser.sprites.Length + 1);


            TriangleMesh.Triangle[] tris = new TriangleMesh.Triangle[]
            {
                new TriangleMesh.Triangle(0, 1, 2)
            };
            sLeaser.sprites[index] = new TriangleMesh("Futile_White", tris, false, true);
            self.AddToContainer(sLeaser, rCam, null);
        }

        private void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig.Invoke(self, abstractCreature, world);
            if (leaf.TryGet(self, out var flag) && flag)
            {

            }
        }
        private void LoadResources(RainWorld rainWorld)
        {
            // 加载资源
        }
    }
}