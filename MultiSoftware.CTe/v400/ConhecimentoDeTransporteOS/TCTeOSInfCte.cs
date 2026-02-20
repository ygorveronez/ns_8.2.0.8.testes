namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteOS
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCte
    {

        private TCTeOSInfCteIde ideField;

        private TCTeOSInfCteCompl complField;

        private TCTeOSInfCteEmit emitField;

        private TCTeOSInfCteToma tomaField;

        private TCTeOSInfCteVPrest vPrestField;

        private TCTeOSInfCteImp impField;

        private object[] itemsField;

        private TCTeOSInfCteAutXML[] autXMLField;

        private TRespTec infRespTecField;

        private string versaoField;

        private string idField;

        /// <remarks/>
        public TCTeOSInfCteIde ide
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
        public TCTeOSInfCteCompl compl
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
        public TCTeOSInfCteEmit emit
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
        public TCTeOSInfCteToma toma
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
        public TCTeOSInfCteVPrest vPrest
        {
            get
            {
                return this.vPrestField;
            }
            set
            {
                this.vPrestField = value;
            }
        }

        /// <remarks/>
        public TCTeOSInfCteImp imp
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
        [System.Xml.Serialization.XmlElementAttribute("infCTeNorm", typeof(TCTeOSInfCteInfCTeNorm))]
        [System.Xml.Serialization.XmlElementAttribute("infCteComp", typeof(TCTeOSInfCteInfCteComp))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("autXML")]
        public TCTeOSInfCteAutXML[] autXML
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
