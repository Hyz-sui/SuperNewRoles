using System.Collections.Generic;
using UnityEngine;
using SuperNewRoles.Patches;

namespace SuperNewRoles.Roles
{
    public partial class Madmate
    {
        public static List<PlayerControl> Player;
        public static Color32 color = RoleClass.ImpostorRed;

        public static CustomOption Option;
        public static CustomOption Count;
        public static CustomOption Par;

        public static void SetupCustomOptions()
        {
            int id= 1010;
            Option = CustomOption.Create(id, true, CustomOptionType.Crewmate, CustomOptionHolder.Cs(color, "MadmateName"), false, null, isHeader: true);
            Count = CustomOption.Create(id, true, CustomOptionType.Crewmate, "MadmateCountSetting", 1f, 1f, 15f, 1f, Option);
            Par = CustomOption.Create(id, true, CustomOptionType.Crewmate, "MadmateParSetting", CustomOptionHolder.rates, Option);
        }

        public static void ClearAndReload() {
            Player = new();
        }
    }
}