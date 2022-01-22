using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Avalonia.Controls.Platform.Dialogs;

namespace Avalonia.iOS.Dialogs
{
    internal class IOSFilePicker : IFilePicker
    {
        public bool CanOpen => false;

        public bool CanSave => false;

        public bool CanExport => false;

        public Task Export(FilePickerSaveOptions options, Func<Stream, FilePickerFileType, Task> writer)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<IFilePickerFile>> OpenAsync(FilePickerOpenOptions options)
        {
            throw new NotImplementedException();
        }

        public IFilePickerBookmark OpenBookmark(string bookmark)
        {
            throw new NotImplementedException();
        }

        public Task<IFilePickerFile> SaveAsync(FilePickerSaveOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
