﻿using System;
using System.Globalization;
using XFiler.SDK;

namespace XFiler
{
    public class CloneParamsConverter : BaseMultiValueConverter
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            => values.Clone();
    }
}