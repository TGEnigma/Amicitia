﻿namespace AmicitiaLibrary.Graphics
{
    using System.Drawing;

    // TODO
    public interface ITextureFile
    {
        Bitmap GetBitmap();
        Color[] GetPixels();
    }
}
