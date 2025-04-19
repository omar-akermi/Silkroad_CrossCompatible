using System;

#if IL2CPP
using Il2CppScheduleOne.Economy;
using Il2CppScheduleOne.Persistence.Datas;
#else
using ScheduleOne.Economy;
using ScheduleOne.Persistence.Datas;
#endif
namespace S1API.API
{
    [Serializable]
    public class DeadDropData : SaveData
    {
        public string deliverDeadDropGUID;
        public string collectDeadDropGUID;

        public DeadDrop DeliveryDeadDrop
        {
            get
            {
                if (string.IsNullOrEmpty(deliverDeadDropGUID)) return null;

#if IL2CPP
                for (int i = 0; i < DeadDrop.DeadDrops.Count; i++)
                {
                    var drop = DeadDrop.DeadDrops[i];
                    if (drop != null && drop.GUID.ToString() == deliverDeadDropGUID)
                        return drop;
                }
#else
                foreach (var drop in DeadDrop.DeadDrops)
                {
                    if (drop != null && drop.GUID.ToString() == deliverDeadDropGUID)
                        return drop;
                }
#endif
                return null;
            }
        }

        public DeadDrop CollectDeadDrop
        {
            get
            {
                if (string.IsNullOrEmpty(collectDeadDropGUID)) return null;

#if IL2CPP
                for (int i = 0; i < DeadDrop.DeadDrops.Count; i++)
                {
                    var drop = DeadDrop.DeadDrops[i];
                    if (drop != null && drop.GUID.ToString() == collectDeadDropGUID)
                        return drop;
                }
#else
                foreach (var drop in DeadDrop.DeadDrops)
                {
                    if (drop != null && drop.GUID.ToString() == collectDeadDropGUID)
                        return drop;
                }
#endif
                return null;
            }
        }
    }
}
