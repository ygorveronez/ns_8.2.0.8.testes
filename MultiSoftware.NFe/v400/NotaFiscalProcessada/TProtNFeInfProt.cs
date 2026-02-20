namespace MultiSoftware.NFe.v400.NotaFiscalProcessada
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
    public partial class TProtNFeInfProt
    {
    
    private NotaFiscal.TAmb tpAmbField;
    
    private string verAplicField;
    
    private string chNFeField;
    
    private string dhRecbtoField;
    
    private string nProtField;
    
    private byte[] digValField;
    
    private string cStatField;
    
    private string xMotivoField;
    
    private string idField;
    
    /// <remarks/>
    public NotaFiscal.TAmb tpAmb {
        get {
            return this.tpAmbField;
        }
        set {
            this.tpAmbField = value;
        }
    }
    
    /// <remarks/>
    public string verAplic {
        get {
            return this.verAplicField;
        }
        set {
            this.verAplicField = value;
        }
    }
    
    /// <remarks/>
    public string chNFe {
        get {
            return this.chNFeField;
        }
        set {
            this.chNFeField = value;
        }
    }
    
    /// <remarks/>
    public string dhRecbto {
        get {
            return this.dhRecbtoField;
        }
        set {
            this.dhRecbtoField = value;
        }
    }
    
    /// <remarks/>
    public string nProt {
        get {
            return this.nProtField;
        }
        set {
            this.nProtField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")]
    public byte[] digVal {
        get {
            return this.digValField;
        }
        set {
            this.digValField = value;
        }
    }
    
    /// <remarks/>
    public string cStat {
        get {
            return this.cStatField;
        }
        set {
            this.cStatField = value;
        }
    }
    
    /// <remarks/>
    public string xMotivo {
        get {
            return this.xMotivoField;
        }
        set {
            this.xMotivoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="ID")]
    public string Id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}
}
