namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeCanaForDia {
    
    private string qtdeField;
    
    private string diaField;
    
    /// <remarks/>
    public string qtde {
        get {
            return this.qtdeField;
        }
        set {
            this.qtdeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string dia {
        get {
            return this.diaField;
        }
        set {
            this.diaField = value;
        }
    }
}
}
