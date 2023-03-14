using HarmonyLib;
using SuperNewRoles.Roles;
using SuperNewRoles.Roles.Crewmate;
using SuperNewRoles.Roles.Impostor;
using SuperNewRoles.Roles.Impostor.MadRole;
using SuperNewRoles.Roles.Neutral;
using SuperNewRoles.Roles.Neutral.FriendRoles;

namespace SuperNewRoles.Patches.Harmony;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
class ExilerController_WrapUp
{
    private static void Postfix(PlayerControl __instance)
    {
        var MyRole = PlayerControl.LocalPlayer.GetRole();
        Jackal.JackalFixedPatch.Postfix(__instance, MyRole);
        JackalSeer.Postfix(__instance, MyRole);
    }
}