#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace Avalonia.Controls.Platform.Dialogs
{
    public interface IFilePickerFile
    {
        string FileName { get; }
        bool TryGetFullPath([NotNullWhen(true)] out string? path);

        Stream Stream { get; }
        
        bool CanBookmark { get; }
        Task<string?> SaveBookmark();
    }
}
