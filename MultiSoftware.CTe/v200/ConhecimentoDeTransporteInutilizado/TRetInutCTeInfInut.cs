namespace MultiSoftware.CTe.v200.ConhecimentoDeTransporteInutilizado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TRetInutCTeInfInut
    {

        private ConhecimentoDeTransporte.TAmb tpAmbField;

        private string verAplicField;

        private string cStatField;

        private string xMotivoField;

        private ConhecimentoDeTransporte.TCodUfIBGE cUFField;

        private short anoField;

        private bool anoFieldSpecified;

        private string cNPJField;

        private ConhecimentoDeTransporte.TModCT modField;

        private bool modFieldSpecified;

        private string serieField;

        private string nCTIniField;

        private string nCTFinField;

        private System.DateTime dhRecbtoField;

        private bool dhRecbtoFieldSpecified;

        private string nProtField;

        private string idField;

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
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool anoSpecified
        {
            get
            {
                return this.anoFieldSpecified;
            }
            set
            {
                this.anoFieldSpecified = value;
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
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool modSpecified
        {
            get
            {
                return this.modFieldSpecified;
            }
            set
            {
                this.modFieldSpecified = value;
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
        public System.DateTime dhRecbto
        {
            get
            {
                return this.dhRecbtoField;
            }
            set
            {
                this.dhRecbtoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool dhRecbtoSpecified
        {
            get
            {
                return this.dhRecbtoFieldSpecified;
            }
            set
            {
                this.dhRecbtoFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string nProt
        {
            get
            {
                return this.nProtField;
            }
            set
            {
                this.nProtField = value;
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
