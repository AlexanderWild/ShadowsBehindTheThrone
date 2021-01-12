using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code
{
    class AchievementManager
    {
        public static bool[] hasAchieved;

        public static void setup()
        {
            hasAchieved = new bool[SteamManager.achievementKeys.Length];
        }

        public static void unlockAchievement(SteamManager.achievement_key key)
        {
            if (hasAchieved != null)
            {
                if (hasAchieved[(int)key]) { return; }
                hasAchieved[(int)(key)] = true;
            }
            if (SteamManager.s_instance != null && (SteamManager.apiShutdown == false))
            {
                SteamManager.unlockAchievement(key);
            }
        }
    }
}
