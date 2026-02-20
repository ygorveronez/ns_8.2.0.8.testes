using System.IO;
using System.Web;

namespace Servicos.Extensions;

public static class HttpPostedFileBaseExtensions
{
    public static void SaveAs(this HttpPostedFileBase filebase, string path)
    {
        using var memorystream = new MemoryStream();
        filebase.InputStream.CopyTo(memorystream);
        filebase.InputStream.Position = 0;
        memorystream.Position = 0;
        Utilidades.IO.FileStorageService.Storage.WriteAllBytes(path, memorystream.ToArray());
    }
    
}