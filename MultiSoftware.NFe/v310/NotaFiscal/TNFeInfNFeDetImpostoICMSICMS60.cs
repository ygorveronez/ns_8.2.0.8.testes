namespace MultiSoftware.NFe.v310.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeDetImpostoICMSICMS60
    {

        private Torig origField;

        private TNFeInfNFeDetImpostoICMSICMS60CST cSTField;

        private string vBCSTRetField;

        private string vICMSSTRetField;

        /// <remarks/>
        public Torig orig
        {
            get
            {
                return this.origField;
            }
            set
            {
                this.origField = value;
            }
        }

        /// <remarks/>
        public TNFeInfNFeDetImpostoICMSICMS60CST CST
        {
            get
            {
                return this.cSTField;
            }
            set
            {
                this.cSTField = value;
            }
        }

        /// <remarks/>
        public string vBCSTRet
        {
            get
            {
                return this.vBCSTRetField;
            }
            set
            {
                this.vBCSTRetField = value;
            }
        }

        /// <remarks/>
        public string vICMSSTRet
        {
            get
            {
                return this.vICMSSTRetField;
            }
            set
            {
                this.vICMSSTRetField = value;
            }
        }
    }
}
