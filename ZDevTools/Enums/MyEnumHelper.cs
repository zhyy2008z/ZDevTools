using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        /// <summary>
        /// Key 是 枚举值，Value是枚举的描述
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<Enum, string> GetDescriptionDictionary(Type enumType)
        {
            checkIsEnum(enumType);

            Dictionary<Enum, string> dic = new Dictionary<Enum, string>();
            FieldInfo[] fieldinfos = enumType.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    dic.Add((Enum)field.GetValue(null), objs.Length > 0 ? ((DescriptionAttribute)objs[0]).Description : field.Name);
                }
            }

            return dic;
        }

        /// <summary>
        /// Key 是 枚举值，Value是枚举的描述
        /// </summary>
        public static Dictionary<T, string> GetDescriptionDictionary<T>()
            where T : Enum
        {
            Dictionary<T, string> dic = new Dictionary<T, string>();
            FieldInfo[] fieldinfos = typeof(T).GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    dic.Add((T)field.GetValue(null), objs.Length > 0 ? ((DescriptionAttribute)objs[0]).Description : field.Name);
                }
            }
            return dic;
        }

        /// <summary>
        /// Key 是 枚举值，Value是枚举的描述
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<Enum, T> GetAttachedAttributeDictionary<T>(Type enumType)
            where T : DescriptionAttribute
        {
            checkIsEnum(enumType);

            Dictionary<Enum, T> dic = new Dictionary<Enum, T>();
            FieldInfo[] fieldinfos = enumType.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    var objs = field.GetCustomAttributes(typeof(T), false);
                    dic.Add((Enum)field.GetValue(null), objs.Length > 0 ? ((T)objs[0]) : null);
                }
            }
            return dic;
        }

        /// <summary>
        /// Key 是 枚举值，Value是枚举的描述
        /// </summary>
        public static Dictionary<TEnum, TDescriptionAttribute> GetAttachedAttributeDictionary<TEnum, TDescriptionAttribute>()
            where TDescriptionAttribute : DescriptionAttribute
            where TEnum : Enum
        {
            Dictionary<TEnum, TDescriptionAttribute> dic = new Dictionary<TEnum, TDescriptionAttribute>();
            FieldInfo[] fieldinfos = typeof(TEnum).GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    var objs = field.GetCustomAttributes(typeof(TDescriptionAttribute), false);
                    dic.Add((TEnum)field.GetValue(null), objs.Length > 0 ? ((TDescriptionAttribute)objs[0]) : null);
                }
            }
            return dic;
        }

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
