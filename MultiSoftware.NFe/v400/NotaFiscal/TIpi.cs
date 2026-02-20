namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TIpi {
    
    private string cNPJProdField;
    
    private string cSeloField;
    
    private string qSeloField;
    
    private string cEnqField;
    
    private object itemField;
    
    /// <remarks/>
    public string CNPJProd {
        get {
            return this.cNPJProdField;
        }
        set {
            this.cNPJProdField = value;
        }
    }
    
    /// <remarks/>
    public string cSelo {
        get {
            return this.cSeloField;
        }
        set {
            this.cSeloField = value;
        }
    }
    
    /// <remarks/>
    public string qSelo {
        get {
            return this.qSeloField;
        }
        set {
            this.qSeloField = value;
        }
    }
    
    /// <remarks/>
    public string cEnq {
        get {
            return this.cEnqField;
        }
        set {
            this.cEnqField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("IPINT", typeof(TIpiIPINT))]
    [System.Xml.Serialization.XmlElementAttribute("IPITrib", typeof(TIpiIPITrib))]
    public object Item {
        get {
            return this.itemField;
        }
        set {
            this.itemField = value;
        }
    }
}
}
