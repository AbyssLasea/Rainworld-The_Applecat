using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applecat
{
    public class CustomEnding
    {
        public static void PatchOn()
        {
            On.RainWorldGame.GoToRedsGameOver += RainWorldGame_GoToRedsGameOver;
            On.Menu.SlideShow.ctor += SlideShow_ctor;

            On.Menu.MenuScene.BuildScene += MenuScene_BuildScene;
            On.Menu.MenuDepthIllustration.ctor += MenuDepthIllustration_ctor;

            On.Menu.SlugcatSelectMenu.SlugcatPage.AddAltEndingImage += SlugcatPage_AddAltEndingImage;
        }

        private static void SlugcatPage_AddAltEndingImage(On.Menu.SlugcatSelectMenu.SlugcatPage.orig_AddAltEndingImage orig, Menu.SlugcatSelectMenu.SlugcatPage self)
        {
            throw new NotImplementedException();
        }

        private static void MenuDepthIllustration_ctor(On.Menu.MenuDepthIllustration.orig_ctor orig, Menu.MenuDepthIllustration self, Menu.Menu menu, Menu.MenuObject owner, string folderName, string fileName, UnityEngine.Vector2 pos, float depth, Menu.MenuDepthIllustration.MenuShader shader)
        {
            throw new NotImplementedException();
        }

        private static void MenuScene_BuildScene(On.Menu.MenuScene.orig_BuildScene orig, Menu.MenuScene self)
        {
            throw new NotImplementedException();
        }

        private static void SlideShow_ctor(On.Menu.SlideShow.orig_ctor orig, Menu.SlideShow self, ProcessManager manager, Menu.SlideShow.SlideShowID slideShowID)
        {
            throw new NotImplementedException();
        }

        private static void RainWorldGame_GoToRedsGameOver(On.RainWorldGame.orig_GoToRedsGameOver orig, RainWorldGame self)
        {
            bool flag = self.GetStorySession.saveState.saveStateNumber == new global::SlugcatStats.Name(Plugin.DroneMasterName, false);
            if (flag)
            {
                self.manager.rainWorld.progression.currentSaveState.deathPersistentSaveData.altEnding = true;
                bool flag2 = self.manager.upcomingProcess != null;
                if (!flag2)
                {
                    bool flag3 = self.manager.musicPlayer != null;
                    if (flag3)
                    {
                        self.manager.musicPlayer.FadeOutAllSongs(20f);
                    }
                    self.GetStorySession.saveState.SessionEnded(self, true, false);
                    self.manager.statsAfterCredits = true;
                    self.manager.nextSlideshow = ApplecatEnums.DroneMasterAltEnd;
                    self.manager.RequestMainProcessSwitch(global::ProcessManager.ProcessID.SlideShow);
                }
            }
            else
            {
                orig(self);
            }
        }
    }
}
