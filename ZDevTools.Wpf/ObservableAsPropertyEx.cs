using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Fody.Helpers
{
    /// <summary>
    /// 针对ReactiveUI.Fody项目补充的扩展函数
    /// </summary>
    public static class ObservableAsPropertyEx
    {
        /// <summary>
        /// 为可观察属性设置默认值
        /// </summary>
        public static ObservableAsPropertyHelper<TRet> ToPropertyDefaultEx<TObj, TRet>(this TObj source, Expression<Func<TObj, TRet>> property, TRet initialValue = default, IScheduler scheduler = null) where TObj : ReactiveObject
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            var observableAsPropertyHelper = ObservableAsPropertyHelper<TRet>.Default(initialValue, scheduler);

            PropertyInfo propertyInfo = getPropertyInfo(property);
            if (propertyInfo == null)
            {
                throw new Exception("Could not resolve expression " + property?.ToString() + " into a property.");
            }

            FieldInfo declaredField = propertyInfo.DeclaringType.GetTypeInfo().GetDeclaredField("$" + propertyInfo.Name);
            if (declaredField == null)
            {
                throw new Exception("Backing field not found for " + propertyInfo?.ToString());
            }

            declaredField.SetValue(source, observableAsPropertyHelper);
            return observableAsPropertyHelper;
        }

        static PropertyInfo getPropertyInfo(LambdaExpression expression)
        {
            Expression expression2 = expression.Body;
            UnaryExpression unaryExpression = expression2 as UnaryExpression;
            if (unaryExpression != null)
            {
                expression2 = unaryExpression.Operand;
            }

            return (PropertyInfo)((MemberExpression)expression2).Member;
        }
    }
}
