namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDetImpostoCOFINSST {
    
    private string[] itemsField;
    
    private ItemsChoiceType4[] itemsElementNameField;
    
    private string vCOFINSField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("pCOFINS", typeof(string))]
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
    public ItemsChoiceType4[] ItemsElementName {
        get {
            return this.itemsElementNameField;
        }
        set {
            this.itemsElementNameField = value;
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
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe", IncludeInSchema=false)]
public enum ItemsChoiceType4 {
    
    /// <remarks/>
    pCOFINS,
    
    /// <remarks/>
    qBCProd,
    
    /// <remarks/>
    vAliqProd,
    
    /// <remarks/>
    vBC,
}
}
