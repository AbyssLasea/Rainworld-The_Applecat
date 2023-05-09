using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace applecat
{
    public static class DrawMiniAppleHook
    {
        public static ConditionalWeakTable<DangleFruit, MiniApple> modules = new ConditionalWeakTable<DangleFruit, MiniApple>();

        public static void Hook()
        {
            On.DangleFruit.DrawSprites += DangleFruit_To_Apple_DrawSprites;
        }

        public static void DangleFruit_To_Apple_DrawSprites(On.DangleFruit.orig_DrawSprites orig, DangleFruit self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
        {
            orig.Invoke(self,sLeaser,rCam,timeStacker,camPos);
            if (modules.TryGetValue(self,out var miniApple))
            {
                sLeaser.sprites[0].color = miniApple.matureColor;
            }

        }
    }
    
    public class MiniApple
    {
        public Color matureColor;
        public MiniApple(Color color)
        {
            this.matureColor = color;
        }
    }
}
