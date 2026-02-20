using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MultiSoftware.NFe.Servicos
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

            }

            if (doc == null)
            {
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

                    if (doc == null)
                    {
                        stream.Position = 0;
                        StreamReader reader = new StreamReader(stream);
                        string text = reader.ReadToEnd();

                        string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";//criado nova tratativa para remoção de caracteres hexadecimais do xml #25984
                        string returnValue = Regex.Replace(text, r, "", RegexOptions.Compiled);

                        var writer = new StreamWriter(stream2);
                        writer.Write(returnValue);
                        writer.Flush();
                        stream2.Position = 0;
                        doc = XDocument.Load(stream2);
                    }
                }
            }

            XNamespace ns = doc.Root.Name.Namespace;
            if (string.IsNullOrWhiteSpace(ns.NamespaceName))
            {
                try
                {
                    //Tratativa para NF-e da ADAR aonde não possui o Namespace no Root principal
                    var nodeNFe = (from ele in doc.Descendants("nfeProc") select ele).FirstOrDefault().FirstNode;
                    stream2 = new MemoryStream();
                    var writer = new StreamWriter(stream2);
                    writer.Write(nodeNFe.ToString());
                    writer.Flush();
                    stream2.Position = 0;

                    doc = XDocument.Load(stream2);
                    ns = doc.Root.Name.Namespace;
                }
                catch
                {
                }
            }

            stream2.Position = 0;

            if (doc != null && (doc.Root.Descendants(ns + "NFe").Count() > 0 || doc.Descendants(ns + "NFe").Count() > 0))
            {
                string versao = (from ele in doc.Descendants(ns + "infNFe") select ele.Attribute("versao").Value).FirstOrDefault();

                if (doc.Descendants(ns + "nfeProc").Count() > 0)
                {
                    if (versao == "2.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc));
                        return (MultiSoftware.NFe.NotaFiscalProcessada.TNfeProc)serializer.Deserialize(stream2);
                    }

                    if (versao == "3.10")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc));
                        return (MultiSoftware.NFe.v310.NotaFiscalProcessada.TNfeProc)serializer.Deserialize(stream2);
                    }

                    if (versao == "4.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc));
                        return (MultiSoftware.NFe.v400.NotaFiscalProcessada.TNfeProc)serializer.Deserialize(stream2);
                    }
                }

                if (doc.Descendants(ns + "NFe").Count() > 0)
                {
                    if (versao == "2.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.NFe.NotaFiscal.TNFe));
                        return (MultiSoftware.NFe.NotaFiscal.TNFe)serializer.Deserialize(stream2);
                    }

                    if (versao == "3.10")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.NFe.v310.NotaFiscal.TNFe));
                        return (MultiSoftware.NFe.v310.NotaFiscal.TNFe)serializer.Deserialize(stream2);
                    }

                    if (versao == "4.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.NFe.v400.NotaFiscal.TNFe));
                        return (MultiSoftware.NFe.v400.NotaFiscal.TNFe)serializer.Deserialize(stream2);
                    }
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
