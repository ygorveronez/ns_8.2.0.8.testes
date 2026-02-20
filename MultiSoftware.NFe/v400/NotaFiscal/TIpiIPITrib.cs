namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TIpiIPITrib {
    
    private TIpiIPITribCST cSTField;
    
    private string[] itemsField;
    
    private ItemsChoiceType[] itemsElementNameField;
    
    private string vIPIField;
    
    /// <remarks/>
    public TIpiIPITribCST CST {
        get {
            return this.cSTField;
        }
        set {
            this.cSTField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("pIPI", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("qUnid", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("vBC", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("vUnid", typeof(string))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public string[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType[] ItemsElementName {
        get {
            return this.itemsElementNameField;
        }
        set {
            this.itemsElementNameField = value;
        }
    }
    
    /// <remarks/>
    public string vIPI {
        get {
            return this.vIPIField;
        }
        set {
            this.vIPIField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TIpiIPITribCST {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("00")]
    Item00 = 00,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("49")]
    Item49 = 49,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("50")]
    Item50 = 50,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("99")]
    Item99 = 99,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe", IncludeInSchema=false)]
public enum ItemsChoiceType {
    
    /// <remarks/>
    pIPI,
    
    /// <remarks/>
    qUnid,
    
    /// <remarks/>
    vBC,
    
    /// <remarks/>
    vUnid,
}
}
