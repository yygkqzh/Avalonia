#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Android.App;
using Android.Content;

using Avalonia.Controls.Platform.Dialogs;
using Avalonia.Logging;

using AndroidUri = Android.Net.Uri;

namespace Avalonia.Android.Platform.Dialogs
{
    internal class AndroidFilePicker : IFilePicker
    {
        private readonly AvaloniaActivity _activity;
        private int _lastRequestCode = 0;

        public AndroidFilePicker(AvaloniaActivity activity)
        {
            _activity = activity;
        }

        public bool CanOpen => true;

        public bool CanSave => true;

        public bool CanExport => false;

        public Task Export(FilePickerSaveOptions options, Func<Stream, FilePickerFileType, Task> writer) => throw new NotImplementedException();

        public IFilePickerBookmark? OpenBookmark(string bookmark)
        {
            var uri = AndroidUri.Parse(bookmark) ?? throw new ArgumentException("Couldn't parse Bookmark value", nameof(bookmark));
            return new AndroidFilePickerFile(_activity, uri, true);
        }

        public async Task<IReadOnlyList<IFilePickerFile>> OpenAsync(FilePickerOpenOptions options)
        {
            var resultList = new List<IFilePickerFile>();

            try
            {
                var mimeTypes = options.FileTypes?.Where(t => t != FilePickerFileTypes.All)
                    .SelectMany(f => f.MimeTypes).Distinct().ToArray() ?? Array.Empty<string>();

                var intent = new Intent(Intent.ActionOpenDocument)
                    .AddCategory(Intent.CategoryOpenable)
                    .PutExtra(Intent.ExtraAllowMultiple, options.AllowMultiple)
                    .SetType(FilePickerFileTypes.All.MimeTypes![0]);
                if (mimeTypes.Length > 0)
                {
                    _ = intent.PutExtra(Intent.ExtraMimeTypes, mimeTypes);
                }

                var pickerIntent = Intent.CreateChooser(intent, options?.Title ?? "Select file");

                var tcs = new TaskCompletionSource<Intent?>();
                var currentRequestCode = _lastRequestCode++;

                _activity.ActivityResult += OnActivityResult;
                _activity.StartActivityForResult(pickerIntent, currentRequestCode);

                var result = await tcs.Task;


                if (result != null)
                {
                    if (result.ClipData is { } clipData)
                    {
                        for (var i = 0; i < clipData.ItemCount; i++)
                        {
                            var uri = clipData.GetItemAt(i)?.Uri;
                            if (uri != null)
                            {
                                resultList.Add(new AndroidFilePickerFile(_activity, uri, false));
                            }
                        }
                    }
                    else if (result.Data is { } uri)
                    {
                        resultList.Add(new AndroidFilePickerFile(_activity, uri, false));
                    }
                }

                void OnActivityResult(int requestCode, Result resultCode, Intent data)
                {
                    if (currentRequestCode != requestCode)
                    {
                        return;
                    }

                    _activity.ActivityResult -= OnActivityResult;

                    _ = tcs.TrySetResult(resultCode == Result.Ok ? data : null);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.AndroidPlatform)?.Log(this, "Failed to open file picker. Error message: {Message}", ex.Message);
            }

            return resultList;
        }

        public async Task<IFilePickerFile?> SaveAsync(FilePickerSaveOptions options)
        {
            try
            {
                var mimeTypes = options.FileTypes?.Where(t => t != FilePickerFileTypes.All)
                    .SelectMany(f => f.MimeTypes).Distinct().ToArray() ?? Array.Empty<string>();

                var intent = new Intent(Intent.ActionCreateDocument)
                    .AddCategory(Intent.CategoryOpenable)
                    .SetType(FilePickerFileTypes.All.MimeTypes![0]);
                if (mimeTypes.Length > 0)
                {
                    _ = intent.PutExtra(Intent.ExtraMimeTypes, mimeTypes);
                }

                var pickerIntent = Intent.CreateChooser(intent, options?.Title ?? "Save file");

                var tcs = new TaskCompletionSource<Intent?>();
                var currentRequestCode = _lastRequestCode++;

                _activity.ActivityResult += OnActivityResult;
                _activity.StartActivityForResult(pickerIntent, currentRequestCode);

                var result = await tcs.Task;
                if (result?.Data is { } uri)
                {
                    return new AndroidFilePickerFile(_activity, uri, true);
                }

                void OnActivityResult(int requestCode, Result resultCode, Intent data)
                {
                    if (currentRequestCode != requestCode)
                    {
                        return;
                    }

                    _activity.ActivityResult -= OnActivityResult;

                    _ = tcs.TrySetResult(resultCode == Result.Ok ? data : null);
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Error, LogArea.AndroidPlatform)?.Log(this, "Failed to open save file picker. Error message: {Message}", ex.Message);
            }
            return null;
        }
    }
}
