using Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace applecat
{
    public class ApplecatEnums
    {
        public static bool registed;
        public static SlideShow.SlideShowID start_Applecat_1;

        public static void RegisterValues()
        {
            bool flag = ApplecatEnums.registed;
            if (!flag)
            {
                ApplecatEnums.start_Applecat_1 = new SlideShow.SlideShowID("start_Applecat_1", true);
                ApplecatEnums.registed = true;
            }
        }
    }
}
