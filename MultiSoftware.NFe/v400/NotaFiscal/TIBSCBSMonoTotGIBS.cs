namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TIBSCBSMonoTotGIBS
    {
        private TIBSCBSMonoTotGIBSGIBSUF gIBSUFField;
        private TIBSCBSMonoTotGIBSGIBSMun gIBSMunField;
        private string vIBSField;
        private string vCredPresField;
        private string vCredPresCondSusField;

        /// <remarks/>
        public TIBSCBSMonoTotGIBSGIBSUF gIBSUF
        {
            get
            {
                return this.gIBSUFField;
            }
            set
            {
                this.gIBSUFField = value;
            }
        }

        /// <remarks/>
        public TIBSCBSMonoTotGIBSGIBSMun gIBSMun
        {
            get
            {
                return this.gIBSMunField;
            }
            set
            {
                this.gIBSMunField = value;
            }
        }

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
        public string vCredPres
        {
            get
            {
                return this.vCredPresField;
            }
            set
            {
                this.vCredPresField = value;
            }
        }

        /// <remarks/>
        public string vCredPresCondSus
        {
            get
            {
                return this.vCredPresCondSusField;
            }
            set
            {
                this.vCredPresCondSusField = value;
            }
        }
    }

}
