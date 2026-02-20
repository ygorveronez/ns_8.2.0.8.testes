namespace MultiSoftware.MDFe.v300.ConsultaSituacaoMDFe
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/mdfe")]
    [System.Xml.Serialization.XmlRootAttribute("retConsSitMDFe", Namespace = "http://www.portalfiscal.inf.br/mdfe", IsNullable = false)]
    public partial class TRetConsSitMDFe
    {

        private TAmb tpAmbField;

        private string verAplicField;

        private string cStatField;

        private string xMotivoField;

        private TCodUfIBGE cUFField;

        private TProtMDFe protMDFeField;

        private TProcEvento[] procEventoMDFeField;

        private string versaoField;

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
        public TCodUfIBGE cUF
        {
            get
            {
                return this.cUFField;
            }
            set
            {
                this.cUFField = value;
            }
        }

        /// <remarks/>
        public TProtMDFe protMDFe
        {
            get
            {
                return this.protMDFeField;
            }
            set
            {
                this.protMDFeField = value;
            }
        }

        ///// <remarks/>
        [System.Xml.Serialization.XmlElement("procEventoMDFe")]
        public TProcEvento[] procEventoMDFe
        {
            get
            {
                return this.procEventoMDFeField;
            }
            set
            {
                this.procEventoMDFeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string versao
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
