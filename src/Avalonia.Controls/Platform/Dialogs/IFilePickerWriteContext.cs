#nullable enable

using System.IO;

namespace Avalonia.Controls.Platform.Dialogs
{
    public interface IFilePickerWriteContext
    {
        Stream Stream { get; }
        FilePickerFileType FileType { get; }
    }
}
