namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeInfAdicProcRef {
    
    private string nProcField;
    
    private TNFeInfNFeInfAdicProcRefIndProc indProcField;
    
    /// <remarks/>
    public string nProc {
        get {
            return this.nProcField;
        }
        set {
            this.nProcField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeInfAdicProcRefIndProc indProc {
        get {
            return this.indProcField;
        }
        set {
            this.indProcField = value;
        }
    }
}
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeInfAdicProcRefIndProc {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("0")]
    Item0 = 0,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1 = 1,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2")]
    Item2 = 2,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("3")]
    Item3 = 3,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("9")]
    Item9 = 9,
}
}
