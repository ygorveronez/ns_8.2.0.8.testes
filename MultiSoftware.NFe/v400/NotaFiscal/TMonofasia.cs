namespace MultiSoftware.NFe.v400.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TMonofasia
    {

        private TMonofasiaGMonoPadrao gMonoPadraoField;

        private TMonofasiaGMonoReten gMonoRetenField;

        private TMonofasiaGMonoRet gMonoRetField;

        private TMonofasiaGMonoDif gMonoDifField;

        private string vTotIBSMonoItemField;

        private string vTotCBSMonoItemField;

        /// <remarks/>
        public TMonofasiaGMonoPadrao gMonoPadrao
        {
            get
            {
                return this.gMonoPadraoField;
            }
            set
            {
                this.gMonoPadraoField = value;
            }
        }

        /// <remarks/>
        public TMonofasiaGMonoReten gMonoReten
        {
            get
            {
                return this.gMonoRetenField;
            }
            set
            {
                this.gMonoRetenField = value;
            }
        }

        /// <remarks/>
        public TMonofasiaGMonoRet gMonoRet
        {
            get
            {
                return this.gMonoRetField;
            }
            set
            {
                this.gMonoRetField = value;
            }
        }

        /// <remarks/>
        public TMonofasiaGMonoDif gMonoDif
        {
            get
            {
                return this.gMonoDifField;
            }
            set
            {
                this.gMonoDifField = value;
            }
        }

        /// <remarks/>
        public string vTotIBSMonoItem
        {
            get
            {
                return this.vTotIBSMonoItemField;
            }
            set
            {
                this.vTotIBSMonoItemField = value;
            }
        }

        /// <remarks/>
        public string vTotCBSMonoItem
        {
            get
            {
                return this.vTotCBSMonoItemField;
            }
            set
            {
                this.vTotCBSMonoItemField = value;
            }
        }
    }
}
