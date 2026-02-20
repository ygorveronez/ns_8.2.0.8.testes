using System.Drawing;
using System.IO;
using Utilidades.Extensions;
using Zen.Barcode;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace Utilidades
{
    public class Barcode
    {
        public static byte[] Gerar(string text, BarcodeFormat symbology, BarcodeMetrics metrics, ImageFormat format)
        {
            // Configura as opções do código de barras
            var options = new EncodingOptions
            {
                Width = 300 * metrics.Scale,
                Height = 150 * metrics.Scale,
                Margin = 10
            };

            // Cria o gerador de código de barras
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = symbology,
                Options = options
            };

            // Gera os dados do código de barras
            var pixelData = barcodeWriter.Write(text);

            // Converte os dados do código de barras em uma imagem
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                using (var ms = new MemoryStream(pixelData.Pixels))
                {
                    bitmap.SetResolution(96, 96);
                    var data = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, data.Scan0, pixelData.Pixels.Length);
                    bitmap.UnlockBits(data);

                    using (var memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, format);
                        return memoryStream.ToArray();
                    }
                }
            }
        }
    }

}


