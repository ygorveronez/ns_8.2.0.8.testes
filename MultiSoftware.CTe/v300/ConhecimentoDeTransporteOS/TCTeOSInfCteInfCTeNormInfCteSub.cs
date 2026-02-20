namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteInfCTeNormInfCteSub
    {

        private string chCteField;

        private object itemField;

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
        [System.Xml.Serialization.XmlElementAttribute("tomaICMS", typeof(TCTeOSInfCteInfCTeNormInfCteSubTomaICMS))]
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
    }
}
