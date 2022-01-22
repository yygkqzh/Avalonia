#nullable enable
namespace Avalonia.Controls.Platform.Dialogs
{
    public class FilePickerFileTypes
    {
        public static FilePickerFileType All { get; } = new FilePickerFileType("All")
        {
            Extensions = new[] { "*" },
            MimeTypes = new[] { "*/*" }
        };

        public static FilePickerFileType TextPlain { get; } = new FilePickerFileType("Plain Text")
        {
            Extensions = new[] { "txt" },
            // AppleUniformTypeIdentifiers = new[] { },
            MimeTypes = new[] { "text/plain" }
        };

        public static FilePickerFileType ImageAll { get; } = new FilePickerFileType("All Images")
        {
            Extensions = new[] { "png", "jpg", "jpeg", "gif", "bmp" },
            // AppleUniformTypeIdentifiers = new[] { },
            MimeTypes = new[] { "image/*" }
        };

        public static FilePickerFileType ImageJpg { get; } = new FilePickerFileType("JPEG image")
        {
            Extensions = new[] { "jpg", "jpeg" },
            // AppleUniformTypeIdentifiers = new[] { },
            MimeTypes = new[] { "image/jpeg" }
        };

        public static FilePickerFileType ImagePng { get; } = new FilePickerFileType("PNG image")
        {
            Extensions = new[] { "png" },
            // AppleUniformTypeIdentifiers = new[] { },
            MimeTypes = new[] { "image/png" }
        };

        public static FilePickerFileType Pdf { get; } = new FilePickerFileType("PDF document")
        {
            Extensions = new[] { "pdf" },
            AppleUniformTypeIdentifiers = new[] { "com.adobe.pdf" },
            MimeTypes = new[] { "application/pdf" }
        };
    }
}
