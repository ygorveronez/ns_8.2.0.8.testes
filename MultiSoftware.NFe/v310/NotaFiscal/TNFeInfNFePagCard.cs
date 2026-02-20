namespace MultiSoftware.NFe.v310.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFePagCard
    {

        private string cNPJField;

        private TNFeInfNFePagCardTBand tBandField;

        private string cAutField;

        /// <remarks/>
        public string CNPJ
        {
            get
            {
                return this.cNPJField;
            }
            set
            {
                this.cNPJField = value;
            }
        }

        /// <remarks/>
        public TNFeInfNFePagCardTBand tBand
        {
            get
            {
                return this.tBandField;
            }
            set
            {
                this.tBandField = value;
            }
        }

        /// <remarks/>
        public string cAut
        {
            get
            {
                return this.cAutField;
            }
            set
            {
                this.cAutField = value;
            }
        }
    }
}
