namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeTranspRetTransp {
    
    private string vServField;
    
    private string vBCRetField;
    
    private string pICMSRetField;
    
    private string vICMSRetField;
    
    private string cFOPField;
    
    private string cMunFGField;
    
    /// <remarks/>
    public string vServ {
        get {
            return this.vServField;
        }
        set {
            this.vServField = value;
        }
    }
    
    /// <remarks/>
    public string vBCRet {
        get {
            return this.vBCRetField;
        }
        set {
            this.vBCRetField = value;
        }
    }
    
    /// <remarks/>
    public string pICMSRet {
        get {
            return this.pICMSRetField;
        }
        set {
            this.pICMSRetField = value;
        }
    }
    
    /// <remarks/>
    public string vICMSRet {
        get {
            return this.vICMSRetField;
        }
        set {
            this.vICMSRetField = value;
        }
    }
    
    /// <remarks/>
    public string CFOP {
        get {
            return this.cFOPField;
        }
        set {
            this.cFOPField = value;
        }
    }
    
    /// <remarks/>
    public string cMunFG {
        get {
            return this.cMunFGField;
        }
        set {
            this.cMunFGField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe", IncludeInSchema=false)]
public enum ItemsChoiceType5 {
    
    /// <remarks/>
    balsa,
    
    /// <remarks/>
    reboque,
    
    /// <remarks/>
    vagao,
    
    /// <remarks/>
    veicTransp,
}
}
