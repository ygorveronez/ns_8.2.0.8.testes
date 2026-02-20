namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeSimpInfCteDet
    {

        private string cMunIniField;

        private string xMunIniField;

        private string cMunFimField;

        private string xMunFimField;

        private string vPrestField;

        private string vRecField;

        private TCTeSimpInfCteDetComp[] compField;

        private object[] itemsField;

        private string nItemField;

        /// <remarks/>
        public string cMunIni
        {
            get
            {
                return this.cMunIniField;
            }
            set
            {
                this.cMunIniField = value;
            }
        }

        /// <remarks/>
        public string xMunIni
        {
            get
            {
                return this.xMunIniField;
            }
            set
            {
                this.xMunIniField = value;
            }
        }

        /// <remarks/>
        public string cMunFim
        {
            get
            {
                return this.cMunFimField;
            }
            set
            {
                this.cMunFimField = value;
            }
        }

        /// <remarks/>
        public string xMunFim
        {
            get
            {
                return this.xMunFimField;
            }
            set
            {
                this.xMunFimField = value;
            }
        }

        /// <remarks/>
        public string vPrest
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
        public string vRec
        {
            get
            {
                return this.vRecField;
            }
            set
            {
                this.vRecField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Comp")]
        public TCTeSimpInfCteDetComp[] Comp
        {
            get
            {
                return this.compField;
            }
            set
            {
                this.compField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("infDocAnt", typeof(TCTeSimpInfCteDetInfDocAnt))]
        [System.Xml.Serialization.XmlElementAttribute("infNFe", typeof(TCTeSimpInfCteDetInfNFe))]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nItem
        {
            get
            {
                return this.nItemField;
            }
            set
            {
                this.nItemField = value;
            }
        }
    }
}