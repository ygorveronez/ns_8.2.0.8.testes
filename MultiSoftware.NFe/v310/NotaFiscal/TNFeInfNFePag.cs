namespace MultiSoftware.NFe.v310.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFePag
    {

        private TNFeInfNFePagTPag tPagField;

        private string vPagField;

        private TNFeInfNFePagCard cardField;

        /// <remarks/>
        public TNFeInfNFePagTPag tPag
        {
            get
            {
                return this.tPagField;
            }
            set
            {
                this.tPagField = value;
            }
        }

        /// <remarks/>
        public string vPag
        {
            get
            {
                return this.vPagField;
            }
            set
            {
                this.vPagField = value;
            }
        }

        /// <remarks/>
        public TNFeInfNFePagCard card
        {
            get
            {
                return this.cardField;
            }
            set
            {
                this.cardField = value;
            }
        }
    }
}
