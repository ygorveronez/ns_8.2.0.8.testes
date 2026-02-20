namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDetImpostoPISPISOutr {
    
    private TNFeInfNFeDetImpostoPISPISOutrCST cSTField;
    
    private string[] itemsField;
    
    private ItemsChoiceType1[] itemsElementNameField;
    
    private string vPISField;
    
    /// <remarks/>
    public TNFeInfNFeDetImpostoPISPISOutrCST CST {
        get {
            return this.cSTField;
        }
        set {
            this.cSTField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("pPIS", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("qBCProd", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("vAliqProd", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("vBC", typeof(string))]
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
    public ItemsChoiceType1[] ItemsElementName {
        get {
            return this.itemsElementNameField;
        }
        set {
            this.itemsElementNameField = value;
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
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeDetImpostoPISPISOutrCST {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("49")]
    Item49 = 49,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("50")]
    Item50 = 50,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("51")]
    Item51 = 51,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("52")]
    Item52 = 52,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("53")]
    Item53 = 53,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("54")]
    Item54 = 54,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("55")]
    Item55 = 55,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("56")]
    Item56 = 56,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("60")]
    Item60 = 60,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("61")]
    Item61 = 61,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("62")]
    Item62 = 62,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("63")]
    Item63 = 63,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("64")]
    Item64 = 64,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("65")]
    Item65 = 65,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("66")]
    Item66 = 66,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("67")]
    Item67 = 67,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("70")]
    Item70 = 70,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("71")]
    Item71 = 71,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("72")]
    Item72 = 72,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("73")]
    Item73 = 73,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("74")]
    Item74 = 74,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("75")]
    Item75 = 75,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("98")]
    Item98 = 98,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("99")]
    Item99 = 99,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe", IncludeInSchema=false)]
public enum ItemsChoiceType1 {
    
    /// <remarks/>
    pPIS,
    
    /// <remarks/>
    qBCProd,
    
    /// <remarks/>
    vAliqProd,
    
    /// <remarks/>
    vBC,
}
}
