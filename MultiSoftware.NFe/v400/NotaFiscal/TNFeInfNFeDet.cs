namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDet {
    
    private TNFeInfNFeDetProd prodField;
    
    private TNFeInfNFeDetImposto impostoField;
    
    private TNFeInfNFeDetImpostoDevol impostoDevolField;
    
    private string infAdProdField;
    
    private string nItemField;
    
    /// <remarks/>
    public TNFeInfNFeDetProd prod {
        get {
            return this.prodField;
        }
        set {
            this.prodField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeDetImposto imposto {
        get {
            return this.impostoField;
        }
        set {
            this.impostoField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeDetImpostoDevol impostoDevol {
        get {
            return this.impostoDevolField;
        }
        set {
            this.impostoDevolField = value;
        }
    }
    
    /// <remarks/>
    public string infAdProd {
        get {
            return this.infAdProdField;
        }
        set {
            this.infAdProdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nItem {
        get {
            return this.nItemField;
        }
        set {
            this.nItemField = value;
        }
    }
}
}
