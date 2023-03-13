using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SuperNewRoles.DataManage
{
    public static class RoleDataManager
    {
        public static Dictionary<string, byte> ByteValues;
        public static Dictionary<string, bool> BoolValues;
        public static Dictionary<string, float> FloatValues;
        public static Dictionary<string, List<Vector3>> Vector3Values;
        public static void SetValueByte(string name, byte value) => ByteValues[name] = value;
        public static void SetValueBool(string name, bool value) => BoolValues[name] = value;
        public static void SetValueFloat(string name, float value) => FloatValues[name] = value;
        public static List<Vector3> SetValueVector3(string name, List<Vector3> value) => Vector3Values[name] = value;
        public static float GetValueFloat(string name) => FloatValues[name];
        public static byte GetValueByte(string name) => ByteValues[name];
        public static bool GetValueBool(string name) => BoolValues[name];
        public static List<Vector3> GetValueVector3(string name) => Vector3Values[name];

        public static void ClearAndReloads()
        {
            ByteValues = new();
            BoolValues = new();
            FloatValues = new();
            Vector3Values = new();
        }
    }
}
