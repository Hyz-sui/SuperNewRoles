using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SuperNewRoles.Patches.CursedTasks;

public class CursedLeafTask
{
    public static int LeavesNum;
    public static int LeafDoneCount;
    [HarmonyPatch(typeof(LeafMinigame))]
    public static class LeafMinigamePatch
    {
        [HarmonyPatch(nameof(LeafMinigame.Begin)), HarmonyPostfix]
        public static void BeginPostfix(LeafMinigame __instance)
        {
            if (!Main.IsCursed) return;
            if (LeafDoneCount <= 0)
            {
                LeavesNum = Main.Num;
                __instance.MyNormTask.taskStep = 0;
                __instance.MyNormTask.MaxStep = LeavesNum;
                LeafDoneCount = LeavesNum;
                Transform TaskParent = __instance.transform.parent;
                for (int i = 0; i < TaskParent.childCount; i++)
                {
                    Transform child = TaskParent.GetChild(i);
                    if (child.name == "o2_leaf1(Clone)") Object.Destroy(child);
                }

                __instance.Leaves = new Collider2D[LeavesNum];
                for (int i = 0; i < LeavesNum; i++)
                {
                    LeafBehaviour leafBehaviour = Object.Instantiate(__instance.LeafPrefab);
                    leafBehaviour.transform.SetParent(__instance.transform);
                    leafBehaviour.Parent = __instance;
                    Vector2 localPosition = __instance.ValidArea.Next();
                    leafBehaviour.transform.localPosition = new Vector3(localPosition.x, localPosition.y, -1);
                    __instance.Leaves[i] = leafBehaviour.GetComponent<Collider2D>();
                }
            }

            GameObject pointer = new("cursor");
            pointer.transform.SetParent(__instance.transform);
            pointer.layer = 4;
            CircleCollider2D collider2D = pointer.AddComponent<CircleCollider2D>();
            collider2D.radius = 0.5f;
        }

        [HarmonyPatch(nameof(LeafMinigame.FixedUpdate)), HarmonyPostfix]
        public static void FixedUpdatePostfix(LeafMinigame __instance)
        {
            if (!Main.IsCursed) return;
            __instance.transform.FindChild("cursor").position = __instance.myController.HoverPosition;
        }
    }
}
