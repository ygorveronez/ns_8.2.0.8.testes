namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeDetProdArma {
    
    private TNFeInfNFeDetProdArmaTpArma tpArmaField;
    
    private string nSerieField;
    
    private string nCanoField;
    
    private string descrField;
    
    /// <remarks/>
    public TNFeInfNFeDetProdArmaTpArma tpArma {
        get {
            return this.tpArmaField;
        }
        set {
            this.tpArmaField = value;
        }
    }
    
    /// <remarks/>
    public string nSerie {
        get {
            return this.nSerieField;
        }
        set {
            this.nSerieField = value;
        }
    }
    
    /// <remarks/>
    public string nCano {
        get {
            return this.nCanoField;
        }
        set {
            this.nCanoField = value;
        }
    }
    
    /// <remarks/>
    public string descr {
        get {
            return this.descrField;
        }
        set {
            this.descrField = value;
        }
    }
}
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeDetProdArmaTpArma {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("0")]
    Item0 = 0,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1 = 1,
}
}
