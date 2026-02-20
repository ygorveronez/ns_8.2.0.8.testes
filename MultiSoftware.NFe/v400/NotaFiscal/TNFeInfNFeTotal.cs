namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeTotal {
    
    private TNFeInfNFeTotalICMSTot iCMSTotField;
    
    private TNFeInfNFeTotalISSQNtot iSSQNtotField;
    
    private TNFeInfNFeTotalRetTrib retTribField;
    
    /// <remarks/>
    public TNFeInfNFeTotalICMSTot ICMSTot {
        get {
            return this.iCMSTotField;
        }
        set {
            this.iCMSTotField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeTotalISSQNtot ISSQNtot {
        get {
            return this.iSSQNtotField;
        }
        set {
            this.iSSQNtotField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeTotalRetTrib retTrib {
        get {
            return this.retTribField;
        }
        set {
            this.retTribField = value;
        }
    }
}
}
