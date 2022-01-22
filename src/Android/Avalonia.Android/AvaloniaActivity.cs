using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

namespace Avalonia.Android
{
    public abstract class AvaloniaActivity : Activity
    {
        internal Action<int, Result, Intent> ActivityResult;

        internal AvaloniaView View;
        object _content;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            View = new AvaloniaView(this);
            if (_content != null)
                View.Content = _content;
            SetContentView(View);
            base.OnCreate(savedInstanceState);
        }

        public object Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                if (View != null)
                    View.Content = value;
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            ActivityResult?.Invoke(requestCode, resultCode, data);
        }
    }
}
