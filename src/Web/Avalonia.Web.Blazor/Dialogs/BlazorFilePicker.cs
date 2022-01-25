using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Controls.Platform.Dialogs;

namespace Avalonia.Web.Blazor.Dialogs
{
    internal class BlazorFilePicker : IFilePicker
    {
        public bool CanOpen => throw new NotImplementedException();

        public bool CanSave => throw new NotImplementedException();

        public bool CanExport => throw new NotImplementedException();

        public Task Export(FilePickerSaveOptions options, Func<Stream, FilePickerFileType, Task> writer)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IFilePickerFile>> OpenAsync(FilePickerOpenOptions options)
        {
            throw new NotImplementedException();
        }

        public IFilePickerBookmark? OpenBookmark(string bookmark)
        {
            throw new NotImplementedException();
        }

        public Task<IFilePickerFile?> SaveAsync(FilePickerSaveOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
