namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCIBSGCBS
    {

        private string pCBSField;

        private TDif gDifField;

        private TDevTrib gDevTribField;

        private TRed gRedField;

        private string vCBSField;

        /// <remarks/>
        public string pCBS
        {
            get
            {
                return this.pCBSField;
            }
            set
            {
                this.pCBSField = value;
            }
        }

        /// <remarks/>
        public TDif gDif
        {
            get
            {
                return this.gDifField;
            }
            set
            {
                this.gDifField = value;
            }
        }

        /// <remarks/>
        public TDevTrib gDevTrib
        {
            get
            {
                return this.gDevTribField;
            }
            set
            {
                this.gDevTribField = value;
            }
        }

        /// <remarks/>
        public TRed gRed
        {
            get
            {
                return this.gRedField;
            }
            set
            {
                this.gRedField = value;
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
