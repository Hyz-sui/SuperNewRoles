using Hazel;
using System;
using AmongUs.GameOptions;
using SuperNewRoles.Buttons;
using SuperNewRoles.Helpers;
using System.Collections.Generic;
using SuperNewRoles.Mode;
using SuperNewRoles.ReplayManager;
using SuperNewRoles.Roles.RoleBases;
using UnityEngine;
using static SuperNewRoles.Patches.PlayerControlFixedUpdatePatch;
using static SuperNewRoles.Roles.Crewmate.Seer;
using static SuperNewRoles.Modules.CustomOption;
using static SuperNewRoles.Buttons.HudManagerStartPatch;

namespace SuperNewRoles.Roles.Neutral;

public class JackalSeer : RoleBase<JackalSeer>
{
    public static Color color = RoleClass.JackalBlue;

    public JackalSeer()
    {
        RoleId = roleId = RoleId.JackalSeer;
        //以下いるもののみ変更
        HasTask = false;
        IsKiller = true;
        OptionId = 375;
        IsSHRRole = true;
        OptionType = CustomOptionType.Neutral;
        CanUseVentOptionOn = true;
        CanUseSaboOptionOn = true;
        IsImpostorViewOptionOn = true;
        CoolTimeOptionOn = true;
    }

    public override void OnMeetingStart() { }
    public override void OnWrapUp()
    {
        AllButtonCooldown();

        List<Vector3> DeadBodyPositions = new();
        bool limitSoulDuration = false;
        float soulDuration = 0f;

        DeadBodyPositions = DeadBodyPositions_Replay;
        DeadBodyPositions_Replay = new List<Vector3>();
        limitSoulDuration = JackalSeerLimitSoulDuration.GetBool();
        soulDuration = SeerSoulDuration.GetFloat();
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
        CanCreateSidekick = JackalSeerCreateSidekick.GetBool();
        CanCreateFriend = JackalSeerCreateFriend.GetBool();
    }
    public override void UseAbility() { base.UseAbility(); AbilityLimit--; if (AbilityLimit <= 0) EndUseAbility(); }
    public override bool CanUseAbility() { return base.CanUseAbility() && AbilityLimit <= 0; }

    public static void JackalSeerPlayerOutLineTarget() =>
        SetPlayerOutline(JackalSetTarget(), RoleClass.JackalBlue);

    public static void Postfix(PlayerControl __instance, RoleId role)
    {
        if (AmongUsClient.Instance.AmHost)
        {
            if (SidekickSeer.allPlayers.Count > 0)
            {
                var upflag = true;
                foreach (PlayerControl p in allPlayers)
                {
                    if (p.IsAlive())
                    {
                        upflag = false;
                    }
                }
                if (upflag)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SidekickPromotes, SendOption.Reliable, -1);
                    writer.Write(true);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.SidekickPromotes(true);
                }
            }
        }
        if (role == RoleId.JackalSeer)
        {
            JackalSeerPlayerOutLineTarget();
        }
    }

    //ボタンが必要な場合のみ(Buttonsの方に記述する必要あり)
    public static CustomButton JackalSeerSidekickButton;
    public static void MakeButtons(HudManager hm)
    {
        JackalSeerSidekickButton = new(
            () =>
            {
                var target = JackalSetTarget();
                if (target && RoleHelpers.IsAlive(PlayerControl.LocalPlayer) && PlayerControl.LocalPlayer.CanMove && JackalSeer.local.CanCreateSidekick)
                {
                    if (target.IsRole(RoleId.SideKiller)) // サイドキック相手がマッドキラーの場合
                    {
                        if (!RoleClass.SideKiller.IsUpMadKiller) // サイドキラーが未昇格の場合
                        {
                            var sidePlayer = RoleClass.SideKiller.GetSidePlayer(target); // targetのサイドキラーを取得
                            if (sidePlayer != null) // null(作っていない)ならば処理しない
                            {
                                sidePlayer.RPCSetRoleUnchecked(RoleTypes.Impostor);
                                RoleClass.SideKiller.IsUpMadKiller = true;
                            }
                        }
                    }
                    if (local.CanCreateFriend)
                    {
                        Jackal.CreateJackalFriends(target); //クルーにして フレンズにする
                    }
                    else
                    {
                        bool IsFakeSidekickSeer = EvilEraser.IsBlockAndTryUse(EvilEraser.BlockTypes.JackalSeerSidekick, target);
                        MessageWriter killWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CreateSidekickSeer, SendOption.Reliable, -1);
                        killWriter.Write(target.PlayerId);
                        killWriter.Write(IsFakeSidekickSeer);
                        AmongUsClient.Instance.FinishRpcImmediately(killWriter);
                        RPCProcedure.CreateSidekickSeer(target.PlayerId, IsFakeSidekickSeer);
                    }
                    local.CanCreateSidekick = false;
                }
            },
            (bool isAlive, RoleId role) => { return isAlive && role == RoleId.JackalSeer && ModeHandler.IsMode(ModeId.Default) && JackalSeer.local.CanCreateSidekick; },
            () =>
            {
                return JackalSetTarget() && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                if (PlayerControl.LocalPlayer.IsRole(RoleId.JackalSeer)) { JackalSeer.SidekickButtonCooldown(); }
            },
            RoleClass.Jackal.GetButtonSprite(),
            new Vector3(-2f, 1, 0),
            hm,
            hm.AbilityButton,
            KeyCode.F,
            49,
            () => { return false; }
        )
        {
            buttonText = ModTranslation.GetString("JackalCreateSidekickButtonName"),
            showButtonText = true
        };

    }

    public static void AllButtonCooldown()
    {
        ResetKillCooldown();
        SidekickButtonCooldown();
    }
    public static void ResetKillCooldown()
    {
        JackalKillButton.MaxTimer = CoolTimeS;
        JackalKillButton.Timer = JackalKillButton.MaxTimer;
    }
    public static void SidekickButtonCooldown()
    {
        JackalSeerSidekickButton.MaxTimer = JackalSeerSKCooldown.GetFloat();
        JackalSeerSidekickButton.Timer = JackalSeerSKCooldown.GetFloat();
    }

    // CustomOption Start
    public static CustomOption JackalSeerMode;
    public static CustomOption JackalSeerLimitSoulDuration;
    public static CustomOption JackalSeerSoulDuration;
    public static CustomOption JackalSeerCreateSidekick;
    public static CustomOption JackalSeerCreateFriend;
    public static CustomOption JackalSeerSKCooldown;
    public static CustomOption JackalSeerNewJackalCreateSidekick;
    public override void SetupMyOptions()
    {
        JackalSeerMode = Create(OptionId, false, OptionType, "SeerMode", new string[] { "SeerModeBoth", "SeerModeFlash", "SeerModeSouls" }, RoleOption); OptionId++;
        JackalSeerLimitSoulDuration = Create(OptionId, false, OptionType, "SeerLimitSoulDuration", false, RoleOption); OptionId++;
        JackalSeerSoulDuration = Create(OptionId, false, OptionType, "SeerSoulDuration", 15f, 0f, 120f, 5f, JackalSeerLimitSoulDuration, format: "unitCouples"); OptionId++;
        JackalSeerCreateSidekick = Create(OptionId, false, OptionType, "JackalCreateSidekickSetting", false, RoleOption); OptionId++;
        JackalSeerCreateFriend = Create(OptionId, true, OptionType, "JackalCreateFriendSetting", false, RoleOption); OptionId++;
        JackalSeerSKCooldown = Create(OptionId, false, OptionType, "PavlovsownerCreateDogCoolTime", 30f, 2.5f, 60f, 2.5f, JackalSeerCreateSidekick, format: "unitSeconds"); OptionId++;
        JackalSeerNewJackalCreateSidekick = Create(OptionId, false, OptionType, "JackalNewJackalCreateSidekickSetting", false, JackalSeerCreateSidekick);
    }
    // CustomOption End

    // RoleClass Start
    public static List<int> CreatePlayers;
    public List<Vector3> DeadBodyPositions_Replay
    {
        get { return ReplayData.CanReplayCheckPlayerView ? GetValueVector3("_deadBodyPositions_Replay") : _deadBodyPositions_Replay; }
        set { if (ReplayData.CanReplayCheckPlayerView) SetValueVector3("_deadBodyPositions_Replay", value); else _deadBodyPositions_Replay = value; }
    }
    private List<Vector3> _deadBodyPositions_Replay;
    public static int mode;

    public bool CanCreateSidekick
    {
        get { return ReplayData.CanReplayCheckPlayerView ? GetValueBool("_CanCreateSidekick") : _CanCreateSidekick; }
        set { if (ReplayData.CanReplayCheckPlayerView) SetValueBool("_CanCreateSidekick", value); else _CanCreateSidekick = value; }
    }
    public bool CanCreateFriend
    {
        get { return ReplayData.CanReplayCheckPlayerView ? GetValueBool("_CanCreateFriend") : _CanCreateFriend; }
        set { if (ReplayData.CanReplayCheckPlayerView) SetValueBool("_CanCreateFriend", value); else _CanCreateFriend = value; }
    }
    private bool _CanCreateSidekick;
    private bool _CanCreateFriend;

    public static void Clear()
    {
        players = new();
        mode = ModeHandler.IsMode(ModeId.SuperHostRoles) ? 1 : JackalSeerMode.GetSelection();
        CreatePlayers = new();
    }

    // RoleClass End
}