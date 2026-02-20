namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TTransfCred
    {
        private string vIBSField;
        private string vCBSField;

        /// <remarks/>
        public string vIBS
        {
            get
            {
                return this.vIBSField;
            }
            set
            {
                this.vIBSField = value;
            }
        }

        /// <remarks/>
        public string vCBS
        {
            get
            {
                return this.vCBSField;
            }
            set
            {
                this.vCBSField = value;
            }
        }
    }
}
