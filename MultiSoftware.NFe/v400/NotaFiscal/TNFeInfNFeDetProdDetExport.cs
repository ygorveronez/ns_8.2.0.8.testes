namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDetProdDetExport {
    
    private string nDrawField;
    
    private TNFeInfNFeDetProdDetExportExportInd exportIndField;
    
    /// <remarks/>
    public string nDraw {
        get {
            return this.nDrawField;
        }
        set {
            this.nDrawField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeDetProdDetExportExportInd exportInd {
        get {
            return this.exportIndField;
        }
        set {
            this.exportIndField = value;
        }
    }
}
}
