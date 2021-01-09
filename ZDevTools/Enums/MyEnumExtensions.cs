using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ZDevTools.Enums
{
    /// <summary>
    /// 枚举扩展函数
    /// </summary>
    public static class MyEnumExtensions
    {
        /// <summary>
        /// 获取描述信息
        /// </summary>
        public static string GetDescription(this Enum enumValue)
        {
            string objName = enumValue.ToString();

            Type t = enumValue.GetType();

            FieldInfo fi = t.GetField(objName);

            if (fi == null) //枚举值没有对应的枚举字段
                return null;

            DescriptionAttribute[] descs = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descs.Length > 0)
                return descs[0].Description;

            DisplayAttribute[] displays = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (displays.Length > 0)
                return displays[0].Description;

            return fi.Name;
        }

        /// <summary>
        /// 获取字段显示名称
        /// </summary>
        public static string GetDisplayName(this Enum enumValue)
        {
            string objName = enumValue.ToString();

            Type t = enumValue.GetType();

            FieldInfo fi = t.GetField(objName);

            if (fi == null) //枚举值没有对应的枚举字段
                return null;

            DisplayAttribute[] displays = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (displays.Length > 0)
                return displays[0].Name;

            DescriptionAttribute[] descs = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descs.Length > 0)
                return descs[0].Description;

            return fi.Name;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether UI should be generated automatically in order to display this field.
        /// </summary>
        public static bool GetAutoGenerateField(this Enum enumValue, bool defaultValue)
        {
            string objName = enumValue.ToString();

            Type t = enumValue.GetType();

            FieldInfo fi = t.GetField(objName);

            if (fi == null) //枚举值没有对应的枚举字段
                return defaultValue;

            DisplayAttribute[] displays = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (displays.Length > 0)
                return displays[0].GetAutoGenerateField() ?? defaultValue;

            return defaultValue;
        }

        /// <summary>
        /// 获取DisplayAttribute
        /// </summary>
        public static DisplayAttribute GetDisplay(this Enum enumValue)
        {
            string objName = enumValue.ToString();

            Type t = enumValue.GetType();

            FieldInfo fi = t.GetField(objName);

            if (fi == null)
                return null;

            DisplayAttribute[] arrDesc = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);

            return arrDesc.Length > 0 ? arrDesc[0] : null;
        }

        /// <summary>
        /// 获取描述特性实例
        /// </summary>
        public static T GetAttribute<T>(this Enum enumValue)
            where T : Attribute
        {
            string objName = enumValue.ToString();

            Type t = enumValue.GetType();

            FieldInfo fi = t.GetField(objName);

            if (fi == null)
                return null;

            T[] arrDesc = (T[])fi.GetCustomAttributes(typeof(T), false);

            return arrDesc.Length > 0 ? arrDesc[0] : null;
        }

        /// <summary>
        /// 获取所有特性
        /// </summary>
        public static object[] GetAttributes(this Enum enumValue)
        {
            string objName = enumValue.ToString();

            Type t = enumValue.GetType();

            FieldInfo fi = t.GetField(objName);

            if (fi == null)
                return null;

            return fi.GetCustomAttributes(false);
        }
    }
}
