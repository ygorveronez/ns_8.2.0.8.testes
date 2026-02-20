namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeIdeNFrefRefECF
    {

        private TNFeInfNFeIdeNFrefRefECFMod modField;

        private string nECFField;

        private string nCOOField;

        /// <remarks/>
        public TNFeInfNFeIdeNFrefRefECFMod mod
        {
            get
            {
                return this.modField;
            }
            set
            {
                this.modField = value;
            }
        }

        /// <remarks/>
        public string nECF
        {
            get
            {
                return this.nECFField;
            }
            set
            {
                this.nECFField = value;
            }
        }

        /// <remarks/>
        public string nCOO
        {
            get
            {
                return this.nCOOField;
            }
            set
            {
                this.nCOOField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public enum TNFeInfNFeIdeNFrefRefECFMod
    {

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute("2B")]
            Item2B,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute("2C")]
            Item2C,

           /// <remarks/>
           [System.Xml.Serialization.XmlEnumAttribute("2D")]
           Item2D,
    }
}
