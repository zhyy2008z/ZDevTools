using System;
using ReactiveUI;

namespace ZDevTools.WindowsForms.ReactiveUI
{
    public class EnumBindingTypeConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType.IsSubclassOf(typeof(Enum)) && (toType == typeof(int)))
                return 1;
            else if ((fromType == typeof(int)) && toType.IsSubclassOf(typeof(Enum)))
            {
                return 1;
            }
            return -1;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            if ((toType == typeof(int)))
            {
                result = Convert.ToInt32(from);
                return true;
            }
            else if (toType.IsSubclassOf(typeof(Enum)))
            {
                result = Enum.ToObject(toType, from);
                return true;
            }

            result = null;
            return false;
        }
    }
}
