namespace MultiSoftware.NFe.v400.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeDetImpostoICMSICMS61
    {

        private Torig origField;

        private TNFeInfNFeDetImpostoICMSICMS61CST cSTField;

        private string qBCMonoRetField;

        private string adRemICMSRetField;

        private string vICMSMonoRetField;

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
        public TNFeInfNFeDetImpostoICMSICMS61CST CST
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
        public string qBCMonoRet
        {
            get
            {
                return this.qBCMonoRetField;
            }
            set
            {
                this.qBCMonoRetField = value;
            }
        }

        /// <remarks/>
        public string adRemICMSRet
        {
            get
            {
                return this.adRemICMSRetField;
            }
            set
            {
                this.adRemICMSRetField = value;
            }
        }

        /// <remarks/>
        public string vICMSMonoRet
        {
            get
            {
                return this.vICMSMonoRetField;
            }
            set
            {
                this.vICMSMonoRetField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public enum TNFeInfNFeDetImpostoICMSICMS61CST
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("61")]
        Item61 = 61,
    }
}
