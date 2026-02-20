namespace MultiSoftware.NFe.v400.Assinatura
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
public partial class X509DataType {
    
    private byte[] x509CertificateField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
    public byte[] X509Certificate {
        get {
            return this.x509CertificateField;
        }
        set {
            this.x509CertificateField = value;
        }
    }
}
}
