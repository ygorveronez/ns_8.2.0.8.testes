namespace MultiSoftware.NFe.v310.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeInfAdicProcRef
    {

        private string nProcField;

        private TNFeInfNFeInfAdicProcRefIndProc indProcField;

        /// <remarks/>
        public string nProc
        {
            get
            {
                return this.nProcField;
            }
            set
            {
                this.nProcField = value;
            }
        }

        /// <remarks/>
        public TNFeInfNFeInfAdicProcRefIndProc indProc
        {
            get
            {
                return this.indProcField;
            }
            set
            {
                this.indProcField = value;
            }
        }
    }
}
