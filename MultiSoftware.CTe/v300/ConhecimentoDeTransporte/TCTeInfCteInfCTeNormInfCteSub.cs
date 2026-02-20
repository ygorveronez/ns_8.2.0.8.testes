namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfCTeNormInfCteSub
    {

        private string chCteField;

        private object itemField;

        private TCTeInfCteInfCTeNormInfCteSubIndAlteraToma indAlteraTomaField;

        private bool indAlteraTomaFieldSpecified;

        /// <remarks/>
        public string chCte
        {
            get
            {
                return this.chCteField;
            }
            set
            {
                this.chCteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("refCteAnu", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("tomaICMS", typeof(TCTeInfCteInfCTeNormInfCteSubTomaICMS))]
        public object Item
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
        public TCTeInfCteInfCTeNormInfCteSubIndAlteraToma indAlteraToma
        {
            get
            {
                return this.indAlteraTomaField;
            }
            set
            {
                this.indAlteraTomaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool indAlteraTomaSpecified
        {
            get
            {
                return this.indAlteraTomaFieldSpecified;
            }
            set
            {
                this.indAlteraTomaFieldSpecified = value;
            }
        }
    }
}
