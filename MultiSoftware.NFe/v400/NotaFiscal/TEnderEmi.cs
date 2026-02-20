namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TEnderEmi {
    
    private string xLgrField;
    
    private string nroField;
    
    private string xCplField;
    
    private string xBairroField;
    
    private string cMunField;
    
    private string xMunField;
    
    private TUfEmi ufField;
    
    private string cEPField;
    
    private TEnderEmiCPais cPaisField;
    
    private bool cPaisFieldSpecified;
    
    private TEnderEmiXPais xPaisField;
    
    private bool xPaisFieldSpecified;
    
    private string foneField;
    
    /// <remarks/>
    public string xLgr {
        get {
            return this.xLgrField;
        }
        set {
            this.xLgrField = value;
        }
    }
    
    /// <remarks/>
    public string nro {
        get {
            return this.nroField;
        }
        set {
            this.nroField = value;
        }
    }
    
    /// <remarks/>
    public string xCpl {
        get {
            return this.xCplField;
        }
        set {
            this.xCplField = value;
        }
    }
    
    /// <remarks/>
    public string xBairro {
        get {
            return this.xBairroField;
        }
        set {
            this.xBairroField = value;
        }
    }
    
    /// <remarks/>
    public string cMun {
        get {
            return this.cMunField;
        }
        set {
            this.cMunField = value;
        }
    }
    
    /// <remarks/>
    public string xMun {
        get {
            return this.xMunField;
        }
        set {
            this.xMunField = value;
        }
    }
    
    /// <remarks/>
    public TUfEmi UF {
        get {
            return this.ufField;
        }
        set {
            this.ufField = value;
        }
    }
    
    /// <remarks/>
    public string CEP {
        get {
            return this.cEPField;
        }
        set {
            this.cEPField = value;
        }
    }
    
    /// <remarks/>
    public TEnderEmiCPais cPais {
        get {
            return this.cPaisField;
        }
        set {
            this.cPaisField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool cPaisSpecified {
        get {
            return this.cPaisFieldSpecified;
        }
        set {
            this.cPaisFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public TEnderEmiXPais xPais {
        get {
            return this.xPaisField;
        }
        set {
            this.xPaisField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool xPaisSpecified {
        get {
            return this.xPaisFieldSpecified;
        }
        set {
            this.xPaisFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public string fone {
        get {
            return this.foneField;
        }
        set {
            this.foneField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TUfEmi {
    
    /// <remarks/>
    AC,
    
    /// <remarks/>
    AL,
    
    /// <remarks/>
    AM,
    
    /// <remarks/>
    AP,
    
    /// <remarks/>
    BA,
    
    /// <remarks/>
    CE,
    
    /// <remarks/>
    DF,
    
    /// <remarks/>
    ES,
    
    /// <remarks/>
    GO,
    
    /// <remarks/>
    MA,
    
    /// <remarks/>
    MG,
    
    /// <remarks/>
    MS,
    
    /// <remarks/>
    MT,
    
    /// <remarks/>
    PA,
    
    /// <remarks/>
    PB,
    
    /// <remarks/>
    PE,
    
    /// <remarks/>
    PI,
    
    /// <remarks/>
    PR,
    
    /// <remarks/>
    RJ,
    
    /// <remarks/>
    RN,
    
    /// <remarks/>
    RO,
    
    /// <remarks/>
    RR,
    
    /// <remarks/>
    RS,
    
    /// <remarks/>
    SC,
    
    /// <remarks/>
    SE,
    
    /// <remarks/>
    SP,
    
    /// <remarks/>
    TO,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TEnderEmiCPais {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1058")]
    Item1058 = 1058,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TEnderEmiXPais {
    
    /// <remarks/>
    Brasil,
    
    /// <remarks/>
    BRASIL,
}
}
