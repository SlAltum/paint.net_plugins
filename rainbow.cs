// Name:
// Submenu:
// Author:
// Title:
// Version:
// Desc:
// Keywords:
// URL:
// Help:

// For help writing a GPU Image plugin: https://boltbait.com/pdn/CodeLab/help/tutorial/image/

#region UICode
IntSliderControl Angle = 0; // [-180,180] 倾斜角度
#endregion

ColorBgra32 GetRGB(double lambda){
    double r, g, b, alpha;
    if (lambda >= 380.0 && lambda < 440.0) {
        r = -1.0 * (lambda - 440.0) / (440.0 - 380.0);
        g = 0.0;
        b = 1.0;
    }else if (lambda >= 440.0 && lambda < 490.0) {
        r = 0.0;
        g = (lambda - 440.0) / (490.0 - 440.0);
        b = 1.0;
    }else if (lambda >= 490.0 && lambda < 510.0) {
        r = 0.0;
        g = 1.0;
        b = -1.0 * (lambda - 510.0) / (510.0 - 490.0);
    }else if (lambda >= 510.0 && lambda < 580.0) {
        r = (lambda - 510.0) / (580.0 - 510.0);
        g = 1.0;
        b = 0.0;
    }else if (lambda >= 580.0 && lambda < 645.0) {
        r = 1.0;
        g = -1.0 * (lambda - 645.0) / (645.0 - 580.0);
        b = 0.0;
    }else if (lambda >= 645.0 && lambda <= 780.0) {
        r = 1.0;
        g = 0.0;
        b = 0.0;
    }else {
        r = 0.0;
        g = 0.0;
        b = 0.0;
    }

    // 在可见光谱的边缘处强度较低。
    if (lambda >= 380.0 && lambda < 420.0) {
        alpha = 0.30 + 0.70 * (lambda - 380.0) / (420.0 - 380.0);
    }else if (lambda >= 420.0 && lambda < 701.0) {
        alpha = 1.0;
    }else if (lambda >= 701.0 && lambda < 780.0) {
        alpha = 0.30 + 0.70 * (780.0 - lambda) / (780.0 - 700.0);
    }else {
        alpha = 0.0;
    }

    // 1953年在引入NTSC电视时,计算具有荧光体的监视器的亮度公式如下
    int Y = (int)(0.212671 * r + 0.715160 * g + 0.072169 * b);

    // 伽马射线 gamma
    // 照明强度 intensityMax
    double intensityMax = 255.0;
    double gamma = 0.8;
    int R = (r == 0.0) ? 0 : (int)Math.Round(intensityMax * Math.Pow(r * alpha, gamma));
    int G = (g == 0.0) ? 0 : (int)Math.Round(intensityMax * Math.Pow(g * alpha, gamma));
    int B = (b == 0.0) ? 0 : (int)Math.Round(intensityMax * Math.Pow(b * alpha, gamma));
    int A = (int)alpha;

    return new ColorBgra32((byte)R, (byte)G, (byte)B, 255);
}

protected override void OnRender(IBitmapEffectOutput output)
{
    using IEffectInputBitmap<ColorBgra32> sourceBitmap = Environment.GetSourceBitmapBgra32();
    using IBitmapLock<ColorBgra32> sourceLock = Environment.GetSourceBitmapBgra32().Lock(new RectInt32(0, 0, sourceBitmap.Size));
    RegionPtr<ColorBgra32> sourceRegion = sourceLock.AsRegionPtr();

    RectInt32 outputBounds = output.Bounds;
    using IBitmapLock<ColorBgra32> outputLock = output.LockBgra32();
    RegionPtr<ColorBgra32> outputSubRegion = outputLock.AsRegionPtr();
    var outputRegion = outputSubRegion.OffsetView(-outputBounds.Location);

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

            // 从左到右彩虹
            double arcAngle = Math.PI * Angle / 180;
            double lambda = 580 + 400 * Math.Abs( Math.Cos(arcAngle) ) * Math.Cos(arcAngle) * (x - selectionCenterX) / (selection.Right - selection.Left)
                + 400 * Math.Abs( Math.Sin(arcAngle) ) * Math.Sin(arcAngle) * (y - selectionCenterY) / (selection.Bottom - selection.Top);
            lambda = 296400 / (1160 - lambda);
            sourcePixel = GetRGB(lambda);
            // Save your pixel to the output canvas
            outputRegion[x,y] = sourcePixel;
        }
    }
}