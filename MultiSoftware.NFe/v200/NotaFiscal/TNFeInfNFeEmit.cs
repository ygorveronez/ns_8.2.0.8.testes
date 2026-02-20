namespace MultiSoftware.NFe.NotaFiscal
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeEmit
    {

        private string itemField;

        private ItemChoiceType2 itemElementNameField;

        private string xNomeField;

        private string xFantField;

        private TEnderEmi enderEmitField;

        private string ieField;

        private string iESTField;

        private string imField;

        private string cNAEField;

        private TNFeInfNFeEmitCRT cRTField;

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
        public ItemChoiceType2 ItemElementName
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
        public string xFant
        {
            get
            {
                return this.xFantField;
            }
            set
            {
                this.xFantField = value;
            }
        }

        /// <remarks/>
        public TEnderEmi enderEmit
        {
            get
            {
                return this.enderEmitField;
            }
            set
            {
                this.enderEmitField = value;
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
        public string IEST
        {
            get
            {
                return this.iESTField;
            }
            set
            {
                this.iESTField = value;
            }
        }

        /// <remarks/>
        public string IM
        {
            get
            {
                return this.imField;
            }
            set
            {
                this.imField = value;
            }
        }

        /// <remarks/>
        public string CNAE
        {
            get
            {
                return this.cNAEField;
            }
            set
            {
                this.cNAEField = value;
            }
        }

        /// <remarks/>
        public TNFeInfNFeEmitCRT CRT
        {
            get
            {
                return this.cRTField;
            }
            set
            {
                this.cRTField = value;
            }
        }
    }
}
