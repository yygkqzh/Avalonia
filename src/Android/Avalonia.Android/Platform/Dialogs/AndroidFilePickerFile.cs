#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Android.Content;
using Android.OS;
using Android.Provider;

using Avalonia.Controls.Platform.Dialogs;
using Avalonia.Logging;

using AndroidUri = Android.Net.Uri;

namespace Avalonia.Android.Platform.Dialogs
{
    internal class AndroidFilePickerFile : IFilePickerFile, IFilePickerBookmark
    {
        private const string storageTypePrimary = "primary";
        private const string storageTypeRaw = "raw";
        private const string storageTypeImage = "image";
        private const string storageTypeVideo = "video";
        private const string storageTypeAudio = "audio";
        private static readonly string[] contentUriPrefixes =
        {
            "content://downloads/public_downloads",
            "content://downloads/my_downloads",
            "content://downloads/all_downloads",
        };
        internal const string UriSchemeFile = "file";
        internal const string UriSchemeContent = "content";

        internal const string UriAuthorityExternalStorage = "com.android.externalstorage.documents";
        internal const string UriAuthorityDownloads = "com.android.providers.downloads.documents";
        internal const string UriAuthorityMedia = "com.android.providers.media.documents";

        private readonly Context _context;
        private readonly AndroidUri _uri;
        private readonly bool _isOutput;
        public AndroidFilePickerFile(Context context, AndroidUri uri, bool isOutput)
        {
            _context = context;
            _uri = uri;
            _isOutput = isOutput;
        }

        public string FileName => GetColumnValue(_context, _uri, MediaStore.Files.FileColumns.DisplayName)
            ?? _uri.PathSegments.LastOrDefault() ?? string.Empty;

        public Stream Stream => OpenContentStream(_context, _uri, _isOutput)
            ?? throw new InvalidOperationException("Failed to open content stream");

        public bool CanBookmark => true;

        bool IFilePickerBookmark.CanOpenRead => true;

        bool IFilePickerBookmark.CanOpenWrite => true;

        Task<Stream> IFilePickerBookmark.OpenRead() => Task.FromResult(OpenContentStream(_context, _uri, false)
            ?? throw new InvalidOperationException("Failed to open content stream"));

        Task<Stream> IFilePickerBookmark.OpenWrite() => Task.FromResult(OpenContentStream(_context, _uri, true)
            ?? throw new InvalidOperationException("Failed to open content stream"));

        Task IFilePickerBookmark.Release()
        {
            _context.ContentResolver?.ReleasePersistableUriPermission(_uri, ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
            return Task.CompletedTask;
        }

        public Task<string?> SaveBookmark()
        {
            _context.ContentResolver?.TakePersistableUriPermission(_uri, ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
            return Task.FromResult(_uri.ToString());
        }

        public bool TryGetFullPath([NotNullWhen(true)] out string? path)
        {
            path = EnsurePhysicalPath(_context, _uri, true);
            return path is not null;
        }

        internal string? EnsurePhysicalPath(Context context, AndroidUri uri, bool requireExtendedAccess = true)
        {
            // if this is a file, use that
            if (uri.Scheme?.Equals(UriSchemeFile, StringComparison.OrdinalIgnoreCase) == true)
                return uri.Path;

            // try resolve using the content provider
            var absolute = ResolvePhysicalPath(context, uri, requireExtendedAccess);
            if (!string.IsNullOrWhiteSpace(absolute) && Path.IsPathRooted(absolute))
                return absolute;

            return null;
        }

        private string? ResolvePhysicalPath(Context context, AndroidUri uri, bool requireExtendedAccess = true)
        {
            if (uri.Scheme?.Equals(UriSchemeFile, StringComparison.OrdinalIgnoreCase) == true)
            {
                // if it is a file, then return directly

                var resolved = uri.Path;
                if (File.Exists(resolved))
                    return resolved;
            }
            else
            {
                // if this is on an older OS version, or we just need it now

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat && DocumentsContract.IsDocumentUri(context, uri))
                {
                    var resolved = ResolveDocumentPath(context, uri);
                    if (File.Exists(resolved))
                        return resolved;
                }
                else if (uri.Scheme?.Equals(UriSchemeContent, StringComparison.OrdinalIgnoreCase) == true)
                {
                    var resolved = ResolveContentPath(context, uri);
                    if (File.Exists(resolved))
                        return resolved;
                }
            }

            return null;
        }

        private string? ResolveDocumentPath(Context context, AndroidUri uri)
        {
            Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "Trying to resolve document URI: '{Uri}'", uri);

            var docId = DocumentsContract.GetDocumentId(uri);

            var docIdParts = docId?.Split(':');
            if (docIdParts == null || docIdParts.Length == 0)
                return null;

            if (uri.Authority?.Equals(UriAuthorityExternalStorage, StringComparison.OrdinalIgnoreCase) == true)
            {
                Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "Resolving external storage URI: '{Uri}'", uri);

                if (docIdParts.Length == 2)
                {
                    var storageType = docIdParts[0];
                    var uriPath = docIdParts[1];

                    // This is the internal "external" memory, NOT the SD Card
                    if (storageType.Equals(storageTypePrimary, StringComparison.OrdinalIgnoreCase))
                    {
#pragma warning disable CS0618 // Type or member is obsolete
                        var root = global::Android.OS.Environment.ExternalStorageDirectory!.Path;
#pragma warning restore CS0618 // Type or member is obsolete

                        return Path.Combine(root, uriPath);
                    }

                    // TODO: support other types, such as actual SD Cards
                }
            }
            else if (uri.Authority?.Equals(UriAuthorityDownloads, StringComparison.OrdinalIgnoreCase) == true)
            {
                Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "Resolving downloads URI: '{Uri}'", uri);

                // NOTE: This only really applies to older Android vesions since the privacy changes

                if (docIdParts.Length == 2)
                {
                    var storageType = docIdParts[0];
                    var uriPath = docIdParts[1];

                    if (storageType.Equals(storageTypeRaw, StringComparison.OrdinalIgnoreCase))
                        return uriPath;
                }

                // ID could be "###" or "msf:###"
                var fileId = docIdParts.Length == 2
                    ? docIdParts[1]
                    : docIdParts[0];

                foreach (var prefix in contentUriPrefixes)
                {
                    var uriString = prefix + "/" + fileId;
                    var contentUri = AndroidUri.Parse(uriString)!;

                    if (GetDataFilePath(context, contentUri) is string filePath)
                        return filePath;
                }
            }
            else if (uri.Authority?.Equals(UriAuthorityMedia, StringComparison.OrdinalIgnoreCase) == true)
            {
                Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "Resolving media URI: '{Uri}'", uri);

                if (docIdParts.Length == 2)
                {
                    var storageType = docIdParts[0];
                    var uriPath = docIdParts[1];

                    AndroidUri? contentUri = null;
                    if (storageType.Equals(storageTypeImage, StringComparison.OrdinalIgnoreCase))
                        contentUri = MediaStore.Images.Media.ExternalContentUri;
                    else if (storageType.Equals(storageTypeVideo, StringComparison.OrdinalIgnoreCase))
                        contentUri = MediaStore.Video.Media.ExternalContentUri;
                    else if (storageType.Equals(storageTypeAudio, StringComparison.OrdinalIgnoreCase))
                        contentUri = MediaStore.Audio.Media.ExternalContentUri;

                    if (contentUri != null && GetDataFilePath(context, contentUri, $"{MediaStore.MediaColumns.Id}=?", new[] { uriPath }) is string filePath)
                        return filePath;
                }
            }

            Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "Unable to resolve document URI: '{Uri}'", uri);

            return null;
        }

        private string? ResolveContentPath(Context context, AndroidUri uri)
        {
            Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "Trying to resolve content URI: '{Uri}'", uri);

            if (GetDataFilePath(context, uri) is string filePath)
                return filePath;

            // TODO: support some additional things, like Google Photos if that is possible

            Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "Unable to resolve content URI: '{Uri}'", uri);

            return null;
        }

        private Stream? OpenContentStream(Context context, AndroidUri uri, bool isOutput)
        {
            var isVirtual = IsVirtualFile(context, uri);
            if (isVirtual)
            {
                Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "Content URI was virtual: '{Uri}'", uri);
                return GetVirtualFileStream(context, uri, isOutput);
            }

            return isOutput
                ? context.ContentResolver?.OpenOutputStream(uri)
                : context.ContentResolver?.OpenInputStream(uri);
        }

        private bool IsVirtualFile(Context context, AndroidUri uri)
        {
            if (!DocumentsContract.IsDocumentUri(context, uri))
                return false;

            var value = GetColumnValue(context, uri, DocumentsContract.Document.ColumnFlags);
            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out var flagsInt))
            {
                var flags = (DocumentContractFlags)flagsInt;
                return flags.HasFlag(DocumentContractFlags.VirtualDocument);
            }

            return false;
        }

        private Stream? GetVirtualFileStream(Context context, AndroidUri uri, bool isOutput)
        {
            var mimeTypes = context.ContentResolver?.GetStreamTypes(uri, FilePickerFileTypes.All.MimeTypes![0]);
            if (mimeTypes?.Length >= 1)
            {
                var mimeType = mimeTypes[0];
                var asset = context.ContentResolver!
                    .OpenTypedAssetFileDescriptor(uri, mimeType, null);

                var stream = isOutput
                    ? asset?.CreateOutputStream()
                    : asset?.CreateInputStream();

                return stream;
            }

            return null;
        }

        private string? GetColumnValue(Context context, AndroidUri contentUri, string column, string? selection = null, string[]? selectionArgs = null)
        {
            try
            {
                var value = QueryContentResolverColumn(context, contentUri, column, selection, selectionArgs);
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Verbose, LogArea.AndroidPlatform)?.Log(this, "File metadata reader failed: '{Exception}'", ex);
            }

            return null;
        }

        private string? GetDataFilePath(Context context, AndroidUri contentUri, string? selection = null, string[]? selectionArgs = null)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            const string column = MediaStore.Files.FileColumns.Data;
#pragma warning restore CS0618 // Type or member is obsolete

            // ask the content provider for the data column, which may contain the actual file path
            var path = GetColumnValue(context, contentUri, column, selection, selectionArgs);
            return !string.IsNullOrEmpty(path) && Path.IsPathRooted(path) ? path : null;
        }

        private string? QueryContentResolverColumn(Context context, AndroidUri contentUri, string columnName, string? selection = null, string[]? selectionArgs = null)
        {
            string? text = null;

            var projection = new[] { columnName };
            using var cursor = context.ContentResolver!.Query(contentUri, projection, selection, selectionArgs, null);
            if (cursor?.MoveToFirst() == true)
            {
                var columnIndex = cursor.GetColumnIndex(columnName);
                if (columnIndex != -1)
                    text = cursor.GetString(columnIndex);
            }

            return text;
        }
    }
}
