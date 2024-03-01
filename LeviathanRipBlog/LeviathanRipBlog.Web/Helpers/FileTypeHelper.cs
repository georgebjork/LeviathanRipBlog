namespace LeviathanRipBlog.Web.Helpers;

public static class FileTypeHelper
{
    // some magic bytes for the most important image formats, see Wikipedia for more
    static readonly List<byte> jpg = new List<byte> { 0xFF, 0xD8 };
    static readonly List<byte> bmp = new List<byte> { 0x42, 0x4D };
    static readonly List<byte> gif = new List<byte> { 0x47, 0x49, 0x46 };
    static readonly List<byte> png = new List<byte> { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A};

    static readonly List<(List<byte> magic, string? extension)> imageFormats = new List<(List<byte> magic, string? extension)>()
    {
        (jpg, "jpg"),
        (bmp, "bmp"),
        (gif, "gif"),
        (png, "png")
    };

    public static string? TryGetExtension(byte[] array)
    {
        return (from imageFormat in imageFormats where array.IsImage(imageFormat.magic) select imageFormat.extension).FirstOrDefault();
    }

    private static bool IsImage(this Byte[] array, List<byte> comparer, int offset = 0)
    {
        var arrayIndex = offset;
        foreach (var c in comparer)
        {
            if (arrayIndex > array.Length -1 || array[arrayIndex] != c)
                return false;
            ++arrayIndex;
        }
        return true;
    }
}
