using SuperNewRoles.Roles;
using SuperNewRoles.Roles.Crewmate;
using SuperNewRoles.Roles.Impostor;
using SuperNewRoles.Roles.Impostor.MadRole;
using SuperNewRoles.Roles.Neutral;

namespace SuperNewRoles.Mode.SuperHostRoles;

public static class RoleHelper
{
    public static bool IsCrewVision(this PlayerControl player)
    {
        var IsCrewVision = false;
        switch (player.GetRole())
        {
            case RoleId.Sheriff:
            case RoleId.truelover:
            case RoleId.FalseCharges:
            case RoleId.RemoteSheriff:
            case RoleId.Arsonist:
            case RoleId.ToiletFan:
            case RoleId.NiceButtoner:
            case RoleId.Worshiper when !Worshiper.IsImpostorViewS:
                IsCrewVision = true;
                break;
                //クルー視界か
        }
        return IsCrewVision;
    }
    public static bool IsImpostorVision(this PlayerControl player)
    {
        return player.GetRole() switch
        {
            RoleId.Madmate => RoleClass.Madmate.IsImpostorLight,
            RoleId.MadMayor => RoleClass.MadMayor.IsImpostorLight,
            RoleId.MadStuntMan => RoleClass.MadStuntMan.IsImpostorLight,
            RoleId.MadJester => RoleClass.MadJester.IsImpostorLight,
            RoleId.JackalFriends => RoleClass.JackalFriends.IsImpostorLight,
            RoleId.Fox => Fox.IsImpostorViewOption.GetBool(),
            RoleId.MayorFriends => RoleClass.MayorFriends.IsImpostorLight,
            RoleId.BlackCat => RoleClass.BlackCat.IsImpostorLight,
            RoleId.MadSeer => MadSeer.IsImpostorViewS,
            RoleId.SeerFriends => RoleClass.SeerFriends.IsImpostorLight,
            _ => false,
        };
    }
    public static bool IsZeroCoolEngineer(this PlayerControl player)
    {
        var IsZeroCoolEngineer = false;
        switch (player.GetRole())
        {
            case RoleId.Technician:
                IsZeroCoolEngineer = true;
                break;
            case RoleId.Jester:
                return Jester.CanUseVentS;
            case RoleId.Madmate:
                return RoleClass.Madmate.IsUseVent;
            case RoleId.MadMayor:
                return RoleClass.MadMayor.IsUseVent;
            case RoleId.MadStuntMan:
                return RoleClass.MadStuntMan.IsUseVent;
            case RoleId.MadJester:
                return RoleClass.MadJester.IsUseVent;
            case RoleId.JackalFriends:
                return RoleClass.JackalFriends.IsUseVent;
            case RoleId.Fox:
                return Fox.CanUseVentOption.GetBool();
            case RoleId.MayorFriends:
                return RoleClass.MayorFriends.IsUseVent;
            case RoleId.Tuna:
                return RoleClass.Tuna.IsUseVent;
            case RoleId.BlackCat:
                return RoleClass.BlackCat.IsUseVent;
            case RoleId.Spy:
                return RoleClass.Spy.CanUseVent;
            case RoleId.Arsonist:
                return RoleClass.Arsonist.IsUseVent;
            case RoleId.MadSeer:
                return MadSeer.CanUseVentS;
            case RoleId.SeerFriends:
                return RoleClass.SeerFriends.IsUseVent;
                //ベント無限か
        }
        return IsZeroCoolEngineer;
    }
}