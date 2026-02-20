namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDest {
    
    private string itemField;
    
    private ItemChoiceType3 itemElementNameField;
    
    private string xNomeField;
    
    private TEndereco enderDestField;
    
    private TNFeInfNFeDestIndIEDest indIEDestField;
    
    private string ieField;
    
    private string iSUFField;
    
    private string imField;
    
    private string emailField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("CNPJ", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("CPF", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("idEstrangeiro", typeof(string))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
    public string Item {
        get {
            return this.itemField;
        }
        set {
            this.itemField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemChoiceType3 ItemElementName {
        get {
            return this.itemElementNameField;
        }
        set {
            this.itemElementNameField = value;
        }
    }
    
    /// <remarks/>
    public string xNome {
        get {
            return this.xNomeField;
        }
        set {
            this.xNomeField = value;
        }
    }
    
    /// <remarks/>
    public TEndereco enderDest {
        get {
            return this.enderDestField;
        }
        set {
            this.enderDestField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeDestIndIEDest indIEDest {
        get {
            return this.indIEDestField;
        }
        set {
            this.indIEDestField = value;
        }
    }
    
    /// <remarks/>
    public string IE {
        get {
            return this.ieField;
        }
        set {
            this.ieField = value;
        }
    }
    
    /// <remarks/>
    public string ISUF {
        get {
            return this.iSUFField;
        }
        set {
            this.iSUFField = value;
        }
    }
    
    /// <remarks/>
    public string IM {
        get {
            return this.imField;
        }
        set {
            this.imField = value;
        }
    }
    
    /// <remarks/>
    public string email {
        get {
            return this.emailField;
        }
        set {
            this.emailField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe", IncludeInSchema=false)]
public enum ItemChoiceType3 {
    
    /// <remarks/>
    CNPJ,
    
    /// <remarks/>
    CPF,
    
    /// <remarks/>
    idEstrangeiro,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeDestIndIEDest {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1 = 1,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2")]
    Item2 = 2,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("9")]
    Item9 = 9,
}
}
