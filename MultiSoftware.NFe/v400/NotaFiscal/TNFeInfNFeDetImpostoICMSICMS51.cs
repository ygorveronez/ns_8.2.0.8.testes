namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDetImpostoICMSICMS51 {
    
    private Torig origField;
    
    private TNFeInfNFeDetImpostoICMSICMS51CST cSTField;
    
    private TNFeInfNFeDetImpostoICMSICMS51ModBC modBCField;
    
    private bool modBCFieldSpecified;
    
    private string pRedBCField;
    
    private string vBCField;
    
    private string pICMSField;
    
    private string vICMSOpField;
    
    private string pDifField;
    
    private string vICMSDifField;
    
    private string vICMSField;
    
    private string vBCFCPField;
    
    private string pFCPField;
    
    private string vFCPField;
    
    /// <remarks/>
    public Torig orig {
        get {
            return this.origField;
        }
        set {
            this.origField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeDetImpostoICMSICMS51CST CST {
        get {
            return this.cSTField;
        }
        set {
            this.cSTField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeDetImpostoICMSICMS51ModBC modBC {
        get {
            return this.modBCField;
        }
        set {
            this.modBCField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool modBCSpecified {
        get {
            return this.modBCFieldSpecified;
        }
        set {
            this.modBCFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public string pRedBC {
        get {
            return this.pRedBCField;
        }
        set {
            this.pRedBCField = value;
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
    public string pICMS {
        get {
            return this.pICMSField;
        }
        set {
            this.pICMSField = value;
        }
    }
    
    /// <remarks/>
    public string vICMSOp {
        get {
            return this.vICMSOpField;
        }
        set {
            this.vICMSOpField = value;
        }
    }
    
    /// <remarks/>
    public string pDif {
        get {
            return this.pDifField;
        }
        set {
            this.pDifField = value;
        }
    }
    
    /// <remarks/>
    public string vICMSDif {
        get {
            return this.vICMSDifField;
        }
        set {
            this.vICMSDifField = value;
        }
    }
    
    /// <remarks/>
    public string vICMS {
        get {
            return this.vICMSField;
        }
        set {
            this.vICMSField = value;
        }
    }
    
    /// <remarks/>
    public string vBCFCP {
        get {
            return this.vBCFCPField;
        }
        set {
            this.vBCFCPField = value;
        }
    }
    
    /// <remarks/>
    public string pFCP {
        get {
            return this.pFCPField;
        }
        set {
            this.pFCPField = value;
        }
    }
    
    /// <remarks/>
    public string vFCP {
        get {
            return this.vFCPField;
        }
        set {
            this.vFCPField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeDetImpostoICMSICMS51CST {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("51")]
    Item51 = 51,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeDetImpostoICMSICMS51ModBC {
    
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
}
}
