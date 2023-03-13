using System;
using System.Collections.Generic;
using SuperNewRoles.Mode;
using SuperNewRoles.ReplayManager;
using SuperNewRoles.Patches;
using SuperNewRoles.Roles.Crewmate;
using SuperNewRoles.Roles.RoleBases;
using UnityEngine;
using static SuperNewRoles.Roles.Crewmate.Seer;
using static SuperNewRoles.Modules.CustomOption;
using static SuperNewRoles.Modules.CustomOptionHolder;

namespace SuperNewRoles.Roles.Impostor.MadRole;

public class MadSeer : RoleBase<MadSeer>
{
    public static Color color = RoleClass.ImpostorRed;

    public MadSeer()
    {
        RoleId = roleId = RoleId.MadSeer;
        //以下いるもののみ変更
        OptionId = 323;
        IsSHRRole = true;
        OptionType = CustomOptionType.Crewmate;
        CanUseVentOptionOn = true;
        CanUseVentOptionDefault = false;
        IsImpostorViewOptionOn = true;
        IsImpostorViewOptionDefault = false;
        CoolTimeOptionOn = true;
    }

    public override void OnMeetingStart() { }
    public override void OnWrapUp()
    {
        List<Vector3> DeadBodyPositions = new();
        bool limitSoulDuration = false;
        float soulDuration = 0f;

        DeadBodyPositions = deadBodyPositions;
        deadBodyPositions = new List<Vector3>();
        limitSoulDuration = MadSeerLimitSoulDuration.GetBool();
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
    public override void PostInit() { deadBodyPositions = new(); }
    public override void UseAbility() { base.UseAbility(); AbilityLimit--; if (AbilityLimit <= 0) EndUseAbility(); }
    public override bool CanUseAbility() { return base.CanUseAbility() && AbilityLimit <= 0; }

    //ボタンが必要な場合のみ(Buttonsの方に記述する必要あり)
    public static void MakeButtons(HudManager hm) { }
    public static void SetButtonCooldowns() { }

    // CustomOption Start
    public static CustomOption MadSeerMode;
    public static CustomOption MadSeerLimitSoulDuration;
    public static CustomOption MadSeerSoulDuration;
    public static CustomOption MadSeerIsCheckImpostor;
    public static CustomOption MadSeerCommonTask;
    public static CustomOption MadSeerShortTask;
    public static CustomOption MadSeerLongTask;
    public static CustomOption MadSeerCheckImpostorTask;
    public override void SetupMyOptions()
    {
        MadSeerMode = Create(OptionId, false, CustomOptionType.Crewmate, "SeerMode", new string[] { "SeerModeBoth", "SeerModeFlash", "SeerModeSouls" }, RoleOption); OptionId++;
        MadSeerLimitSoulDuration = Create(OptionId, false, CustomOptionType.Crewmate, "SeerLimitSoulDuration", false, RoleOption); OptionId++;
        MadSeerSoulDuration = Create(OptionId, false, CustomOptionType.Crewmate, "SeerSoulDuration", 15f, 0f, 120f, 5f, MadSeerLimitSoulDuration, format: "unitCouples");
        MadSeerIsCheckImpostor = CustomOption.Create(OptionId, false, CustomOptionType.Crewmate, "MadmateIsCheckImpostorSetting", false, RoleOption); OptionId++;
        var MadSeeroption = SelectTask.TaskSetting(OptionId, OptionId + 1, OptionId + 2, MadSeerIsCheckImpostor, CustomOptionType.Crewmate, true); OptionId += 3;
        MadSeerCommonTask = MadSeeroption.Item1;
        MadSeerShortTask = MadSeeroption.Item2;
        MadSeerLongTask = MadSeeroption.Item3;
        MadSeerCheckImpostorTask = CustomOption.Create(OptionId, false, CustomOptionType.Crewmate, "MadmateCheckImpostorTaskSetting", rates4, MadSeerIsCheckImpostor);
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

    public static void Clear()
    {
        players = new();
        mode = ModeHandler.IsMode(ModeId.SuperHostRoles) ? 1 : MadSeerMode.GetSelection();
    }

    // RoleClass End
}