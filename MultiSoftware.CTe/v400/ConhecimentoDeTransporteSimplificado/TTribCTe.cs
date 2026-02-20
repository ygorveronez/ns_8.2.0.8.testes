namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TTribCTe
    {

        private string cSTField;

        private string cClassTribField;

        private TCIBS gIBSCBSField;

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
        public TCIBS gIBSCBS
        {
            get
            {
                return this.gIBSCBSField;
            }
            set
            {
                this.gIBSCBSField = value;
            }
        }
    }
}