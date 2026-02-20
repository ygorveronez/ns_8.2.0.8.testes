namespace MultiSoftware.CTe.v104.ConhecimentoDeTransporteInutilizado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TInutCTeInfInut
    {

        private ConhecimentoDeTransporte.TAmb tpAmbField;

        private string xServField;

        private ConhecimentoDeTransporte.TCodUfIBGE cUFField;

        private short anoField;

        private string cNPJField;

        private ConhecimentoDeTransporte.TModCT modField;

        private string serieField;

        private string nCTIniField;

        private string nCTFinField;

        private string xJustField;

        private string idField;

        public TInutCTeInfInut()
        {
            this.xServField = "INUTILIZAR";
        }

        /// <remarks/>
        public ConhecimentoDeTransporte.TAmb tpAmb
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
        public string xServ
        {
            get
            {
                return this.xServField;
            }
            set
            {
                this.xServField = value;
            }
        }

        /// <remarks/>
        public ConhecimentoDeTransporte.TCodUfIBGE cUF
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
        public short ano
        {
            get
            {
                return this.anoField;
            }
            set
            {
                this.anoField = value;
            }
        }

        /// <remarks/>
        public string CNPJ
        {
            get
            {
                return this.cNPJField;
            }
            set
            {
                this.cNPJField = value;
            }
        }

        /// <remarks/>
        public ConhecimentoDeTransporte.TModCT mod
        {
            get
            {
                return this.modField;
            }
            set
            {
                this.modField = value;
            }
        }

        /// <remarks/>
        public string serie
        {
            get
            {
                return this.serieField;
            }
            set
            {
                this.serieField = value;
            }
        }

        /// <remarks/>
        public string nCTIni
        {
            get
            {
                return this.nCTIniField;
            }
            set
            {
                this.nCTIniField = value;
            }
        }

        /// <remarks/>
        public string nCTFin
        {
            get
            {
                return this.nCTFinField;
            }
            set
            {
                this.nCTFinField = value;
            }
        }

        /// <remarks/>
        public string xJust
        {
            get
            {
                return this.xJustField;
            }
            set
            {
                this.xJustField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "ID")]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }
}
