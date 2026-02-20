namespace MultiSoftware.NFe.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeDetProdDI
    {

        private string nDIField;

        private string dDIField;

        private string xLocDesembField;

        private TUfEmi uFDesembField;

        private string dDesembField;

        private string cExportadorField;

        private TNFeInfNFeDetProdDIAdi[] adiField;

        /// <remarks/>
        public string nDI
        {
            get
            {
                return this.nDIField;
            }
            set
            {
                this.nDIField = value;
            }
        }

        /// <remarks/>
        public string dDI
        {
            get
            {
                return this.dDIField;
            }
            set
            {
                this.dDIField = value;
            }
        }

        /// <remarks/>
        public string xLocDesemb
        {
            get
            {
                return this.xLocDesembField;
            }
            set
            {
                this.xLocDesembField = value;
            }
        }

        /// <remarks/>
        public TUfEmi UFDesemb
        {
            get
            {
                return this.uFDesembField;
            }
            set
            {
                this.uFDesembField = value;
            }
        }

        /// <remarks/>
        public string dDesemb
        {
            get
            {
                return this.dDesembField;
            }
            set
            {
                this.dDesembField = value;
            }
        }

        /// <remarks/>
        public string cExportador
        {
            get
            {
                return this.cExportadorField;
            }
            set
            {
                this.cExportadorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("adi")]
        public TNFeInfNFeDetProdDIAdi[] adi
        {
            get
            {
                return this.adiField;
            }
            set
            {
                this.adiField = value;
            }
        }
    }
}
