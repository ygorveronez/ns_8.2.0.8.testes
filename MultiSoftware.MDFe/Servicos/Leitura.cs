using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MultiSoftware.MDFe.Servicos
{
    public static class Leitura
    {
        public static object Ler(Stream stream)
        {
            stream.Position = 0;

            XDocument doc = null;
            // Definio inicio do XML, que pode ser 0, ou o tamanho do so cabecalho
            int inicioXML = 0;

            try
            {
                // as vezes, basta remover o cabecalho "<?xml version="1.0" encoding="UTF-8"?>"
                doc = XDocument.Load(stream);
            }
            catch (System.Exception e)
            {
                Infrastructure.Services.Logging.Logger.Current.Error($"[Arquitetura-CatchNoAction] Erro ao carregar XML MDFe - tentando sem cabeçalho: {e}", "CatchNoAction");
            }

            if (doc == null)
            {
                // Remove Cabecalho
                inicioXML = ("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>".Length * 2) - 2; // Multiplicado por 2 pois a string do strim possui "\0" para cada caracter
                stream.Position = inicioXML;
                doc = XDocument.Load(stream);
            }

            XNamespace ns = doc.Root.Name.Namespace;
            stream.Position = inicioXML;

            if (doc.Root.Descendants(ns + "infMDFe").Count() > 0)
            {
                string versao = (from ele in doc.Descendants(ns + "infMDFe") select ele.Attribute("versao").Value).FirstOrDefault();

                if (doc.Descendants(ns + "mdfeProc").Count() > 0)
                {
                    if (versao == "1.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.MDFe.v100a.mdfeProc));
                        return (MultiSoftware.MDFe.v100a.mdfeProc)serializer.Deserialize(stream);
                    }
                    else if (versao == "3.00")
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.MDFe.v300.mdfeProc));
                        return (MultiSoftware.MDFe.v300.mdfeProc)serializer.Deserialize(stream);
                    }
                }
            }

            if (doc.Descendants(ns + "procEventoMDFe").Count() > 0)
            {
                string versao = (from ele in doc.Descendants(ns + "procEventoMDFe") select ele.Attribute("versao")?.Value).FirstOrDefault();

                if (string.IsNullOrWhiteSpace(versao))
                    versao = (from ele in doc.Descendants(ns + "eventoMDFe") select ele.Attribute("versao")?.Value).FirstOrDefault();

                if (versao == "3.00")
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(MultiSoftware.MDFe.v300.TProcEvento));
                    return (MultiSoftware.MDFe.v300.TProcEvento)serializer.Deserialize(stream);
                }
            }

            return null;
        }
    }

}

