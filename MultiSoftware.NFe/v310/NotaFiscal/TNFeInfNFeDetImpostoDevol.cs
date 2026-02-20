namespace MultiSoftware.NFe.v310.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeDetImpostoDevol
    {

        private string pDevolField;

        private TNFeInfNFeDetImpostoDevolIPI iPIField;

        /// <remarks/>
        public string pDevol
        {
            get
            {
                return this.pDevolField;
            }
            set
            {
                this.pDevolField = value;
            }
        }

        /// <remarks/>
        public TNFeInfNFeDetImpostoDevolIPI IPI
        {
            get
            {
                return this.iPIField;
            }
            set
            {
                this.iPIField = value;
            }
        }
    }
}
