namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TDif
    {

        private string pDifField;

        private string vDifField;

        /// <remarks/>
        public string pDif
        {
            get
            {
                return this.pDifField;
            }
            set
            {
                this.pDifField = value;
            }
        }

        /// <remarks/>
        public string vDif
        {
            get
            {
                return this.vDifField;
            }
            set
            {
                this.vDifField = value;
            }
        }
    }
}
