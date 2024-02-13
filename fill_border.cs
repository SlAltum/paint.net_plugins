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
IntSliderControl Size = 1; // [1,100] 粗细
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
    IGeometry outlineGeometry = Environment.Selection.Geometry;

    // Delete any of these lines you don't need
    ColorBgra32 primaryColor = Environment.PrimaryColor;
    ColorBgra32 secondaryColor = Environment.SecondaryColor;
    int canvasCenterX = Environment.Document.Size.Width / 2;
    int canvasCenterY = Environment.Document.Size.Height / 2;
    var selection = Environment.Selection.RenderBounds;
    int selectionCenterX = (selection.Right - selection.Left) / 2 + selection.Left;
    int selectionCenterY = (selection.Bottom - selection.Top) / 2 + selection.Top;

    // Loop through the output canvas tile
    for (int y = outputBounds.Top; y < outputBounds.Bottom; ++y)
    {
        if (IsCancelRequested) return;

        for (int x = outputBounds.Left; x < outputBounds.Right; ++x)
        {
            // Get your source pixel
            ColorBgra32 sourcePixel = sourceRegion[x,y];

            if( !outlineGeometry.FillContainsPoint(new Point2Float(x-Size, y)) || 
                !outlineGeometry.FillContainsPoint(new Point2Float(x, y-Size)) ||
                !outlineGeometry.FillContainsPoint(new Point2Float(x+Size, y)) || 
                !outlineGeometry.FillContainsPoint(new Point2Float(x, y+Size))
            ){
                sourcePixel = Environment.PrimaryColor;
            }

            // Save your pixel to the output canvas
            outputRegion[x,y] = sourcePixel;
        }
    }
}
