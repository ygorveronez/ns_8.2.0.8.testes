namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeSimpInfCte
    {

        private TCTeSimpInfCteIde ideField;

        private TCTeSimpInfCteCompl complField;

        private TCTeSimpInfCteEmit emitField;

        private TCTeSimpInfCteToma tomaField;

        private TCTeSimpInfCteInfCarga infCargaField;

        private TCTeSimpInfCteDet[] detField;

        private TCTeSimpInfCteInfModal infModalField;

        private TCTeSimpInfCteCobr cobrField;

        private TCTeSimpInfCteInfCteSub infCteSubField;

        private TCTeSimpInfCteImp impField;

        private TCTeSimpInfCteTotal totalField;

        private TCTeSimpInfCteAutXML[] autXMLField;

        private TRespTec infRespTecField;

        private TCTeSimpInfCteInfSolicNFF infSolicNFFField;

        private TCTeSimpInfCteInfPAA infPAAField;

        private string versaoField;

        private string idField;

        /// <remarks/>
        public TCTeSimpInfCteIde ide
        {
            get
            {
                return this.ideField;
            }
            set
            {
                this.ideField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteCompl compl
        {
            get
            {
                return this.complField;
            }
            set
            {
                this.complField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteEmit emit
        {
            get
            {
                return this.emitField;
            }
            set
            {
                this.emitField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteToma toma
        {
            get
            {
                return this.tomaField;
            }
            set
            {
                this.tomaField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteInfCarga infCarga
        {
            get
            {
                return this.infCargaField;
            }
            set
            {
                this.infCargaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("det")]
        public TCTeSimpInfCteDet[] det
        {
            get
            {
                return this.detField;
            }
            set
            {
                this.detField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteInfModal infModal
        {
            get
            {
                return this.infModalField;
            }
            set
            {
                this.infModalField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteCobr cobr
        {
            get
            {
                return this.cobrField;
            }
            set
            {
                this.cobrField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteInfCteSub infCteSub
        {
            get
            {
                return this.infCteSubField;
            }
            set
            {
                this.infCteSubField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteImp imp
        {
            get
            {
                return this.impField;
            }
            set
            {
                this.impField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteTotal total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("autXML")]
        public TCTeSimpInfCteAutXML[] autXML
        {
            get
            {
                return this.autXMLField;
            }
            set
            {
                this.autXMLField = value;
            }
        }

        /// <remarks/>
        public TRespTec infRespTec
        {
            get
            {
                return this.infRespTecField;
            }
            set
            {
                this.infRespTecField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteInfSolicNFF infSolicNFF
        {
            get
            {
                return this.infSolicNFFField;
            }
            set
            {
                this.infSolicNFFField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteInfPAA infPAA
        {
            get
            {
                return this.infPAAField;
            }
            set
            {
                this.infPAAField = value;
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