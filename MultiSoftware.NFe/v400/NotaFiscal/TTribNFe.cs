namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TTribNFe
    {
        private string cSTField;
        private string cClassTribField;
        private object itemField;
        private TCredPresIBSZFM gCredPresIBSZFMField;

        /// <remarks/>
        public string CST
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
        public string cClassTrib
        {
            get
            {
                return this.cClassTribField;
            }
            set
            {
                this.cClassTribField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("gIBSCBS", typeof(TCIBS))]
        [System.Xml.Serialization.XmlElementAttribute("gIBSCBSMono", typeof(TMonofasia))]
        [System.Xml.Serialization.XmlElementAttribute("gTransfCred", typeof(TTransfCred))]
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

        /// <remarks/>
        public TCredPresIBSZFM gCredPresIBSZFM
        {
            get
            {
                return this.gCredPresIBSZFMField;
            }
            set
            {
                this.gCredPresIBSZFMField = value;
            }
        }
    }
}
