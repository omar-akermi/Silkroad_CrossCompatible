﻿#if (IL2CPP)
using S1Levelling = Il2CppScheduleOne.Levelling;
#elif (MONO)
using S1Levelling = ScheduleOne.Levelling;
#endif

namespace S1API.Leveling
{
    /// <summary>
    /// Allows management of the level system.
    /// </summary>
    public static class LevelManager
    {
        /// <summary>
        /// The current rank of the save file.
        /// </summary>
        public static Rank Rank = (Rank)S1Levelling.LevelManager.Instance.Rank;
    }
}