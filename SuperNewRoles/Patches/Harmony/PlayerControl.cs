using HarmonyLib;
using SuperNewRoles.Roles;
using SuperNewRoles.Roles.Crewmate;
using SuperNewRoles.Roles.Impostor;
using SuperNewRoles.Roles.Impostor.MadRole;
using SuperNewRoles.Roles.Neutral;
using SuperNewRoles.Roles.Neutral.FriendRoles;

namespace SuperNewRoles.Patches.Harmony;

//キルされたとき実行！
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
class PlayerControl_MurderPlayer
{
    private static void Postfix(PlayerControl __instance)
    {
        var MyRole = PlayerControl.LocalPlayer.GetRole();
        Jackal.JackalFixedPatch.Postfix(__instance, MyRole);
        JackalSeer.Postfix(__instance, MyRole);
    }
}