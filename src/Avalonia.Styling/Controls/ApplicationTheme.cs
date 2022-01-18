#nullable enable
using System;
using System.ComponentModel;
using System.Globalization;

namespace Avalonia.Controls
{
    [TypeConverter(typeof(ApplicationThemeTypeConverter))]
    public class ApplicationTheme
    {
        public ApplicationTheme(object key)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
        }

        public object Key { get; }

        public static ApplicationTheme Light { get; } = new(nameof(Light));
        public static ApplicationTheme Dark { get; } = new(nameof(Dark));

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is ApplicationTheme theme && Key.Equals(theme.Key);
        }

        public override string ToString()
        {
            return Key.ToString() ?? "ApplicationTheme";
        }
    }

    public class ApplicationThemeTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            return value switch
            {
                nameof(ApplicationTheme.Light) => ApplicationTheme.Light,
                nameof(ApplicationTheme.Dark) => ApplicationTheme.Dark,
                _ => new ApplicationTheme(value)
            };
        }
    }

    public interface IApplicationThemeHost
    {
        ApplicationTheme RequestedTheme { get; set; }

        event Action<ApplicationTheme>? ThemeChanged;
    }
}
