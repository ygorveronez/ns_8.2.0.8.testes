namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public partial class TNFeInfNFeIde {
    
    private TCodUfIBGE cUFField;
    
    private string cNFField;
    
    private string natOpField;
    
    private TMod modField;
    
    private string serieField;
    
    private string nNFField;
    
    private string dhEmiField;
    
    private string dhSaiEntField;
    
    private TNFeInfNFeIdeTpNF tpNFField;
    
    private TNFeInfNFeIdeIdDest idDestField;
    
    private string cMunFGField;
    
    private TNFeInfNFeIdeTpImp tpImpField;
    
    private TNFeInfNFeIdeTpEmis tpEmisField;
    
    private string cDVField;
    
    private TAmb tpAmbField;
    
    private TFinNFe finNFeField;
    
    private TNFeInfNFeIdeIndFinal indFinalField;
    
    private TNFeInfNFeIdeIndPres indPresField;
    
    private TProcEmi procEmiField;
    
    private string verProcField;
    
    private string dhContField;
    
    private string xJustField;
    
    private TNFeInfNFeIdeNFref[] nFrefField;
    
    /// <remarks/>
    public TCodUfIBGE cUF {
        get {
            return this.cUFField;
        }
        set {
            this.cUFField = value;
        }
    }
    
    /// <remarks/>
    public string cNF {
        get {
            return this.cNFField;
        }
        set {
            this.cNFField = value;
        }
    }
    
    /// <remarks/>
    public string natOp {
        get {
            return this.natOpField;
        }
        set {
            this.natOpField = value;
        }
    }
    
    /// <remarks/>
    public TMod mod {
        get {
            return this.modField;
        }
        set {
            this.modField = value;
        }
    }
    
    /// <remarks/>
    public string serie {
        get {
            return this.serieField;
        }
        set {
            this.serieField = value;
        }
    }
    
    /// <remarks/>
    public string nNF {
        get {
            return this.nNFField;
        }
        set {
            this.nNFField = value;
        }
    }
    
    /// <remarks/>
    public string dhEmi {
        get {
            return this.dhEmiField;
        }
        set {
            this.dhEmiField = value;
        }
    }
    
    /// <remarks/>
    public string dhSaiEnt {
        get {
            return this.dhSaiEntField;
        }
        set {
            this.dhSaiEntField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeIdeTpNF tpNF {
        get {
            return this.tpNFField;
        }
        set {
            this.tpNFField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeIdeIdDest idDest {
        get {
            return this.idDestField;
        }
        set {
            this.idDestField = value;
        }
    }
    
    /// <remarks/>
    public string cMunFG {
        get {
            return this.cMunFGField;
        }
        set {
            this.cMunFGField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeIdeTpImp tpImp {
        get {
            return this.tpImpField;
        }
        set {
            this.tpImpField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeIdeTpEmis tpEmis {
        get {
            return this.tpEmisField;
        }
        set {
            this.tpEmisField = value;
        }
    }
    
    /// <remarks/>
    public string cDV {
        get {
            return this.cDVField;
        }
        set {
            this.cDVField = value;
        }
    }
    
    /// <remarks/>
    public TAmb tpAmb {
        get {
            return this.tpAmbField;
        }
        set {
            this.tpAmbField = value;
        }
    }
    
    /// <remarks/>
    public TFinNFe finNFe {
        get {
            return this.finNFeField;
        }
        set {
            this.finNFeField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeIdeIndFinal indFinal {
        get {
            return this.indFinalField;
        }
        set {
            this.indFinalField = value;
        }
    }
    
    /// <remarks/>
    public TNFeInfNFeIdeIndPres indPres {
        get {
            return this.indPresField;
        }
        set {
            this.indPresField = value;
        }
    }
    
    /// <remarks/>
    public TProcEmi procEmi {
        get {
            return this.procEmiField;
        }
        set {
            this.procEmiField = value;
        }
    }
    
    /// <remarks/>
    public string verProc {
        get {
            return this.verProcField;
        }
        set {
            this.verProcField = value;
        }
    }
    
    /// <remarks/>
    public string dhCont {
        get {
            return this.dhContField;
        }
        set {
            this.dhContField = value;
        }
    }
    
    /// <remarks/>
    public string xJust {
        get {
            return this.xJustField;
        }
        set {
            this.xJustField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("NFref")]
    public TNFeInfNFeIdeNFref[] NFref {
        get {
            return this.nFrefField;
        }
        set {
            this.nFrefField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TCodUfIBGE {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("11")]
    Item11 = 11,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("12")]
    Item12 = 12,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("13")]
    Item13 = 13,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("14")]
    Item14 = 14,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("15")]
    Item15 = 15,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("16")]
    Item16 = 16,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("17")]
    Item17 = 17,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("21")]
    Item21 = 21,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("22")]
    Item22 = 22,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("23")]
    Item23 = 23,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("24")]
    Item24 = 24,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("25")]
    Item25 = 25,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("26")]
    Item26 = 26,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("27")]
    Item27 = 27,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("28")]
    Item28 = 28,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("29")]
    Item29 = 29,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("31")]
    Item31 = 31,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("32")]
    Item32 = 32,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("33")]
    Item33 = 33,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("35")]
    Item35 = 35,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("41")]
    Item41 = 41,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("42")]
    Item42 = 42,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("43")]
    Item43 = 43,
    
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
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TMod {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("55")]
    Item55 = 55,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("65")]
    Item65 = 65,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeIdeTpNF {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("0")]
    Item0 = 0,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1 = 1,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeIdeIdDest {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1 = 1,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2")]
    Item2 = 2,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("3")]
    Item3 = 3,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeIdeTpImp {
    
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
    [System.Xml.Serialization.XmlEnumAttribute("5")]
    Item5 = 5,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeIdeTpEmis {
    
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
    [System.Xml.Serialization.XmlEnumAttribute("5")]
    Item5 = 5,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("6")]
    Item6 = 6,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("7")]
    Item7 = 7,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("9")]
    Item9 = 9,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TAmb {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1 = 1,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2")]
    Item2 = 2,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TFinNFe {
    
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
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeIdeIndFinal {
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("0")]
    Item0 = 0,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("1")]
    Item1 = 1,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TNFeInfNFeIdeIndPres {
    
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
    [System.Xml.Serialization.XmlEnumAttribute("5")]
    Item5 = 5,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("9")]
    Item9 = 9,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
public enum TProcEmi {
    
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
}
}
