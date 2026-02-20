namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCte
    {

        private TCTeInfCteIde ideField;

        private TCTeInfCteCompl complField;

        private TCTeInfCteEmit emitField;

        private TCTeInfCteRem remField;

        private TCTeInfCteExped expedField;

        private TCTeInfCteReceb recebField;

        private TCTeInfCteDest destField;

        private TCTeInfCteVPrest vPrestField;

        private TCTeInfCteImp impField;

        private object[] itemsField;

        private TCTeInfCteAutXML[] autXMLField;

        private TRespTec infRespTecField;

        private TCTeInfCteInfSolicNFF infSolicNFFField;

        private TCTeInfCteInfPAA infPAAField;

        private string versaoField;

        private string idField;

        /// <remarks/>
        public TCTeInfCteIde ide
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
        public TCTeInfCteCompl compl
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
        public TCTeInfCteEmit emit
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
        public TCTeInfCteRem rem
        {
            get
            {
                return this.remField;
            }
            set
            {
                this.remField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteExped exped
        {
            get
            {
                return this.expedField;
            }
            set
            {
                this.expedField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteReceb receb
        {
            get
            {
                return this.recebField;
            }
            set
            {
                this.recebField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteDest dest
        {
            get
            {
                return this.destField;
            }
            set
            {
                this.destField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteVPrest vPrest
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
        public TCTeInfCteImp imp
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
        [System.Xml.Serialization.XmlElementAttribute("infCTeNorm", typeof(TCTeInfCteInfCTeNorm))]
        [System.Xml.Serialization.XmlElementAttribute("infCteComp", typeof(TCTeInfCteInfCteComp))]
        [System.Xml.Serialization.XmlElementAttribute("infCTeNormInfDocInfNFe", typeof(TCTeInfCteInfCTeNormInfDocInfNFe))]
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
        public TCTeInfCteAutXML[] autXML
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
        public TCTeInfCteInfSolicNFF infSolicNFF
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
        public TCTeInfCteInfPAA infPAA
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
