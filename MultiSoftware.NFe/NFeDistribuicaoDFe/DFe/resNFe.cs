namespace MultiSoftware.NFe.NFeDistribuicaoDFe.DFe
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/nfe", IsNullable = false)]
    public partial class resNFe
    {

        private string chNFeField;

        private string itemField;

        private ItemChoiceType itemElementNameField;

        private string xNomeField;

        private string ieField;

        private string dhEmiField;

        private resNFeTpNF tpNFField;

        private string vNFField;

        private byte[] digValField;

        private string dhRecbtoField;

        private string nProtField;

        private resNFeCSitNFe cSitNFeField;

        private TVerResNFe versaoField;

        /// <remarks/>
        public string chNFe
        {
            get
            {
                return this.chNFeField;
            }
            set
            {
                this.chNFeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("CNPJ", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("CPF", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemElementName")]
        public string Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemChoiceType ItemElementName
        {
            get
            {
                return this.itemElementNameField;
            }
            set
            {
                this.itemElementNameField = value;
            }
        }

        /// <remarks/>
        public string xNome
        {
            get
            {
                return this.xNomeField;
            }
            set
            {
                this.xNomeField = value;
            }
        }

        /// <remarks/>
        public string IE
        {
            get
            {
                return this.ieField;
            }
            set
            {
                this.ieField = value;
            }
        }

        /// <remarks/>
        public string dhEmi
        {
            get
            {
                return this.dhEmiField;
            }
            set
            {
                this.dhEmiField = value;
            }
        }

        /// <remarks/>
        public resNFeTpNF tpNF
        {
            get
            {
                return this.tpNFField;
            }
            set
            {
                this.tpNFField = value;
            }
        }

        /// <remarks/>
        public string vNF
        {
            get
            {
                return this.vNFField;
            }
            set
            {
                this.vNFField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] digVal
        {
            get
            {
                return this.digValField;
            }
            set
            {
                this.digValField = value;
            }
        }

        /// <remarks/>
        public string dhRecbto
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
        public resNFeCSitNFe cSitNFe
        {
            get
            {
                return this.cSitNFeField;
            }
            set
            {
                this.cSitNFeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TVerResNFe versao
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
