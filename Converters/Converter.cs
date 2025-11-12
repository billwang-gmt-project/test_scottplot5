using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// Must Add new converter to Converters.xaml
namespace Converters
{
    public class Converter_XYP2_Name : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (bool)value ? "XP2" : "YP2";

            }
            catch (Exception)
            {
                return "XP2";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberToHexString : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value == null) return "??";

            string format = "X8";
            if (parameter != null)
            {
                format = (string)parameter;
            }
            try
            {
                Type type = value.GetType();
                if (type == typeof(byte))
                {
                    return ((byte)value).ToString(format);
                }
                if (type == typeof(ushort))
                {
                    return ((ushort)value).ToString(format);
                }
                else if (type == typeof(int))
                {
                    return ((int)value).ToString(format);
                }
                else if (type == typeof(uint))
                {
                    return ((uint)value).ToString(format);
                }
                else
                {
                    return "??";
                }
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var temp = uint.Parse(value.ToString()!.Replace("0x", ""), NumberStyles.HexNumber);
                return temp;

            }
            catch (Exception)
            {
                return value;
            }
        }
    }

    public class BoolInverterConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool v)
            {
                return !v;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool v)
            {
                return !v;
            }
            return value;
        }

        #endregion
    }
    public class BoolToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1 : 0;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                bool isTrue = intValue > 0;
                //                LogHelper.AddInfoLog($"IntToBoolConverter: {intValue} => {isTrue}");
                return isTrue;
            }
            return false;
        }
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isInvert = false;
            if (parameter != null && ((string)parameter).ToLower() == "invert")
            {
                isInvert = true;
            }

            if (value is bool boolValue)
            {
                if (isInvert)
                {
                    //LogHelper.AddInfoLog($"BoolToVisibilityConverter: value ={value} invert={isInvert} => {(boolValue ? Visibility.Collapsed : Visibility.Visible)}");
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    //LogHelper.AddInfoLog($"BoolToVisibilityConverter: value ={value} invert={isInvert} => {(boolValue ? Visibility.Visible : Visibility.Collapsed)}");
                    return boolValue ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DecimalToN3 : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Math.Round((decimal)value, 3);
            }
            catch (Exception)
            {
                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (decimal.TryParse(value.ToString(), out decimal temp))
                    return Math.Round(temp, 3);
                else
                    return value;
            }
            catch (Exception)
            {
                return value;
            }
        }
    }

    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue.ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (decimal.TryParse(stringValue, out decimal decimalValue))
                {
                    return decimalValue;
                }
            }
            return 0m;
        }
    }

    /// <summary>
    /// if value == parameter, return true, else return false
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class ItemToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((string)parameter == (string)value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// When value== parameter, return Visible, else return Collapsed
    /// </summary>    
    public class ItemToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return "Collapsed";
            }
            if ((string)parameter == (string)value)
            {
                return "Visible";
            }
            else
            {
                return "Collapsed";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// - if item is null, return Collapsed             
    /// - if item is not null
    ///     if item.count == 0, return Collapsed
    ///     if item.count > 0, return Visible
    /// - if parameter is not null and is equal to "invert", revert the return value            
    /// </summary>
    public class ItemListToVisibilityConverter : IValueConverter
    {
        /* 
             if item is null, return Collapsed             
             if item is not null
                 if item.count == 0, return Collapsed
                 if item.count > 0, return Visible
             if parameter is not null and is equal to "invert", revert the return value
        */
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isInvert = false;
            if (parameter != null && ((string)parameter).ToLower() == "invert")
            {
                isInvert = true;
            }
            if (value == null)
            {
                return (isInvert) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (value is IList list)
            {
                if (list.Count == 0)
                {
                    return (isInvert) ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return (isInvert) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///  if value is null, return Visible
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NotNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = value == null ? Visibility.Collapsed : Visibility.Visible;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                bool isTrue = intValue > 0;
                //                LogHelper.AddInfoLog($"IntToBoolConverter: {intValue} => {isTrue}");
                return isTrue;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1 : 0;
            }
            return 0;
        }
    }

    public class IndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var item = values[0];
            var list = values[1] as IList;
            if (list != null)
            {
                return list.IndexOf(item).ToString();
            }
            return -1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HexFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //LogHelper.AddInfoLog($"HexFormatConverter: {values[0].GetType()} {values[0]}");
            if (values[1] is bool isDecimal)
            {
                if (isDecimal)
                {
                    if (float.TryParse(values[0].ToString(), out float floatValue))
                    {
                        FloatUnion floatUnion = new FloatUnion();
                        floatUnion.floatValue = floatValue;
                        return floatUnion.uintValue.ToString("X8");
                    }
                    else
                    {
                        return "????????";
                    }
                }
                else
                {
                    if (uint.TryParse(values[0].ToString(), out uint uintValue))
                    {
                        return uintValue.ToString("X2");
                    }
                    else
                    {
                        return "??";
                    }
                }
            }
            else
            {
                if (uint.TryParse(values[0].ToString(), out uint uintValue))
                {
                    return uintValue.ToString("X2");
                }
                else
                {
                    return "??";
                }
            }
        }

        public object[] ConvertBack(object? value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Convert hex string to uint or float by length
            if (value != null)
            {
                string hexString = value.ToString()!.Replace("0x", "");
                try
                {
                    uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
                    if (hexString.Length == 8)
                    {
                        byte[] floatBytes = BitConverter.GetBytes(num);
                        float f = BitConverter.ToSingle(floatBytes, 0);
                        return new object[] { f, Binding.DoNothing };
                    }
                    else
                    {
                        return new object[] { num, Binding.DoNothing };
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show($"Invalid Hex Value: {value}");
                    return new object[] { Binding.DoNothing, Binding.DoNothing };
                }
            }
            else
                return new object[] { Binding.DoNothing, Binding.DoNothing };
        }
    }


    public class DoubleToUintConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return 0;
            }
            if (double.TryParse(value.ToString(), out double doubleValue))
            {
                return doubleValue;
            }
            //if (value is uint uintValue)
            //{
            //    return uintValue;
            //}
            //else if (value is float floatValue)
            //{
            //    return (uint)floatValue;
            //}
            //else if (value is double doubleValue)
            //{
            //    return (uint)doubleValue;
            //}
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is double doubleValue)
            {
                if (doubleValue >= 0)
                    return (uint)doubleValue;
                else
                    return doubleValue;
            }
            return DependencyProperty.UnsetValue;
            //if (value is uint uintValue)
            //{
            //    return (double)uintValue;
            //}
            //return DependencyProperty.UnsetValue;
        }
    }

    public class NumberFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            //LogHelper.AddInfoLog($"NumberFormatConverter: {values[0].GetType()} {values[0]}");
            if (values[1] is bool isDecimal)
            {
                if (isDecimal)
                {
                    // Convert float to string
                    if (float.TryParse(values[0].ToString(), out float floatValue))
                    {
                        if (values[2] is string floatFormat)
                        {
                            return floatValue.ToString(floatFormat);
                        }
                        else
                        {
                            return floatValue.ToString("F3");
                        }
                    }
                    else
                    {
                        return "???";
                    }
                }
                else
                {
                    // Convert int to string
                    if (int.TryParse(values[0].ToString(), out int intValue))
                    {
                        if (values[2] is string intFormat)
                        {
                            return intValue.ToString(intFormat);
                        }
                        else
                        {
                            return intValue.ToString();
                        }
                    }
                    else
                    {
                        return "???";
                    }
                }
            }
            else
            {
                return "???";
            }

            //        if (values[0] is float floatNumber && values[1] is string floatFormat)
            //{
            //    return floatNumber.ToString(floatFormat);
            //}
            //else if (values[0] is uint uintNumber && values[1] is string uintFormat)
            //{
            //    return uintNumber.ToString(uintFormat);
            //}
            //else if (values[0] is int intNumber && values[1] is string intFormat)
            //{
            //    return intNumber.ToString(intFormat);
            //}
            //else
            //{
            //    return string.Empty;
            //}
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                if (uint.TryParse(stringValue, out uint uintNumber))
                {
                    return new object[] { uintNumber, Binding.DoNothing };
                }
                else if (float.TryParse(stringValue, out float floatNumber))
                {
                    return new object[] { floatNumber, Binding.DoNothing };
                }

                else
                {
                    return new object[] { Binding.DoNothing, Binding.DoNothing };
                }
            }
            else
            {
                return new object[] { Binding.DoNothing, Binding.DoNothing };
            }
        }
    }

    public class SliderFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(values[0].ToString(), out double doubleNumber))
            {
                return doubleNumber;
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            //LogHelper.AddInfoLog($"SliderFormatConverterBack: {value.GetType()}");

            // Convert double to int

            //if (value is double doubleNumber)
            //{
            //    decimal decimalNumber = System.Convert.ToDecimal(doubleNumber);
            //    decimalNumber = Math.Round(decimalNumber, 6);
            //    return new object[] { (float)decimalNumber, Binding.DoNothing };
            //}
            //else if (value is int intNumber)
            //{
            //    return new object[] { intNumber, Binding.DoNothing };
            //}
            //else if (value is uint uintNumber)
            //{
            //    return new object[] { (int)uintNumber, Binding.DoNothing };
            //}
            //else
            //{
            //    return new object[] { Binding.DoNothing, Binding.DoNothing };
            //}

            //LogHelper.AddInfoLog($"SliderFormatConverterBack: {value.GetType()}");
            int intNumber = System.Convert.ToInt32(value);
            return new object[] { intNumber, Binding.DoNothing };
        }
    }

    public class IntToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((int)value)
            {
                case 0: return "Green";
                case 1: return "Blue";
                case 2: return "Red";
                default: return "Black";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    //public class HexFormatConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is uint uintValue)
    //        {
    //            if (uintValue == 0xFFFFFFFF)
    //            {
    //                return uintValue.ToString("X8");
    //            }
    //            else
    //            {
    //                return uintValue.ToString("X2");
    //            }
    //        }
    //        return value.ToString();
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is string stringValue)
    //        {
    //            if (uint.TryParse(stringValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint uintValue))
    //            {
    //                return uintValue;
    //            }
    //        }
    //        return 0;
    //    }
    //}

}
