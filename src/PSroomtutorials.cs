using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreSlugcats;

namespace applecat
{
    internal class PSroomtutorials
    {
        public static void AddRoomSpecificScript(Room room)
        {
            bool flag = !ModManager.MSC || (room.game.session is StoryGameSession && (room.game.session as StoryGameSession).saveState.cycleNumber < 2 && room.game.GetStorySession.saveState.saveStateNumber != MoreSlugcatsEnums.SlugcatStatsName.Spear);
            string name = room.abstractRoom.name;
            if (name == "SU_A53")
            {
                if (room.game.session is StoryGameSession && !(room.game.session as StoryGameSession).saveState.regionStates[room.world.region.regionNumber].roomsVisited.Contains
                    (room.abstractRoom.name) && (room.game.session as StoryGameSession).saveState.regionStates[room.world.region.regionNumber].roomsVisited.Contains("SU_A53"))
                {
                    room.AddObject(new SU_A53PS(room));
                }
            }
        }
        public class SU_A53PS : UpdatableAndDeletable
        {
            public SU_A53PS(Room room)
            {

            }
        }
    }
}
