using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace MultiSoftware.NFe.Servicos
{
    public class Assinatura
    {
        public static XmlElement AssinarXML(XmlNode elementoPai, XmlNode elementoAssinar, X509Certificate2 certificado)
        {
            string id = elementoAssinar.Attributes.GetNamedItem("Id").Value;

            SignedXml signedXml = new SignedXml((XmlElement)elementoAssinar);
            signedXml.SigningKey = certificado.PrivateKey;

            Reference reference = new Reference("#" + id);
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());

            signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
            reference.DigestMethod = SignedXml.XmlDsigSHA1Url;
            
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
    }
}
