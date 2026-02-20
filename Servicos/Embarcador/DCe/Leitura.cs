using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Servicos.Embarcador.DCe
{
    public class Leitura
    {
        public static object Ler(System.IO.Stream stream)
        {
            stream.Position = 0;

            XDocument doc = null;
            System.IO.Stream stream2 = new MemoryStream();

            try
            {
                stream.Position = 0;
                doc = XDocument.Load(stream);
                doc.Save(stream2);
            }
            catch (System.Exception)
            {
                // Handle exceptions if needed
            }

            if (doc == null)
            {
                // Handle XML reading errors
                return null;
            }

            XNamespace ns = "http://www.portalfiscal.inf.br/dce";

            if (doc.Root.Name.Namespace != ns)
            {
                // If the namespace does not match, return null
                return null;
            }

            var infDCe = doc.Descendants(ns + "infDCe").FirstOrDefault();
            if (infDCe != null)
            {
                // Deserialize the infDCe element into a specific object
                XmlSerializer serializer = new XmlSerializer(typeof(Dominio.ObjetosDeValor.Embarcador.DCe.TDCeInfDCe));
                using (var reader = infDCe.CreateReader())
                {
                    return (Dominio.ObjetosDeValor.Embarcador.DCe.TDCeInfDCe)serializer.Deserialize(reader);
                }
            }

            return null;
        }

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
            input.Position = 0;
            output.Position = 0;
        }
    }
}
