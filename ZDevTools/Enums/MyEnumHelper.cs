using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ZDevTools.Enums
{
    /// <summary>
    /// 枚举辅助类
    /// </summary>
    public static class MyEnumHelper
    {
        static void checkIsEnum(Type type)
        {
            if (!type.IsEnum)
                throw new InvalidOperationException("该类型对象不是枚举类型！");
        }

        ///// <summary>
        ///// Key 是 枚举值，Value是枚举的描述
        ///// </summary>
        ///// <param name="enumType">枚举类型</param>
        ///// <returns></returns>
        //public static Dictionary<Enum, string> GetDescriptionDictionary(Type enumType)
        //{
        //    checkIsEnum(enumType);

        //    Dictionary<Enum, string> dic = new Dictionary<Enum, string>();
        //    FieldInfo[] fieldinfos = enumType.GetFields();
        //    foreach (FieldInfo field in fieldinfos)
        //    {
        //        if (field.FieldType.IsEnum)
        //        {
        //            DescriptionAttribute[] descs = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //            var key = (Enum)field.GetValue(null);
        //            if (descs.Length > 0)
        //                dic.Add(key, descs[0].Description);
        //            else
        //            {
        //                var displays = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
        //                if (displays.Length > 0)
        //                    dic.Add(key, displays[0].Description);
        //                else
        //                    dic.Add(key, field.Name);
        //            }
        //        }
        //    }

        //    return dic;
        //}

        ///// <summary>
        ///// Key 是 枚举值，Value是枚举的描述
        ///// </summary>
        //public static Dictionary<T, string> GetDescriptionDictionary<T>()
        //    where T : Enum
        //{
        //    Dictionary<T, string> dic = new Dictionary<T, string>();
        //    FieldInfo[] fieldinfos = typeof(T).GetFields();
        //    foreach (FieldInfo field in fieldinfos)
        //    {
        //        if (field.FieldType.IsEnum)
        //        {
        //            DescriptionAttribute[] descs = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //            var key = (T)field.GetValue(null);
        //            if (descs.Length > 0)
        //                dic.Add(key, descs[0].Description);
        //            else
        //            {
        //                var displays = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
        //                if (displays.Length > 0)
        //                    dic.Add(key, displays[0].Description);
        //                else
        //                    dic.Add(key, field.Name);
        //            }
        //        }
        //    }
        //    return dic;
        //}

        ///// <summary>
        ///// Key 是 枚举值，Value是枚举的描述
        ///// </summary>
        ///// <param name="enumType">枚举类型</param>
        ///// <returns></returns>
        //public static Dictionary<Enum, string> GetDisplayNameDictionary(Type enumType)
        //{
        //    checkIsEnum(enumType);

        //    Dictionary<Enum, string> dic = new Dictionary<Enum, string>();
        //    FieldInfo[] fieldinfos = enumType.GetFields();
        //    foreach (FieldInfo field in fieldinfos)
        //    {
        //        if (field.FieldType.IsEnum)
        //        {
        //            var key = (Enum)field.GetValue(null);

        //            var displays = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
        //            if (displays.Length > 0)
        //                dic.Add(key, displays[0].Name);
        //            else
        //            {
        //                DescriptionAttribute[] descs = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //                if (descs.Length > 0)
        //                    dic.Add(key, descs[0].Description);
        //                else
        //                    dic.Add(key, field.Name);
        //            }
        //        }
        //    }

        //    return dic;
        //}

        ///// <summary>
        ///// Key 是 枚举值，Value是枚举的描述
        ///// </summary>
        //public static Dictionary<T, string> GetDisplayNameDictionary<T>()
        //    where T : Enum
        //{
        //    Dictionary<T, string> dic = new Dictionary<T, string>();
        //    FieldInfo[] fieldinfos = typeof(T).GetFields();
        //    foreach (FieldInfo field in fieldinfos)
        //    {
        //        if (field.FieldType.IsEnum)
        //        {
        //            var key = (T)field.GetValue(null);

        //            var displays = (DisplayAttribute[])field.GetCustomAttributes(typeof(DisplayAttribute), false);
        //            if (displays.Length > 0)
        //                dic.Add(key, displays[0].Name);
        //            else
        //            {
        //                DescriptionAttribute[] descs = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        //                if (descs.Length > 0)
        //                    dic.Add(key, descs[0].Description);
        //                else
        //                    dic.Add(key, field.Name);
        //            }
        //        }
        //    }
        //    return dic;
        //}

        ///// <summary>
        ///// Key 是 枚举值，Value是枚举的描述
        ///// </summary>
        ///// <param name="enumType">枚举类型</param>
        ///// <returns></returns>
        //public static Dictionary<Enum, T> GetAttachedAttributeDictionary<T>(Type enumType)
        //    where T : Attribute
        //{
        //    checkIsEnum(enumType);

        //    Dictionary<Enum, T> dic = new Dictionary<Enum, T>();
        //    FieldInfo[] fieldinfos = enumType.GetFields();
        //    foreach (FieldInfo field in fieldinfos)
        //    {
        //        if (field.FieldType.IsEnum)
        //        {
        //            var objs = field.GetCustomAttributes(typeof(T), false);
        //            dic.Add((Enum)field.GetValue(null), objs.Length > 0 ? ((T)objs[0]) : null);
        //        }
        //    }
        //    return dic;
        //}

        ///// <summary>
        ///// Key 是 枚举值，Value是枚举的描述
        ///// </summary>
        //public static Dictionary<TEnum, TAttribute> GetAttachedAttributeDictionary<TEnum, TAttribute>()
        //    where TAttribute : Attribute
        //    where TEnum : Enum
        //{
        //    Dictionary<TEnum, TAttribute> dic = new Dictionary<TEnum, TAttribute>();
        //    FieldInfo[] fieldinfos = typeof(TEnum).GetFields();
        //    foreach (FieldInfo field in fieldinfos)
        //    {
        //        if (field.FieldType.IsEnum)
        //        {
        //            var objs = field.GetCustomAttributes(typeof(TAttribute), false);
        //            dic.Add((TEnum)field.GetValue(null), objs.Length > 0 ? ((TAttribute)objs[0]) : null);
        //        }
        //    }
        //    return dic;
        //}

        /// <summary>
        /// 获取枚举类型的描述信息
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static string GetDescription(Type enumType)
        {
            checkIsEnum(enumType);

            DescriptionAttribute[] arrDesc = (DescriptionAttribute[])enumType.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return arrDesc.Length > 0 ? arrDesc[0].Description : enumType.Name;
        }

    }
}
