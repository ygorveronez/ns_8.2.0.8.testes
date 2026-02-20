namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeTransp {
    
    private TNFeInfNFeTranspModFrete modFreteField;
    
    private TNFeInfNFeTranspTransporta transportaField;
    
    private TNFeInfNFeTranspRetTransp retTranspField;
    
    private object[] itemsField;
    
    private ItemsChoiceType5[] itemsElementNameField;
    
    private TNFeInfNFeTranspVol[] volField;
    
    /// <remarks/>
    public TNFeInfNFeTranspModFrete modFrete {
        get {
            return this.modFreteField;
        }
        set {
            this.modFreteField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeTranspTransporta transporta {
        get {
            return this.transportaField;
        }
        set {
            this.transportaField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeTranspRetTransp retTransp {
        get {
            return this.retTranspField;
        }
        set {
            this.retTranspField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("balsa", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("reboque", typeof(TVeiculo))]
    [System.Xml.Serialization.XmlElementAttribute("vagao", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("veicTransp", typeof(TVeiculo))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public object[] Items {
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
    public ItemsChoiceType5[] ItemsElementName {
        get {
            return this.itemsElementNameField;
        }
        set {
            this.itemsElementNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("vol")]
    public TNFeInfNFeTranspVol[] vol {
        get {
            return this.volField;
        }
        set {
            this.volField = value;
        }
    }
}
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeTranspModFrete {
    
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
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("4")]
    Item4 = 4,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("9")]
    Item9 = 9,
}
}
