using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Avalonia.Web.Blazor.Interop
{
    internal class FilePickerInterop : JSModuleInterop
    {
        private const string JsFilename = "./_content/Avalonia.Web.Blazor/FilePicker.js";

        public static async Task<FilePickerInterop> ImportAsync(IJSRuntime js)
        {
            var interop = new FilePickerInterop(js);
            await interop.ImportAsync();
            return interop;
        }

        public FilePickerInterop(IJSRuntime js)
            : base(js, JsFilename)
        {
        }
    }
}
