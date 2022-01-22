#nullable enable
using System.Collections.Generic;

namespace Avalonia.Controls.Platform.Dialogs
{
    public class FilePickerOpenOptions
    {
        public string? Title { get; init; }
        public bool AllowMultiple { get; init; }
        public IReadOnlyList<FilePickerFileType>? FileTypes { get; init; }
    }

    public class FilePickerSaveOptions
    {
        public string? Title { get; init; }
        public string? DefaultFileName { get; init; }
        public IReadOnlyList<FilePickerFileType>? FileTypes { get; init; }
    }
}

#if !NET5_0_OR_GREATER
namespace System.Runtime.CompilerServices
{
    using global::System.Diagnostics;
    using global::System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Reserved to be used by the compiler for tracking metadata.
    ///     This class should not be used by developers in source code.
    /// </summary>
    /// <remarks>
    ///     This definition is provided by the <i>IsExternalInit</i> NuGet package (https://www.nuget.org/packages/IsExternalInit).
    ///     Please see https://github.com/manuelroemer/IsExternalInit for more information.
    /// </remarks>
    [ExcludeFromCodeCoverage, DebuggerNonUserCode]
    internal static class IsExternalInit
    {
    }
}
#endif
