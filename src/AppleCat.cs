using UnityEngine;

namespace applecat
{
    internal class AppleCat
    {
        public int swallowAndRegurgitateCounter = 0;
        public Color appleColor;
        private float timeStacker = 0f;

        public AppleCat()
        {
        }
        // 玩家受光合作用影响
        public static int lightTime = 0;//用来计数收获时间
        public void WhenInLight(Player self, bool eu, int second)
        {
            if (!eu) { return; }
            //活没活着
            bool flag1 = self.Consious;
            var room = self.room;
            Vector2 pos = self.bodyChunks[0].pos;
            var rCam = self.room.game.cameras[0];
            bool flag2 = Detectlight(rCam, pos, self);// 如果该点没有被阴影覆盖和能够看到天空
            //如果活着就执行
            if (flag1)
            {
                if (flag2)//晒太阳就有特效
                {
                    self.room.AddObject(new ExplosionSpikes(self.room, self.mainBodyChunk.pos, 1, 30, 2, 10, 10, Color.white));
                }
                //如果单单有光就加1计数器,如果还有阳光就加10
                lightTime += flag2 ? 10 : 1;
                //设置光能收集满的数值
                int eatTime = (self.playerState.foodInStomach >= self.slugcatStats.maxFood) ? second * 4000 : second * 400;
                //如果光能大于一定值数就得到饱腹度
                if (lightTime > eatTime)
                {
                    lightTime = 0;
                    self.AddQuarterFood();
                    self.AddQuarterFood();
                }
            }
            else
            {
                //如果不晒阳光就让光能慢慢恢复
                lightTime -= lightTime > 0 ? 15 : 0;
            }
        }
        // 判断是否处于光照区域内
        public bool Detectlight(RoomCamera rCam, Vector2 pos, Player self)
        {
            var room = self.room;
            if (room.VisualContact(pos, pos + 2000 * Vector2.up) == true // 如果能够看到天空
                && room.DarknessOfPoint(rCam, pos) == 0) // 如果该点没有被阴影覆盖
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
