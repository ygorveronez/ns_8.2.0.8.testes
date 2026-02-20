namespace MultiSoftware.NFe.NFeDownloadNF.DownloadNF
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe")]
    [System.Xml.Serialization.XmlRootAttribute("retDownloadNFe", Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public partial class TRetDownloadNFe
    {

        private TAmb tpAmbField;

        private string verAplicField;

        private string cStatField;

        private string xMotivoField;

        private System.DateTime dhRespField;

        private TRetDownloadNFeRetNFe[] retNFeField;

        private TVerDownloadNFe versaoField;

        /// <remarks/>
        public TAmb tpAmb
        {
            get
            {
                return this.tpAmbField;
            }
            set
            {
                this.tpAmbField = value;
            }
        }

        /// <remarks/>
        public string verAplic
        {
            get
            {
                return this.verAplicField;
            }
            set
            {
                this.verAplicField = value;
            }
        }

        /// <remarks/>
        public string cStat
        {
            get
            {
                return this.cStatField;
            }
            set
            {
                this.cStatField = value;
            }
        }

        /// <remarks/>
        public string xMotivo
        {
            get
            {
                return this.xMotivoField;
            }
            set
            {
                this.xMotivoField = value;
            }
        }

        /// <remarks/>
        public System.DateTime dhResp
        {
            get
            {
                return this.dhRespField;
            }
            set
            {
                this.dhRespField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("retNFe")]
        public TRetDownloadNFeRetNFe[] retNFe
        {
            get
            {
                return this.retNFeField;
            }
            set
            {
                this.retNFeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TVerDownloadNFe versao
        {
            get
            {
                return this.versaoField;
            }
            set
            {
                this.versaoField = value;
            }
        }
    }
}
