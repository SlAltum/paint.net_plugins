// Name:
// Submenu:
// Author:
// Title:
// Version:
// Desc:
// Keywords:
// URL:
// Help:

// For help writing a Bitmap plugin: https://boltbait.com/pdn/CodeLab/help/tutorial/bitmap/

#region UICode
IntSliderControl Tolerance = 0; // [0,100] 差容
#endregion

protected override void OnRender(IBitmapEffectOutput output)
{
    using IEffectInputBitmap<ColorBgra32> sourceBitmap = Environment.GetSourceBitmapBgra32();
    using IBitmapLock<ColorBgra32> sourceLock = sourceBitmap.Lock(new RectInt32(0, 0, sourceBitmap.Size));
    RegionPtr<ColorBgra32> sourceRegion = sourceLock.AsRegionPtr();

    RectInt32 outputBounds = output.Bounds;
    using IBitmapLock<ColorBgra32> outputLock = output.LockBgra32();
    RegionPtr<ColorBgra32> outputSubRegion = outputLock.AsRegionPtr();
    var outputRegion = outputSubRegion.OffsetView(-outputBounds.Location);
    //uint seed = RandomNumber.InitializeSeed(RandomNumberRenderSeed, outputBounds.Location);

    // Delete any of these lines you don't need
    ColorBgra32 primaryColor = Environment.PrimaryColor;
    ColorBgra32 secondaryColor = Environment.SecondaryColor;
    int canvasCenterX = Environment.Document.Size.Width / 2;
    int canvasCenterY = Environment.Document.Size.Height / 2;
    var selection = Environment.Selection.RenderBounds;
    int selectionCenterX = (selection.Right - selection.Left) / 2 + selection.Left;
    int selectionCenterY = (selection.Bottom - selection.Top) / 2 + selection.Top;

    ColorBgra32 transparent = new ColorBgra32(0, 0, 0, 0);
    // Loop through the output canvas tile
    for (int y = outputBounds.Top; y < outputBounds.Bottom; ++y)
    {
        if (IsCancelRequested) return;

        for (int x = outputBounds.Left; x < outputBounds.Right; ++x)
        {
            // Get your source pixel
            ColorBgra32 sourcePixel = sourceRegion[x,y];

            // TODO: Change source pixel according to some algorithm
            if(
                ( (sourcePixel.B - Environment.PrimaryColor.B) * (sourcePixel.B - Environment.PrimaryColor.B) +
                (sourcePixel.G - Environment.PrimaryColor.G) * (sourcePixel.G - Environment.PrimaryColor.G) +
                (sourcePixel.R - Environment.PrimaryColor.R) * (sourcePixel.R - Environment.PrimaryColor.R) +
                (sourcePixel.A - Environment.PrimaryColor.A) * (sourcePixel.A - Environment.PrimaryColor.A) )
                <= 260100 * Tolerance / 100
            ){
                sourcePixel = transparent;
            }

            // Save your pixel to the output canvas
            outputRegion[x,y] = sourcePixel;
        }
    }
}
