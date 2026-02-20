using System.Drawing;
using System.Drawing.Imaging;

namespace NxTiler.Infrastructure.Capture;

public sealed partial class WgcCaptureService
{
    private static byte[] EncodePng(Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }
}
