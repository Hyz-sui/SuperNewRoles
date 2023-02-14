using System.Collections.Generic;
using HarmonyLib;

namespace SuperNewRoles.Sabotage;

public class FixSabotage
{
    /// <summary>
    /// (リアクター, 停電, 通信, 酸素)
    /// </summary>
    /// <remarks>true = 直せる, false = 直せない</remarks>
    private static Dictionary<RoleId, (bool, bool, bool, bool)> SetFixSabotageDictionary = new();
    public static void ClearAndReload()
    {
        SetFixSabotageDictionary = new()
        {
            { RoleId.Fox, (false, false, false, false) },
            { RoleId.FireFox, (false, false, false, false) },
            { RoleId.God, (false,false, false, false) },
            { RoleId.Vampire, (true, false, true, true) },
            { RoleId.Dependents, (true, false, true, true) },
            { RoleId.Madmate, (true, CustomOptionHolder.MadRolesCanFixElectrical.GetBool(), CustomOptionHolder.MadRolesCanFixComms.GetBool(), true) },
        };
    }
    [HarmonyPatch(typeof(Console), nameof(Console.Use))]
    public static class ConsolsUsePatch
    {
        public static bool Prefix(Console __instance)
        {
            if (!(SetFixSabotageDictionary.ContainsKey(PlayerControl.LocalPlayer.GetRole()) || PlayerControl.LocalPlayer.IsMadRoles())) return true;
            __instance.CanUse(PlayerControl.LocalPlayer.Data, out var canUse, out var _);
            if (canUse) return IsBlocked(__instance.FindTask(CachedPlayer.LocalPlayer).TaskType, GetRole(PlayerControl.LocalPlayer.GetRole()));
            return true;
        }
    }
    [HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
    public static class UseButtonSetTargetPatch
    {
        public static bool Prefix(UseButton __instance, [HarmonyArgument(0)] IUsable target)
        {
            if (IsBlocked(target))
            {
                __instance.currentTarget = null;
                __instance.graphic.color = Palette.DisabledClear;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                return false;
            }
            __instance.enabled = true;
            __instance.currentTarget = target;
            return true;
        }
    }
    private static bool IsBlocked(TaskTypes type, RoleId role)
    {
        if (!SetFixSabotageDictionary.ContainsKey(role)) return true;
        (bool, bool, bool, bool) fixSabotage = SetFixSabotageDictionary[role];
        if (type is TaskTypes.StopCharles or TaskTypes.ResetSeismic or TaskTypes.ResetReactor && fixSabotage.Item1) return true;
        if (type is TaskTypes.FixLights && fixSabotage.Item2) return true;
        if (type is TaskTypes.FixComms && fixSabotage.Item3) return true;
        if (type is TaskTypes.RestoreOxy && fixSabotage.Item4) return true;
        return false;
    }
    private static bool IsBlocked(IUsable target)
    {
        if (target == null) return false;
        Console console = target.TryCast<Console>();
        if (console != null && !IsBlocked(console.FindTask(CachedPlayer.LocalPlayer).TaskType, GetRole(PlayerControl.LocalPlayer.GetRole())))
            return true;
        return false;
    }
    private static RoleId GetRole(RoleId role)
    {
        if (SetFixSabotageDictionary.ContainsKey(role)) return role;
        else if (PlayerControl.LocalPlayer.IsMadRoles()) return RoleId.Madmate;
        return role;
    }
}
