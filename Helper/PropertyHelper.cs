using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
    public class PropertyHelper
    {
        /// <summary>
        /// Gets the name of the property.
        /// Usage:
        /// var propertyName = PropertyHelper.GetPropertyName(() => variable.Name);
        /// writer.WriteLine($"- {propertyName}: \"{variable.Name}\"");
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Expression is not a member access - propertyExpression</exception>
        public static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }
            throw new ArgumentException("Expression is not a member access", nameof(propertyExpression));
        }

        public static void SetProperty(object obj, string propertyName, string propertyValue)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property != null)
            {
                if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(obj, bool.Parse(propertyValue));
                }
                else if (property.PropertyType == typeof(int))
                {
                    property.SetValue(obj, int.Parse(propertyValue));
                }
                else if (property.PropertyType == typeof(uint))
                {
                    property.SetValue(obj, uint.Parse(propertyValue));
                }
                else if (property.PropertyType == typeof(string))
                {
                    property.SetValue(obj, propertyValue);
                }
                else if (property.PropertyType == typeof(List<string>))
                {
                    // Handle List<string> property type parsing here if needed
                }
                // Add more property types as needed
            }
        }
    }
}
