using HarmonyLib;
using SuperNewRoles.Mode;
using UnityEngine;

namespace SuperNewRoles.Patch
{
    //Town Of Plusより!
    public static class Zoom
    {
        public static void HudUpdate(HudManager __instance)
        {
            if (ModeHandler.IsMode(ModeId.Default) && MapOptions.MapOption.MouseZoom && CachedPlayer.LocalPlayer.Data.IsDead)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    if (Camera.main.orthographicSize > 1.0f)
                    {
                        Camera.main.orthographicSize /= 1.5f;
                        __instance.transform.localScale /= 1.5f;
                        __instance.UICamera.orthographicSize /= 1.5f;
                        HudManager.Instance.TaskStuff.SetActive(false);
                    }
                    else if (Camera.main.orthographicSize > 3.0f)
                    {
                        Camera.main.orthographicSize /= 1.5f;
                        __instance.transform.localScale /= 1.5f;
                        __instance.UICamera.orthographicSize /= 1.5f;
                    }
                }
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        if (Camera.main.orthographicSize < 18.0f)
                        {
                            Camera.main.orthographicSize *= 1.5f;
                            __instance.transform.localScale *= 1.5f;
                            __instance.UICamera.orthographicSize *= 1.5f;
                        }
                    }
                }
                if (ModeHandler.IsMode(ModeId.Default))
                {
                    if (Camera.main.orthographicSize != 3.0f)
                    {
                        HudManager.Instance.TaskStuff.SetActive(false);
                        if (!PlayerControl.LocalPlayer.Data.IsDead) __instance.ShadowQuad.gameObject.SetActive(false);
                    }
                    else
                    {
                        HudManager.Instance.TaskStuff.SetActive(true);
                        if (!PlayerControl.LocalPlayer.Data.IsDead) __instance.ShadowQuad.gameObject.SetActive(true);
                    }
                }
                CreateFlag.NewFlag("Zoom");
            }
            else
            {
                CreateFlag.Run(() =>
                {
                    Camera.main.orthographicSize = 3.0f;
                    HudManager.Instance.UICamera.orthographicSize = 3.0f;
                    HudManager.Instance.transform.localScale = Vector3.one;
                    if (MeetingHud.Instance != null) MeetingHud.Instance.transform.localScale = Vector3.one;
                    HudManager.Instance.Chat.transform.localScale = Vector3.one;
                    if (!PlayerControl.LocalPlayer.Data.IsDead) __instance.ShadowQuad.gameObject.SetActive(true);
                }, "Zoom");
            }
        }
    }
}