namespace MultiSoftware.NFe.v310.NotaFiscal
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/nfe")]
    public partial class TNFeInfNFeDetProdComb
    {

        //private TcProdANP cProdANPField;

        private string pMixGNField;

        private string cODIFField;

        private string qTempField;

        private TUf uFConsField;

        private TNFeInfNFeDetProdCombCIDE cIDEField;

        /// <remarks/>
        //public TcProdANP cProdANP
        //{
        //    get
        //    {
        //        return this.cProdANPField;
        //    }
        //    set
        //    {
        //        this.cProdANPField = value;
        //    }
        //}

        /// <remarks/>
        public string pMixGN
        {
            get
            {
                return this.pMixGNField;
            }
            set
            {
                this.pMixGNField = value;
            }
        }

        /// <remarks/>
        public string CODIF
        {
            get
            {
                return this.cODIFField;
            }
            set
            {
                this.cODIFField = value;
            }
        }

        /// <remarks/>
        public string qTemp
        {
            get
            {
                return this.qTempField;
            }
            set
            {
                this.qTempField = value;
            }
        }

        /// <remarks/>
        public TUf UFCons
        {
            get
            {
                return this.uFConsField;
            }
            set
            {
                this.uFConsField = value;
            }
        }

        /// <remarks/>
        public TNFeInfNFeDetProdCombCIDE CIDE
        {
            get
            {
                return this.cIDEField;
            }
            set
            {
                this.cIDEField = value;
            }
        }
    }
}
