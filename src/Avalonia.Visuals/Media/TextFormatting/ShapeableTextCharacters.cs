﻿using Avalonia.Utilities;

namespace Avalonia.Media.TextFormatting
{
    /// <summary>
    /// A group of characters that can be shaped.
    /// </summary>
    public sealed class ShapeableTextCharacters : TextRun
    {
        public ShapeableTextCharacters(ReadOnlySlice<char> text, TextRunProperties properties, sbyte biDiLevel)
        {
            TextSourceLength = text.Length;
            Text = text;
            Properties = properties;
            BidiLevel = biDiLevel;
        }

        public override int TextSourceLength { get; }

        public override ReadOnlySlice<char> Text { get; }

        public override TextRunProperties Properties { get; }
        
        public sbyte BidiLevel { get; }
    }
}
