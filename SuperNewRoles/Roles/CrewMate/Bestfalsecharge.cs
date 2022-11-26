using HarmonyLib;
using Hazel;
using SuperNewRoles.Achievement;
using SuperNewRoles.Helpers;
using SuperNewRoles.Mode;

namespace SuperNewRoles.Roles
{
    public class Bestfalsecharge
    {
        public static void WrapUp()
        {
            if (ModeHandler.IsMode(ModeId.Default) && !RoleClass.Bestfalsecharge.IsOnMeeting)
            {
                if (PlayerControl.LocalPlayer.IsRole(RoleId.Bestfalsecharge))
                {
                    AchievementManagerSNR.CompleteAchievement(AchievementType.BestFalseChargesExiled);
                }
                if (AmongUsClient.Instance.AmHost)
                {
                    foreach (PlayerControl p in RoleClass.Bestfalsecharge.BestfalsechargePlayer)
                    {
                        p.RpcExiledUnchecked();
                        p.RpcSetFinalStatus(FinalStatus.BestFalseChargesFalseCharge);
                    }
                }
                RoleClass.Bestfalsecharge.IsOnMeeting = true;
            }

            //===========以下さつまいも===========//
            RoleClass.SatsumaAndImo.TeamNumber = RoleClass.SatsumaAndImo.TeamNumber == 1 ? 2 : 1;
        }
    }
}