namespace MultiSoftware.NFe.v400.NotaFiscalProcessada
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
    public partial class TProtNFe {
    
    private TProtNFeInfProt infProtField;
    
    private Assinatura.SignatureType signatureField;
    
    private string versaoField;
    
    /// <remarks/>
    public TProtNFeInfProt infProt {
        get {
            return this.infProtField;
        }
        set {
            this.infProtField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(Namespace="http://www.w3.org/2000/09/xmldsig#")]
    public  Assinatura.SignatureType Signature {
        get {
            return this.signatureField;
        }
        set {
            this.signatureField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string versao {
        get {
            return this.versaoField;
        }
        set {
            this.versaoField = value;
        }
    }
}
}
