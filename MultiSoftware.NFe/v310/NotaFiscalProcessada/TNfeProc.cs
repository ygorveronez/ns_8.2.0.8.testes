namespace MultiSoftware.NFe.v310.NotaFiscalProcessada
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    [System.Xml.Serialization.XmlRootAttribute("nfeProc", Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public partial class TNfeProc
    {

        private NotaFiscal.TNFe nFeField;

        private TProtNFe protNFeField;

        private string versaoField;

        /// <remarks/>
        public NotaFiscal.TNFe NFe
        {
            get
            {
                return this.nFeField;
            }
            set
            {
                this.nFeField = value;
            }
        }

        /// <remarks/>
        public TProtNFe protNFe
        {
            get
            {
                return this.protNFeField;
            }
            set
            {
                this.protNFeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string versao
        {
            get
            {
                return this.versaoField;
            }
            set
            {
                this.versaoField = value;
            }
        }
    }
}
