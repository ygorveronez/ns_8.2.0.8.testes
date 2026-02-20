namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TCIBSGIBSUF
    {
        private string pIBSUFField;
        private TDif gDifField;
        private TDevTrib gDevTribField;
        private TRed gRedField;
        private string vIBSUFField;

        /// <remarks/>
        public string pIBSUF
        {
            get
            {
                return this.pIBSUFField;
            }
            set
            {
                this.pIBSUFField = value;
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
        public string vIBSUF
        {
            get
            {
                return this.vIBSUFField;
            }
            set
            {
                this.vIBSUFField = value;
            }
        }
    }
}
