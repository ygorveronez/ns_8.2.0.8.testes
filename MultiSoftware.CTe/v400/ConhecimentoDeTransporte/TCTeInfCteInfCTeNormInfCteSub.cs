namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporte
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfCTeNormInfCteSub
    {

        private string chCteField;

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
