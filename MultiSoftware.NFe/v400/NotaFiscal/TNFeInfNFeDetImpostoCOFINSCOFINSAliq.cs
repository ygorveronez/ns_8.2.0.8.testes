namespace MultiSoftware.NFe.v400.NotaFiscal
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDetImpostoCOFINSCOFINSAliq {
    
    private TNFeInfNFeDetImpostoCOFINSCOFINSAliqCST cSTField;
    
    private string vBCField;
    
    private string pCOFINSField;
    
    private string vCOFINSField;
    
    /// <remarks/>
    public TNFeInfNFeDetImpostoCOFINSCOFINSAliqCST CST {
        get {
            return this.cSTField;
        }
        set {
            this.cSTField = value;
        }
    }
    
    /// <remarks/>
    public string vBC {
        get {
            return this.vBCField;
        }
        set {
            this.vBCField = value;
        }
    }
    
    /// <remarks/>
    public string pCOFINS {
        get {
            return this.pCOFINSField;
        }
        set {
            this.pCOFINSField = value;
        }
    }
    
    /// <remarks/>
    public string vCOFINS {
        get {
            return this.vCOFINSField;
        }
        set {
            this.vCOFINSField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeDetImpostoCOFINSCOFINSAliqCST {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("01")]
    Item01 = 01,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("02")]
    Item02 = 02,
}
}
