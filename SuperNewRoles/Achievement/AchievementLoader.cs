using System;
using System.IO;
using System.Text;

namespace SuperNewRoles.Achievement
{
    public static class AchievementLoader
    {
        public static void OnLoad()
        {
            string AppDataLocalPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (!Directory.Exists($"{AppDataLocalPath}/SuperNewRoles"))
                Directory.CreateDirectory($"{AppDataLocalPath}/SuperNewRoles");
            if (!File.Exists($"{AppDataLocalPath}/SuperNewRoles/AchievementData.txt"))
                File.Create($"{AppDataLocalPath}/SuperNewRoles/AchievementData.txt");
            string data;
            using (StreamReader sr = new($"{AppDataLocalPath}/SuperNewRoles/AchievementData.txt"))
            {
                data = sr.ReadToEnd();
            }
            data = data.Replace("\r", "");
            ObjectCreate(data);
        }
        public static void ObjectCreate(string data)
        {
            AchievementManager.AllAchievementData = new();
            AchievementManager.currentData = data;
            Logger.Info(AchievementManager.currentData);
            foreach (AchievementType type in Enum.GetValues(typeof(AchievementType)))
            {
                new AchievementData(type);
            }
        }
    }
}
