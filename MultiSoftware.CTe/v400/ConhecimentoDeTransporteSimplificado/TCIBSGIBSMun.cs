namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCIBSGIBSMun
    {

        private string pIBSMunField;

        private TDifIBS gDifField;

        private TDevTrib gDevTribField;

        private TRed gRedField;

        private string vIBSMunField;

        /// <remarks/>
        public string pIBSMun
        {
            get
            {
                return this.pIBSMunField;
            }
            set
            {
                this.pIBSMunField = value;
            }
        }

        /// <remarks/>
        public TDifIBS gDif
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
        public string vIBSMun
        {
            get
            {
                return this.vIBSMunField;
            }
            set
            {
                this.vIBSMunField = value;
            }
        }
    }
}