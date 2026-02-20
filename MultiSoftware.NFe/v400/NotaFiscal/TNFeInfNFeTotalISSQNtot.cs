namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeTotalISSQNtot {
    
    private string vServField;
    
    private string vBCField;
    
    private string vISSField;
    
    private string vPISField;
    
    private string vCOFINSField;
    
    private string dCompetField;
    
    private string vDeducaoField;
    
    private string vOutroField;
    
    private string vDescIncondField;
    
    private string vDescCondField;
    
    private string vISSRetField;
    
    private TNFeInfNFeTotalISSQNtotCRegTrib cRegTribField;
    
    private bool cRegTribFieldSpecified;
    
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
    public string vBC {
        get {
            return this.vBCField;
        }
        set {
            this.vBCField = value;
        }
    }
    
    /// <remarks/>
    public string vISS {
        get {
            return this.vISSField;
        }
        set {
            this.vISSField = value;
        }
    }
    
    /// <remarks/>
    public string vPIS {
        get {
            return this.vPISField;
        }
        set {
            this.vPISField = value;
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
    
    /// <remarks/>
    public string dCompet {
        get {
            return this.dCompetField;
        }
        set {
            this.dCompetField = value;
        }
    }
    
    /// <remarks/>
    public string vDeducao {
        get {
            return this.vDeducaoField;
        }
        set {
            this.vDeducaoField = value;
        }
    }
    
    /// <remarks/>
    public string vOutro {
        get {
            return this.vOutroField;
        }
        set {
            this.vOutroField = value;
        }
    }
    
    /// <remarks/>
    public string vDescIncond {
        get {
            return this.vDescIncondField;
        }
        set {
            this.vDescIncondField = value;
        }
    }
    
    /// <remarks/>
    public string vDescCond {
        get {
            return this.vDescCondField;
        }
        set {
            this.vDescCondField = value;
        }
    }
    
    /// <remarks/>
    public string vISSRet {
        get {
            return this.vISSRetField;
        }
        set {
            this.vISSRetField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeTotalISSQNtotCRegTrib cRegTrib {
        get {
            return this.cRegTribField;
        }
        set {
            this.cRegTribField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool cRegTribSpecified {
        get {
            return this.cRegTribFieldSpecified;
        }
        set {
            this.cRegTribFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeTotalISSQNtotCRegTrib {
    
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
    [System.Xml.Serialization.XmlEnumAttribute("4")]
    Item4 = 4,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("5")]
    Item5 = 5,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("6")]
    Item6 = 6,
}
}
