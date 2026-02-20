namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TIBSCBSMonoTot
    {
        private string vBCIBSCBSField;
        private TIBSCBSMonoTotGIBS gIBSField;
        private TIBSCBSMonoTotGCBS gCBSField;
        private TIBSCBSMonoTotGMono gMonoField;

        /// <remarks/>
        public string vBCIBSCBS
        {
            get
            {
                return this.vBCIBSCBSField;
            }
            set
            {
                this.vBCIBSCBSField = value;
            }
        }

        /// <remarks/>
        public TIBSCBSMonoTotGIBS gIBS
        {
            get
            {
                return this.gIBSField;
            }
            set
            {
                this.gIBSField = value;
            }
        }

        /// <remarks/>
        public TIBSCBSMonoTotGCBS gCBS
        {
            get
            {
                return this.gCBSField;
            }
            set
            {
                this.gCBSField = value;
            }
        }

        /// <remarks/>
        public TIBSCBSMonoTotGMono gMono
        {
            get
            {
                return this.gMonoField;
            }
            set
            {
                this.gMonoField = value;
            }
        }
    }
}
