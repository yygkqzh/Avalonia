#nullable enable
using System.Collections.Generic;

namespace Avalonia.Controls.Platform.Dialogs
{
    public class FilePickerFileType
    {
        public FilePickerFileType(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public IReadOnlyList<string>? Extensions { get; init; }
        // For web
        public IReadOnlyList<string>? MimeTypes { get; init; }
        // For Apple platforms
        public IReadOnlyList<string>? AppleUniformTypeIdentifiers { get; init; }
    }
}
