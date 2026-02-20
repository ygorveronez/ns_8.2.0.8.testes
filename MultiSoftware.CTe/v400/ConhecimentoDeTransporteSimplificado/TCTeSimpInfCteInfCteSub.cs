namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeSimpInfCteInfCteSub
    {

        private string chCteField;

        private TCTeSimpInfCteInfCteSubIndAlteraToma indAlteraTomaField;

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
        public TCTeSimpInfCteInfCteSubIndAlteraToma indAlteraToma
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