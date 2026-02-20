namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeDetImpostoICMSUFDest
    {

        private string vBCUFDestField;

        private string vBCFCPUFDestField;

        private string pFCPUFDestField;

        private string pICMSUFDestField;

        private TNFeInfNFeDetImpostoICMSUFDestPICMSInter pICMSInterField;

        private string pICMSInterPartField;

        private string vFCPUFDestField;

        private string vICMSUFDestField;

        private string vICMSUFRemetField;

        /// <remarks/>
        public string vBCUFDest
        {
            get
            {
                return this.vBCUFDestField;
            }
            set
            {
                this.vBCUFDestField = value;
            }
        }

        /// <remarks/>
        public string vBCFCPUFDest
        {
            get
            {
                return this.vBCFCPUFDestField;
            }
            set
            {
                this.vBCFCPUFDestField = value;
            }
        }

        /// <remarks/>
        public string pFCPUFDest
        {
            get
            {
                return this.pFCPUFDestField;
            }
            set
            {
                this.pFCPUFDestField = value;
            }
        }

        /// <remarks/>
        public string pICMSUFDest
        {
            get
            {
                return this.pICMSUFDestField;
            }
            set
            {
                this.pICMSUFDestField = value;
            }
        }

        /// <remarks/>
        public TNFeInfNFeDetImpostoICMSUFDestPICMSInter pICMSInter
        {
            get
            {
                return this.pICMSInterField;
            }
            set
            {
                this.pICMSInterField = value;
            }
        }

        /// <remarks/>
        public string pICMSInterPart
        {
            get
            {
                return this.pICMSInterPartField;
            }
            set
            {
                this.pICMSInterPartField = value;
            }
        }

        /// <remarks/>
        public string vFCPUFDest
        {
            get
            {
                return this.vFCPUFDestField;
            }
            set
            {
                this.vFCPUFDestField = value;
            }
        }

        /// <remarks/>
        public string vICMSUFDest
        {
            get
            {
                return this.vICMSUFDestField;
            }
            set
            {
                this.vICMSUFDestField = value;
            }
        }

        /// <remarks/>
        public string vICMSUFRemet
        {
            get
            {
                return this.vICMSUFRemetField;
            }
            set
            {
                this.vICMSUFRemetField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public enum TNFeInfNFeDetImpostoICMSUFDestPICMSInter
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4.00")]
        Item400 = 400,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7.00")]
        Item700 = 700,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("12.00")]
        Item1200 = 1200,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("4.0000")]
        Item40000 = 400,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("7.0000")]
        Item70000 = 700,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("12.0000")]
        Item120000 = 1200,
    }
}
