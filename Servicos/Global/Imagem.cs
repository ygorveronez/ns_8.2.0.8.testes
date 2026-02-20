using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;


namespace Servicos
{
    public class Imagem
    {
        public System.Drawing.Image RedimensionarImagem(System.Drawing.Image imagem, Size tamanho)
        {
            int larguraOrigem = imagem.Width;
            int alturaOrigem = imagem.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = (tamanho.Width / (float)larguraOrigem);
            nPercentH = (tamanho.Height / (float)alturaOrigem);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int larguraDestino = (int)(larguraOrigem * nPercent);
            int alturaDestino = (int)(alturaOrigem * nPercent);

            Bitmap b = new Bitmap(larguraDestino, alturaDestino);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imagem, 0, 0, larguraDestino, alturaDestino);
            g.Dispose();

            return b;
        }

        public static byte[] GetFromPath(string path, System.Drawing.Imaging.ImageFormat format)
        {
            using (System.IO.MemoryStream mm = new System.IO.MemoryStream())
            {
                if (!string.IsNullOrWhiteSpace(path) && Utilidades.IO.FileStorageService.Storage.Exists(path))
                {
                    using (System.Drawing.Image img = System.Drawing.Image.FromStream(Utilidades.IO.FileStorageService.Storage.OpenRead(path)))
                    {
                        img.Save(mm, format);
                    }
                }

                return mm.ToArray();
            }
        }

        public static byte[] DrawText(string text, System.Drawing.Font font, Color textColor, Color backColor, int angle)
        {
            SizeF textSize = GetEvenTextImageSize(text, font);

            SizeF imageSize = GetRotatedTextImageSize(textSize, angle);

            using (System.Drawing.Image img = new Bitmap((int)imageSize.Width, (int)imageSize.Height))
            {

                using (Graphics drawing = Graphics.FromImage(img))
                {
                    drawing.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    drawing.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    drawing.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    SizeF textContainerSize = drawing.VisibleClipBounds.Size;

                    drawing.Clear(backColor);

                    Brush textBrush = new SolidBrush(textColor);

                    drawing.TranslateTransform(textContainerSize.Width / 2, textContainerSize.Height / 2);

                    drawing.RotateTransform(angle);

                    drawing.DrawString(text, font, textBrush, -(textSize.Width / 2), -(textSize.Height / 2));

                    drawing.Save();

                    textBrush.Dispose();
                }

                using (System.IO.MemoryStream mm = new System.IO.MemoryStream())
                {
                    img.Save(mm, System.Drawing.Imaging.ImageFormat.Bmp);

                    return mm.ToArray();
                }
            }
        }

        public static SizeF GetRotatedTextImageSize(SizeF fontSize, int angle)
        {
            double theta = angle * Math.PI / 180.0;

            while (theta < 0.0)
                theta += 2 * Math.PI;

            double adjacentTop, oppositeTop;
            double adjacentBottom, oppositeBottom;

            if ((theta >= 0.0 && theta < Math.PI / 2.0) || (theta >= Math.PI && theta < (Math.PI + (Math.PI / 2.0))))
            {
                adjacentTop = Math.Abs(Math.Cos(theta)) * fontSize.Width;
                oppositeTop = Math.Abs(Math.Sin(theta)) * fontSize.Width;
                adjacentBottom = Math.Abs(Math.Cos(theta)) * fontSize.Height;
                oppositeBottom = Math.Abs(Math.Sin(theta)) * fontSize.Height;
            }
            else
            {
                adjacentTop = Math.Abs(Math.Sin(theta)) * fontSize.Height;
                oppositeTop = Math.Abs(Math.Cos(theta)) * fontSize.Height;
                adjacentBottom = Math.Abs(Math.Sin(theta)) * fontSize.Width;
                oppositeBottom = Math.Abs(Math.Cos(theta)) * fontSize.Width;
            }

            int nWidth = (int)Math.Ceiling(adjacentTop + oppositeBottom);
            int nHeight = (int)Math.Ceiling(adjacentBottom + oppositeTop);

            return new SizeF(nWidth, nHeight);
        }

        public static SizeF GetEvenTextImageSize(string text, System.Drawing.Font font)
        {
            using (var image = new Bitmap(1, 1, PixelFormat.Format32bppArgb))
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    return graphics.MeasureString(text, font);
                }
            }
        }

        public static string ConverterWebpParaJpg(string imagemBase64)
        {
            try
            {
                if (ValidarImagemJPG(imagemBase64)) return imagemBase64;

                byte[] imagemBytes = Convert.FromBase64String(imagemBase64);
                using (MemoryStream inputStream = new MemoryStream(imagemBytes))
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (var image = new ImageMagick.MagickImage(inputStream))
                    {
                        image.Quality = 90;
                        image.Write(outputStream, ImageMagick.MagickFormat.Jpeg);
                    }
                    return Convert.ToBase64String(outputStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex, "ConversaoWebPParaJPG");
                return imagemBase64;
            }
        }

        public static bool ValidarImagemJPG(string imagemBase64)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(imagemBase64);
                if (bytes.Length > 3) return bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF;
            }
            catch
            {
                //Ignorar erros na verificação
            }
            return false;
        }
    }
}
