#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Avalonia.Controls.Platform.Dialogs
{
    public interface IFilePicker
    {
        bool CanOpen { get; }
        Task<IReadOnlyList<IFilePickerFile>> OpenAsync(FilePickerOpenOptions options);

        bool CanSave { get; }
        Task<IFilePickerFile?> SaveAsync(FilePickerSaveOptions options);

        bool CanExport { get; }
        Task Export(FilePickerSaveOptions options, Func<Stream, FilePickerFileType, Task> writer);
        IFilePickerBookmark? OpenBookmark(string bookmark);
    }
}
