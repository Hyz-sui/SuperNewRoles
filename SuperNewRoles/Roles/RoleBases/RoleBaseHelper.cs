using System;
using System.Collections.Generic;
using System.Text;
using SuperNewRoles.Roles;
using SuperNewRoles.Roles.Crewmate;
using SuperNewRoles.Roles.Impostor;
using SuperNewRoles.Roles.Impostor.MadRole;
using SuperNewRoles.Roles.Neutral;

namespace SuperNewRoles.Roles.RoleBases;
public static class RoleBaseHelper
{
    public static Dictionary<RoleId, Type> allRoleIds = new()
        {
            // Impostor
            { RoleId.EvilScientist, typeof(RoleBase<EvilScientist>) },
            { RoleId.EvilSeer, typeof(RoleBase<EvilSeer>) },

            // Neutral
            { RoleId.Jester, typeof(RoleBase<Jester>) },
            { RoleId.FireFox, typeof(RoleBase<FireFox>) },
            { RoleId.Fox, typeof(RoleBase<Fox>) },

            // Crew
            { RoleId.SoothSayer, typeof(RoleBase<SoothSayer>) },
            { RoleId.Lighter, typeof(RoleBase<Lighter>) },
            { RoleId.Sheriff, typeof(RoleBase<Sheriff>) },
            { RoleId.RemoteSheriff, typeof(RoleBase<RemoteSheriff>) },
            { RoleId.Seer, typeof(RoleBase<Seer>) },

            //MadRoles
            { RoleId.Worshiper, typeof(RoleBase<Worshiper>) },
            { RoleId.MadSeer, typeof(RoleBase<MadSeer>) },

            //FriendsRoles

            // Other
        };
    public static void SetUpOptions()
    {
        // Impostor
        new EvilScientist().SetUpOption();
        new EvilSeer().SetUpOption();

        // Neutral
        new Jester().SetUpOption();
        new FireFox().SetUpOption();
        new Fox().SetUpOption();

        // Crew
        new Sheriff().SetUpOption();
        new RemoteSheriff().SetUpOption();
        new Worshiper().SetUpOption();
        new MadSeer().SetUpOption();
        new SoothSayer().SetUpOption();
        new Lighter().SetUpOption();
        new Seer().SetUpOption();

        // Other

    }
}