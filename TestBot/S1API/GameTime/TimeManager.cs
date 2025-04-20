#if (IL2CPP)
using S1GameTime = Il2CppScheduleOne.GameTime;
#elif (MONO)
using S1GameTime = ScheduleOne.GameTime;
#endif

using System;

namespace S1API.GameTime
{
    /// <summary>
    /// Provides access to various time management functions in the game.
    /// </summary>
    public static class TimeManager
    {
        /// <summary>
        /// Action called when the day passes in-game.
        /// </summary>
        public static Action OnDayPass = delegate { };
        
        /// <summary>
        /// The current in-game day.
        /// </summary>
        public static Day CurrentDay => 
            (Day)S1GameTime.TimeManager.Instance.CurrentDay;
    }
}