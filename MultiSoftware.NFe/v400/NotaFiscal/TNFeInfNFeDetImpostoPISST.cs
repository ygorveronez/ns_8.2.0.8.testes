namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDetImpostoPISST {
    
    private string[] itemsField;
    
    private ItemsChoiceType2[] itemsElementNameField;
    
    private string vPISField;
    
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
    public ItemsChoiceType2[] ItemsElementName {
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
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe", IncludeInSchema=false)]
public enum ItemsChoiceType2 {
    
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
