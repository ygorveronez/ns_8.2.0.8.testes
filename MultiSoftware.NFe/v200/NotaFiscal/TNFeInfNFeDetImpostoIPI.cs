namespace MultiSoftware.NFe.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeDetImpostoIPI
    {

        private string clEnqField;

        private string cNPJProdField;

        private string cSeloField;

        private string qSeloField;

        private string cEnqField;

        private object itemField;

        /// <remarks/>
        public string clEnq
        {
            get
            {
                return this.clEnqField;
            }
            set
            {
                this.clEnqField = value;
            }
        }

        /// <remarks/>
        public string CNPJProd
        {
            get
            {
                return this.cNPJProdField;
            }
            set
            {
                this.cNPJProdField = value;
            }
        }

        /// <remarks/>
        public string cSelo
        {
            get
            {
                return this.cSeloField;
            }
            set
            {
                this.cSeloField = value;
            }
        }

        /// <remarks/>
        public string qSelo
        {
            get
            {
                return this.qSeloField;
            }
            set
            {
                this.qSeloField = value;
            }
        }

        /// <remarks/>
        public string cEnq
        {
            get
            {
                return this.cEnqField;
            }
            set
            {
                this.cEnqField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("IPINT", typeof(TNFeInfNFeDetImpostoIPIIPINT))]
        [System.Xml.Serialization.XmlElementAttribute("IPITrib", typeof(TNFeInfNFeDetImpostoIPIIPITrib))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }
}
