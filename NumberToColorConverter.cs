using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace StockTradingRecord
{
    public class NumberToColorConverter : IValueConverter
    {
        // 正向转换：数值 -> 颜色
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 处理空值或非数值类型的情况
            if (value == null || !(value is decimal) && !(value is int))
            {
                // 默认返回黑色
                return Brushes.Black;
            }

            // 将值转换为double类型进行判断
            decimal number = System.Convert.ToDecimal(value);

            if (number > 0)
            {
                // 大于0返回红色
                return Brushes.Red;
            }
            else if (number < 0)
            {
                // 小于0返回绿色
                return Brushes.Green;
            }
            else
            {
                // 等于0返回默认黑色
                return Brushes.Black;
            }
        }

        // 反向转换（这里不需要实现）
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
