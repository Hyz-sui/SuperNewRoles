using System;
using System.Collections.Generic;
using AmongUs.GameOptions;
using SuperNewRoles.Mode;
using SuperNewRoles.ReplayManager;
using SuperNewRoles.Patches;
using SuperNewRoles.Roles.RoleBases;
using UnityEngine;
using static SuperNewRoles.Roles.Crewmate.Seer;
using static SuperNewRoles.Modules.CustomOption;
using static SuperNewRoles.Modules.CustomOptionHolder;

namespace SuperNewRoles.Roles.Neutral.FriendRoles;

public class SeerFriends : RoleBase<SeerFriends>
{
    public static Color color = RoleClass.JackalBlue;

    public SeerFriends()
    {
        RoleId = roleId = RoleId.SeerFriends;
        //以下いるもののみ変更
        OptionId = 362;
        IsSHRRole = true;
        OptionType = CustomOptionType.Crewmate;
        CanUseVentOptionOn = true;
        CanUseVentOptionDefault = false;
        IsImpostorViewOptionOn = true;
        IsImpostorViewOptionDefault = false;
    }

    public override void OnMeetingStart() { }
    public override void OnWrapUp()
    {
        List<Vector3> DeadBodyPositions = new();
        bool limitSoulDuration = false;
        float soulDuration = 0f;

        DeadBodyPositions = deadBodyPositions;
        deadBodyPositions = new List<Vector3>();
        limitSoulDuration = SeerFriendsLimitSoulDuration.GetBool();
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
    public static CustomOption SeerFriendsMode;
    public static CustomOption SeerFriendsLimitSoulDuration;
    public static CustomOption SeerFriendsSoulDuration;
    public static CustomOption SeerFriendsIsCheckJackal;
    public static CustomOption SeerFriendsCommonTask;
    public static CustomOption SeerFriendsShortTask;
    public static CustomOption SeerFriendsLongTask;
    public static CustomOption SeerFriendsCheckJackalTask;
    public override void SetupMyOptions()
    {
        SeerFriendsMode = Create(OptionId, false, CustomOptionType.Crewmate, "SeerMode", new string[] { "SeerModeBoth", "SeerModeFlash", "SeerModeSouls" }, RoleOption); OptionId++;
        SeerFriendsLimitSoulDuration = Create(OptionId, false, CustomOptionType.Crewmate, "SeerLimitSoulDuration", false, RoleOption); OptionId++;
        SeerFriendsSoulDuration = Create(OptionId, false, CustomOptionType.Crewmate, "SeerSoulDuration", 15f, 0f, 120f, 5f, SeerFriendsLimitSoulDuration, format: "unitCouples"); OptionId++;
        SeerFriendsIsCheckJackal = CustomOption.Create(OptionId, false, CustomOptionType.Crewmate, "JackalFriendsIsCheckJackalSetting", false, RoleOption); OptionId++;
        var SeerFriendsoption = SelectTask.TaskSetting(OptionId, OptionId + 1, OptionId + 2, SeerFriendsIsCheckJackal, CustomOptionType.Crewmate, true); OptionId += 3;
        SeerFriendsCommonTask = SeerFriendsoption.Item1;
        SeerFriendsShortTask = SeerFriendsoption.Item2;
        SeerFriendsLongTask = SeerFriendsoption.Item3;
        SeerFriendsCheckJackalTask = CustomOption.Create(OptionId, false, CustomOptionType.Crewmate, "MadmateCheckImpostorTaskSetting", rates4, SeerFriendsIsCheckJackal);

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

    public static int JackalCheckTask;

    public static void Clear()
    {
        players = new();
        mode = ModeHandler.IsMode(ModeId.SuperHostRoles) ? 1 : SeerFriendsMode.GetSelection();

            int Common = SeerFriendsCommonTask.GetInt();
            int Long = SeerFriendsLongTask.GetInt();
            int Short = SeerFriendsShortTask.GetInt();
            int AllTask = Common + Long + Short;
            if (AllTask == 0)
            {
                Common = GameOptionsManager.Instance.CurrentGameOptions.GetInt(Int32OptionNames.NumCommonTasks);
                Long = GameOptionsManager.Instance.CurrentGameOptions.GetInt(Int32OptionNames.NumLongTasks);
                Short = GameOptionsManager.Instance.CurrentGameOptions.GetInt(Int32OptionNames.NumShortTasks);
            }
            JackalCheckTask = (int)(AllTask * (int.Parse(SeerFriendsCheckJackalTask.GetString().Replace("%", "")) / 100f));
    }

    // RoleClass End
}