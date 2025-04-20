namespace S1API.Quests
{
    /// <summary>
    /// Represents all states a quest can be. Applicable to quest entries as well.
    /// </summary>
    public enum QuestState
    {
        Inactive,
        Active,
        Completed,
        Failed,
        Expired,
        Cancelled
    }
}