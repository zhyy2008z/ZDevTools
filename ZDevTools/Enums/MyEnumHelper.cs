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
        public static Dictionary<int, string> GetDescriptionDictionary(Type enumType)
        {
            checkIsEnum(enumType);

            Dictionary<int, string> dic = new Dictionary<int, string>();
            FieldInfo[] fieldinfos = enumType.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (objs.Length > 0)
                        dic.Add(((int)field.GetValue(null)), ((DescriptionAttribute)objs[0]).Description);
                }
            }

            return dic;
        }

        /// <summary>
        /// Key 是 枚举值，Value是枚举的描述
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns></returns>
        public static Dictionary<int, T> GetAttachedAttributeDictionary<T>(Type enumType)
            where T : DescriptionAttribute
        {
            checkIsEnum(enumType);


            Dictionary<int, T> dic = new Dictionary<int, T>();
            FieldInfo[] fieldinfos = enumType.GetFields();
            foreach (FieldInfo field in fieldinfos)
            {
                if (field.FieldType.IsEnum)
                {
                    Object[] objs = field.GetCustomAttributes(typeof(T), false);
                    if (objs.Length > 0)
                        dic.Add(((int)field.GetValue(null)), ((T)objs[0]));
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

            return arrDesc.Length > 0 ? arrDesc[0].Description : string.Empty;
        }

    }
}
