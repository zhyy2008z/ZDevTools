using System;
using System.ComponentModel;
using System.Reflection;

namespace ZDevTools.Enums
{
    public static class MyEnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            string objName = enumValue.ToString();

            Type t = enumValue.GetType();

            FieldInfo fi = t.GetField(objName);

            if (fi == null) //枚举值没有对应的枚举字段
                return null;

            DescriptionAttribute[] arrDesc = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return arrDesc.Length > 0 ? arrDesc[0].Description : null;
        }

        public static T GetAttachedAttribute<T>(this Enum enumValue)
            where T : DescriptionAttribute
        {
            string objName = enumValue.ToString();

            Type t = enumValue.GetType();

            FieldInfo fi = t.GetField(objName);

            if (fi == null)
                return null;

            T[] arrDesc = (T[])fi.GetCustomAttributes(typeof(T), false);

            return arrDesc.Length > 0 ? arrDesc[0] : null;
        }
    }
}
