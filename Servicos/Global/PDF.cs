using System;
using System.Collections.Generic;
using System.IO;
using ImageMagick;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Servicos
{
    public class PDF
    {
        public static void ConvertBase64ImagesToPdf(List<string> base64Images, string outputPath)
        {
            if (base64Images == null || base64Images.Count == 0)
                throw new ArgumentException("A lista de imagens não pode estar vazia.");

            Document document = new Document();
            PdfWriter.GetInstance(document, Utilidades.IO.FileStorageService.Storage.OpenWrite(outputPath));
            document.Open();

            foreach (var base64Image in base64Images)
            {
                byte[] imageBytes = Convert.FromBase64String(base64Image);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
                Rectangle pageSize = new Rectangle(image.Width, image.Height);
                document.SetPageSize(pageSize);
                document.NewPage();                
                image.SetAbsolutePosition(0, 0);
                document.Add(image);
            }

            document.Close();
        }
        public static void ConvertWebPBase64ImagesToPdf(List<string> base64Images, string outputPath)
        {
            if (base64Images == null || base64Images.Count == 0)
                throw new ArgumentException("A lista de imagens não pode estar vazia.");

            Document document = new Document();
            PdfWriter.GetInstance(document, Utilidades.IO.FileStorageService.Storage.OpenWrite(outputPath));
            document.Open();

            foreach (var base64Image in base64Images)
            {
                byte[] imageBytes = Convert.FromBase64String(base64Image);

                using (var magickImage = new MagickImage(imageBytes))
                {
                    magickImage.Format = MagickFormat.Jpeg;
                    using (var ms = new MemoryStream())
                    {
                        magickImage.Write(ms);
                        ms.Position = 0;

                        iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(ms.ToArray());
                        image.ScaleToFit(document.PageSize);
                        image.Alignment = Element.ALIGN_MIDDLE;
                        document.Add(image);
                        document.NewPage();
                    }
                }
            }

            document.Close();
        }
    }
}