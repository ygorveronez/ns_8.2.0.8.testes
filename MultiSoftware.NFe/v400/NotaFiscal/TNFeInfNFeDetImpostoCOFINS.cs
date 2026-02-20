namespace MultiSoftware.NFe.v400.NotaFiscal
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDetImpostoCOFINS {
    
    private object itemField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("COFINSAliq", typeof(TNFeInfNFeDetImpostoCOFINSCOFINSAliq))]
    [System.Xml.Serialization.XmlElementAttribute("COFINSNT", typeof(TNFeInfNFeDetImpostoCOFINSCOFINSNT))]
    [System.Xml.Serialization.XmlElementAttribute("COFINSOutr", typeof(TNFeInfNFeDetImpostoCOFINSCOFINSOutr))]
    [System.Xml.Serialization.XmlElementAttribute("COFINSQtde", typeof(TNFeInfNFeDetImpostoCOFINSCOFINSQtde))]
    public object Item {
        get {
            return this.itemField;
        }
        set {
            this.itemField = value;
        }
    }
}
}
