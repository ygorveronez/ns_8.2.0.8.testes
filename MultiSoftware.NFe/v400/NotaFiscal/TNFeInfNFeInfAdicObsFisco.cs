namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeInfAdicObsFisco {
    
    private string xTextoField;
    
    private string xCampoField;
    
    /// <remarks/>
    public string xTexto {
        get {
            return this.xTextoField;
        }
        set {
            this.xTextoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string xCampo {
        get {
            return this.xCampoField;
        }
        set {
            this.xCampoField = value;
        }
    }
}
}
