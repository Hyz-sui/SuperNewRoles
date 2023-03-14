using System;
using System.Collections.Generic;
using SuperNewRoles.Mode;
using SuperNewRoles.ReplayManager;
using SuperNewRoles.Roles.RoleBases;
using UnityEngine;
using static SuperNewRoles.Roles.Crewmate.Seer;
using static SuperNewRoles.Modules.CustomOption;

namespace SuperNewRoles.Roles.Neutral;

public class SidekickSeer : RoleBase<SidekickSeer>
{
    public static Color color = RoleClass.JackalBlue;

    public SidekickSeer()
    {
        RoleId = roleId = RoleId.SidekickSeer;
        //以下いるもののみ変更
        OptionId = 393;
        IsAssignRoleFirst = false;
        HasTask = false;
        OptionType = CustomOptionType.Neutral;
    }

    public override void OnMeetingStart() { }
    public override void OnWrapUp()
    {
        List<Vector3> DeadBodyPositions = new();
        bool limitSoulDuration = false;
        float soulDuration = 0f;

        DeadBodyPositions = DeadBodyPositions_Replay;
        DeadBodyPositions_Replay = new List<Vector3>();
        limitSoulDuration = JackalSeer.JackalSeerLimitSoulDuration.GetBool();
        soulDuration = JackalSeer.JackalSeerSoulDuration.GetFloat();
        if (mode is not 0 and not 2) return;

        foreach (Vector3 pos in DeadBodyPositions)
        {
            GameObject soul = new();
            soul.transform.position = pos;
            soul.layer = 5;
            var rend = soul.AddComponent<SpriteRenderer>();
            rend.sprite = GetSoulSprite();

            if (limitSoulDuration)
            {
                FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(soulDuration, new Action<float>((p) =>
                {
                    if (rend != null)
                    {
                        var tmp = rend.color;
                        tmp.a = Mathf.Clamp01(1 - p);
                        rend.color = tmp;
                    }
                    if (p == 1f && rend != null && rend.gameObject != null) UnityEngine.Object.Destroy(rend.gameObject);
                })));
            }
        }
    }
    public override void FixedUpdate() { }
    public override void MeFixedUpdateAlive() { }
    public override void MeFixedUpdateDead() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void EndUseAbility() { }
    public override void ResetRole() { }
    public override void PostInit()
    {
        DeadBodyPositions_Replay = new();
    }
    public override void UseAbility() { base.UseAbility(); AbilityLimit--; if (AbilityLimit <= 0) EndUseAbility(); }
    public override bool CanUseAbility() { return base.CanUseAbility() && AbilityLimit <= 0; }

    //ボタンが必要な場合のみ(Buttonsの方に記述する必要あり)
    public static void MakeButtons(HudManager hm) { }
    public static void SetButtonCooldowns() { }

    // CustomOption Start
    public override void SetupMyOptions() { }
    // CustomOption End

    // RoleClass Start
    public List<Vector3> DeadBodyPositions_Replay
    {
        get { return ReplayData.CanReplayCheckPlayerView ? GetValueVector3("_deadBodyPositions_Replay") : _deadBodyPositions_Replay; }
        set { if (ReplayData.CanReplayCheckPlayerView) SetValueVector3("_deadBodyPositions_Replay", value); else _deadBodyPositions_Replay = value; }
    }
    private List<Vector3> _deadBodyPositions_Replay;
    public static int mode;
    public static List<PlayerControl> FakeSidekickSeerPlayer;

    public static void Clear()
    {
        players = new();
        FakeSidekickSeerPlayer = new();
        mode = ModeHandler.IsMode(ModeId.SuperHostRoles) ? 1 : JackalSeer.JackalSeerMode.GetSelection();
    }

    public static explicit operator SidekickSeer(PlayerControl v)
    {
        throw new NotImplementedException();
    }

    // RoleClass End
}