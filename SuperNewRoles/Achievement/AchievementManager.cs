using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SuperNewRoles.Achievement
{
    class AchievementManagerSNR
    {
        public static List<AchievementData> AllAchievementData = new();
        public static List<AchievementData> CompletedAchievement = new();
        public static List<AchievementData> WaitCompleteData = new();
        public static Dictionary<byte, int> PlayerData = new();
        public static string currentData;
        public static AchievementData SelectedData {
            get
            {
                return _selectedData;
            }
            set
            {
                _selectedData = value;
                ConfigRoles.AchievementSelectedId.Value = _selectedData.Id;
            }
        }
        public static AchievementData _selectedData;
        public static AchievementData GetAchievementData(AchievementType type)
        {
            return AllAchievementData.FirstOrDefault(x => x.TypeData == type);
        }
        public static void CompleteAchievement(AchievementType type, bool EndGameComplete = true)
        {
            AchievementData data = GetAchievementData(type);
            if (data is null) return;
            if (data.Complete) return;
            if (EndGameComplete)
            {
                WaitCompleteData.Add(data);
                return;
            }
            currentData += $"{data.Id}\n";

            string AppDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            using (StreamWriter sw = new($"{AppDataLocalPath}/SuperNewRoles/AchievementData.txt", false))
            {
                sw.Write(currentData);
            }
            CompletedAchievement.Add(data);
            data.Complete = true;
            Logger.Info($"クリア{data.Name}");
        }
        public static List<string> GetCompletedTitle()
        {
            List<string> data = new();
            foreach (AchievementData adata in CompletedAchievement)
            {
                data.Add(adata.Title);
            }
            return data;
        }
        public static AchievementData GetPlayerData(PlayerControl player)
        {
            if (player == null) return null;
            return PlayerData.ContainsKey(player.PlayerId) ? GetAchievementData((AchievementType)PlayerData[player.PlayerId]) : null;
        }
    }
}
