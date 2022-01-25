#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Avalonia.Controls.Platform.Dialogs
{
    public record FilePickerProperties(long Size);

    public interface IFilePickerFile
    {
        FilePickerProperties Properties { get; }

        string FileName { get; }
        bool TryGetFullPath([NotNullWhen(true)] out string? path);

        bool CanBookmark { get; }
        Task<string?> SaveBookmark();
    }

    public interface IOpenFilePickerFile
    {
        Task<Stream> OpenRead();
    }

    public interface ISaveFilePickerFile
    {
        Task<Stream> OpenWrite();
    }
}
