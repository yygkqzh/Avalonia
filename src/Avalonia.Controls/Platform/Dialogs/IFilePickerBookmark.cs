#nullable enable
using System.IO;
using System.Threading.Tasks;

namespace Avalonia.Controls.Platform.Dialogs
{
    public interface IFilePickerBookmark
    {
        bool CanOpenRead { get; }
        bool CanOpenWrite { get; }
        Task<Stream> OpenRead();
        Task<Stream> OpenWrite();
        Task Release();
        Task<bool> RequestPermissions();
    }
}
