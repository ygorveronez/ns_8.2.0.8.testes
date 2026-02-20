namespace MultiSoftware.NFe.NFeDistribuicaoDFe.DFe
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public partial class retDistDFeInt
    {

        private TAmb tpAmbField;

        private string verAplicField;

        private string cStatField;

        private string xMotivoField;

        private System.DateTime dhRespField;

        private string ultNSUField;

        private string maxNSUField;

        private retDistDFeIntLoteDistDFeInt loteDistDFeIntField;

        private TVerDistDFe versaoField;

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
        [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
        public string ultNSU
        {
            get
            {
                return this.ultNSUField;
            }
            set
            {
                this.ultNSUField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "token")]
        public string maxNSU
        {
            get
            {
                return this.maxNSUField;
            }
            set
            {
                this.maxNSUField = value;
            }
        }

        /// <remarks/>
        public retDistDFeIntLoteDistDFeInt loteDistDFeInt
        {
            get
            {
                return this.loteDistDFeIntField;
            }
            set
            {
                this.loteDistDFeIntField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TVerDistDFe versao
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
