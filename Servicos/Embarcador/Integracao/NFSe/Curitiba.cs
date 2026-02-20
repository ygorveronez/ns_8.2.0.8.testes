using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Servicos.Embarcador.Integracao.NFSe
{
    public class Curitiba
    {
        #region Métodos Públicos

        public static dynamic Ler(System.IO.Stream stream)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pt-BR");
            stream.Position = 0;

            XDocument doc = null;

            System.IO.Stream stream2 = new MemoryStream();

            // Definio inicio do XML, que pode ser 0, ou o tamanho do so cabecalho
            int inicioXML = 0;
            try
            {
                StreamReader oReader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1"));
                doc = XDocument.Load(oReader);
                doc.Save(stream2);
            }
            catch (System.Exception)
            {

            }

            if (doc == null)
            {
                try
                {
                    // Remove Cabecalho
                    inicioXML = ("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>".Length * 2) - 2; // Multiplicado por 2 pois a string do strim possui "\0" para cada caracter
                    stream.Position = inicioXML;

                    StreamReader oReader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1"));
                    doc = XDocument.Load(oReader);
                    doc.Save(stream2);
                }
                catch (System.Exception)
                {
                }
            }

            if (doc == null)
            {
                stream.Position = 0;
                doc = XDocument.Load(stream);
                doc.Save(stream2);
            }

            stream2.Position = 0;

            if (doc != null && (doc.Root.Descendants("Rps").Count() > 0 || doc.Descendants("InfRps").Count() > 0))
            {
                string jsonText = JsonConvert.SerializeXNode(doc.Descendants("InfRps").FirstOrDefault());
                return JsonConvert.DeserializeObject<dynamic>(jsonText);
            }

            return null;
        }

        #endregion
    }
}
