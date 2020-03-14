﻿using ReactiveUI;

using System;

namespace ZDevTools.Wpf.ReactiveUI
{
    public class NumberBindingTypeConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(double) || fromType == typeof(int) || fromType == typeof(float))
            {
                if (toType == typeof(decimal))
                    return 1;
            }

            if (fromType == typeof(decimal))
            {
                if (toType == typeof(double) || toType == typeof(int) || toType == typeof(float))
                    return 1;
            }

            return -1;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            if (toType == typeof(decimal))
            {
                result = Convert.ToDecimal(from);
                return true;
            }
            else if (toType == typeof(double) || toType == typeof(int) || toType == typeof(float))
            {
                result = Convert.ChangeType(from, toType);
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
