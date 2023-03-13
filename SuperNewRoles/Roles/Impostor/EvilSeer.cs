using System;
using System.Collections.Generic;
using SuperNewRoles.Mode;
using SuperNewRoles.ReplayManager;
using SuperNewRoles.Roles.RoleBases;
using UnityEngine;
using static SuperNewRoles.Roles.Crewmate.Seer;
using static SuperNewRoles.Modules.CustomOption;

namespace SuperNewRoles.Roles.Impostor;

public class EvilSeer : RoleBase<EvilSeer>
{
    public static Color color = RoleClass.ImpostorRed;

    public EvilSeer()
    {
        RoleId = roleId = RoleId.EvilSeer;
        //以下いるもののみ変更
        OptionId = 335;
        HasTask = false;
        IsSHRRole = true;
        OptionType = CustomOptionType.Impostor;
    }

    public override void OnMeetingStart() { }
    public override void OnWrapUp()
    {
        List<Vector3> DeadBodyPositions = new();
        bool limitSoulDuration = false;
        float soulDuration = 0f;

        DeadBodyPositions = deadBodyPositions;
        deadBodyPositions = new List<Vector3>();
        limitSoulDuration = EvilSeerLimitSoulDuration.GetBool();
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
        deadBodyPositions = new();
        IsCreateMadmate = EvilSeerMadmateSetting.GetBool();
    }
    public override void UseAbility() { base.UseAbility(); AbilityLimit--; if (AbilityLimit <= 0) EndUseAbility(); }
    public override bool CanUseAbility() { return base.CanUseAbility() && AbilityLimit <= 0; }

    //ボタンが必要な場合のみ(Buttonsの方に記述する必要あり)
    public static void MakeButtons(HudManager hm) { }
    public static void SetButtonCooldowns() { }

    // CustomOption Start
    public static CustomOption EvilSeerMode;
    public static CustomOption EvilSeerLimitSoulDuration;
    public static CustomOption EvilSeerSoulDuration;
    public static CustomOption EvilSeerMadmateSetting;
    public override void SetupMyOptions()
    {
        EvilSeerMode = Create(OptionId, false, CustomOptionType.Impostor, "SeerMode", new string[] { "SeerModeBoth", "SeerModeFlash", "SeerModeSouls" }, RoleOption); OptionId++;
        EvilSeerLimitSoulDuration = Create(OptionId, false, CustomOptionType.Impostor, "SeerLimitSoulDuration", false, RoleOption); OptionId++;
        EvilSeerSoulDuration = Create(OptionId, false, CustomOptionType.Impostor, "SeerSoulDuration", 15f, 0f, 120f, 5f, EvilSeerLimitSoulDuration, format: "unitCouples"); OptionId++;
        EvilSeerMadmateSetting = Create(1092, false, CustomOptionType.Impostor, "CreateMadmateSetting", false, RoleOption);
    }
    // CustomOption End

    // RoleClass Start
    public List<Vector3> deadBodyPositions
    {
        get { return ReplayData.CanReplayCheckPlayerView ? GetValueVector3("_deadBodyPositions") : _deadBodyPositions; }
        set { if (ReplayData.CanReplayCheckPlayerView) SetValueVector3("_deadBodyPositions", value); else _deadBodyPositions = value; }
    }
    private List<Vector3> _deadBodyPositions;
    public static int mode;

    public static bool IsImpostorCheck;
    public static int ImpostorCheckTask;
    public bool IsCreateMadmate
    {
        get { return ReplayData.CanReplayCheckPlayerView ? GetValueBool("_IsCreateMadmate") : _IsCreateMadmate; }
        set { if (ReplayData.CanReplayCheckPlayerView) SetValueBool("_IsCreateMadmate", value); else _IsCreateMadmate = value; }
    }
    private bool _IsCreateMadmate;

    public static void Clear()
    {
        players = new();
        mode = ModeHandler.IsMode(ModeId.SuperHostRoles) ? 1 : EvilSeerMode.GetSelection();
    }

    // RoleClass End
}