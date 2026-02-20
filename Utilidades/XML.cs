using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Utilidades
{
    public class XML
    {
        public static string Serializar<T>(T objeto, bool indentar = false)
        {
            if (objeto == null)
                return string.Empty;

            using (StringWriter escritorTexto = new StringWriter())
            {
                XmlWriterSettings configuracoesXml = new XmlWriterSettings()
                {
                    Indent = indentar,
                    OmitXmlDeclaration = true
                };

                using (XmlWriter escritorXml = XmlWriter.Create(escritorTexto, configuracoesXml))
                {
                    XmlSerializer serializadorXml = new XmlSerializer(typeof(T));
                    XmlSerializerNamespaces namespaceXml = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

                    serializadorXml.Serialize(escritorXml, objeto, namespaceXml);

                    return escritorTexto.ToString();
                }
            }
        }

        public static T Deserializar<T>(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
                return default(T);

            using (TextReader leitorTexto = new StringReader(xml))
            {
                XmlSerializer serializadorXml = new XmlSerializer(typeof(T));

                return (T)serializadorXml.Deserialize(leitorTexto);
            }
        }

        public static XmlNode StringParaXmlNode(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            return xmlDoc.DocumentElement;
        }

        public static string ObterConteudoTag(string xml, string tag, bool incluirTagNaBusca = false)
        {
            Regex expressaoRegular = new Regex($@"({(incluirTagNaBusca ? "" : "?:")}<{tag}>)([\s\S]+)({(incluirTagNaBusca ? "" : "?:")}<\/{tag}>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection resultadosCompativeis = expressaoRegular.Matches(xml);

            if (resultadosCompativeis.Count == 0)
                return null;

            Match primeiroResultadoCompativel = resultadosCompativeis[0];

            if (primeiroResultadoCompativel.Groups.Count < 0)
                return null;

            return primeiroResultadoCompativel.Groups[incluirTagNaBusca ? 0 : 1]?.Value?.Trim();
        }

        public static List<string> ObterErrosValidacaoSchema(string xml, string schemaUri)
        {
            List<string> errosValidacao = new List<string>();

            if (string.IsNullOrWhiteSpace(xml) || !Utilidades.IO.FileStorageService.Storage.Exists(schemaUri))
                return errosValidacao;

            XmlReaderSettings configuracoesLeitorXml = new XmlReaderSettings();

            configuracoesLeitorXml.Schemas.Add(null, schemaUri);
            configuracoesLeitorXml.ValidationType = ValidationType.Schema;
            configuracoesLeitorXml.ValidationEventHandler += new System.Xml.Schema.ValidationEventHandler((object sender, System.Xml.Schema.ValidationEventArgs e) =>
            {
                if (e.Severity == System.Xml.Schema.XmlSeverityType.Error)
                    errosValidacao.Add(e.Message);
            });

            MemoryStream arquivoXml = String.ToStream(xml);
            XmlReader leitorXml = XmlReader.Create(arquivoXml, configuracoesLeitorXml);

            while (leitorXml.Read()) { }

            return errosValidacao;
        }
    }
}
