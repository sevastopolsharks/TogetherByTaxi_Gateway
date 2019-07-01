using System;
using System.Reflection;
using JetBrains.Annotations;

namespace SevSharks.Gateway.WebApi.Models
{
    /// <summary>
    /// Расширения для работы с рефлексией
    /// </summary>
    [UsedImplicitly]
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Возвращает значение из MemberInfo если это PropertyInfo или FieldInfo
        /// </summary>
        /// <returns></returns>
        public static T GetValue<T>(this MemberInfo member, object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (member is PropertyInfo property)
            {
                return (T)property.GetValue(instance);
            }

            if (member is FieldInfo field)
            {
                return (T)field.GetValue(instance);
            }

            throw new ArgumentException("Не возможно получить значение от не свойства или поля", nameof(member));
        }

        /// <summary>
        /// Задает значение через MemberInfo если это PropertyInfo или FieldInfo
        /// </summary>
        public static void SetValue(this MemberInfo member, object instance, object value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (member is PropertyInfo property)
            {
                property.SetValue(instance, value);
                return;
            }

            if (member is FieldInfo field)
            {
                field.SetValue(instance, value);
                return;
            }

            throw new ArgumentException("Присвоение значение возможно только для свойств или полей", nameof(member));
        }
    }
}
