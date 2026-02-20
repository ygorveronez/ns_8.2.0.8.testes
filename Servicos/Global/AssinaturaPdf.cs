using System;
using System.Security.Cryptography.X509Certificates;

using GemBox.Pdf;

namespace Servicos
{
    public class AssinaturaPdf 
    {

        public string AssinarPdf(string path, string signedPath, X509Certificate2 certificado)
        {

            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

            if(!Utilidades.IO.FileStorageService.Storage.Exists(path))
            {
                throw new Exception("Erro ao assinar PDF");
            }

            using (var document = PdfDocument.Load(path))
            {
                // Add an invisible signature field to the PDF document.
                var signatureField = document.Form.Fields.AddSignature();

                // Initiate signing of a PDF file with the specified digital ID file and the password.
                signatureField.Sign(certificado);

                // Finish signing of a PDF file.
                document.Save(signedPath);
            }

            return path;
        }

    }
}
