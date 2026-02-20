using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace MultiSoftware.CTe.Servicos
{
    public class Assinatura
    {
        public static XmlElement AssinarXML(XmlNode elementoPai, XmlNode elementoAssinar, X509Certificate2 certificado, bool NFSe = false)
        {
            string id = NFSe == true ? string.Empty : "#" + elementoAssinar.Attributes.GetNamedItem("Id").Value;

            SignedXml signedXml = new SignedXml((XmlElement)elementoAssinar);
            signedXml.SigningKey = certificado.PrivateKey;

            Reference reference = new Reference(id);
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            signedXml.AddReference(reference);

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));
            signedXml.KeyInfo = keyInfo;

            signedXml.ComputeSignature();

            XmlDocument documento = new XmlDocument();

            XmlElement xmlSignature = documento.CreateElement("Signature", "http://www.w3.org/2000/09/xmldsig#");
            XmlElement xmlSignedInfo = signedXml.SignedInfo.GetXml();
            XmlElement xmlKeyInfo = signedXml.KeyInfo.GetXml();

            XmlElement xmlSignatureValue = documento.CreateElement("SignatureValue", xmlSignature.NamespaceURI);
            string signBase64 = Convert.ToBase64String(signedXml.Signature.SignatureValue);
            XmlText text = documento.CreateTextNode(signBase64);
            xmlSignatureValue.AppendChild(text);

            xmlSignature.AppendChild(documento.ImportNode(xmlSignedInfo, true));
            xmlSignature.AppendChild(xmlSignatureValue);
            xmlSignature.AppendChild(documento.ImportNode(xmlKeyInfo, true));

            elementoPai.AppendChild(elementoPai.OwnerDocument.ImportNode(xmlSignature, true));

            return (XmlElement)elementoPai;
        }

        public static XmlDocument AssinarXmlManual(XmlDocument xmlDoc, X509Certificate2 certificado, string referencia)
        {
            string xmlSemDeclaracao;
            using (StringWriter stringWriter = new StringWriter())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true, // Remove a declaração XML
                    Encoding = Encoding.UTF8,
                    Indent = false
                };

                using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    xmlDoc.Save(xmlWriter);
                }

                xmlSemDeclaracao = stringWriter.ToString();
            }

            // Carregar o XML sem a declaração novamente no XmlDocument
            XmlDocument xmlDocSemDeclaracao = new XmlDocument();
            xmlDocSemDeclaracao.LoadXml(xmlSemDeclaracao);

            SignedXml signedXml = new SignedXml(xmlDoc)
            {
                SigningKey = certificado.PrivateKey
            };

            Reference reference = new Reference
            {
                Uri = "#" + referencia
            };

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());

            reference.DigestMethod = SignedXml.XmlDsigSHA1Url;

            signedXml.AddReference(reference);

            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));
            signedXml.KeyInfo = keyInfo;

            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;

            signedXml.ComputeSignature();

            XmlElement xmlDigitalSignature = signedXml.GetXml();
            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

            return xmlDoc;
        }
    }
}
