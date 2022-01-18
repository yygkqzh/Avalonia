using System;
using Avalonia.Markup.Xaml.Styling;
#nullable enable

namespace Avalonia.Themes.Default
{
    public class SimpleTheme : StyleInclude
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTheme"/> class.
        /// </summary>
        /// <param name="baseUri">The base URL for the XAML context.</param>
        public SimpleTheme(Uri baseUri) : base(baseUri)
        {
            Source = new Uri("avares://Avalonia.Themes.Default/DefaultTheme.xaml");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleTheme"/> class.
        /// </summary>
        /// <param name="serviceProvider">The XAML service provider.</param>
        public SimpleTheme(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Source = new Uri("avares://Avalonia.Themes.Default/DefaultTheme.xaml");
        }
    }
}
