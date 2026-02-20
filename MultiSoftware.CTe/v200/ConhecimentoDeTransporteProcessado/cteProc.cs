namespace MultiSoftware.CTe.v200.ConhecimentoDeTransporteProcessado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class cteProc
    {

        private ConhecimentoDeTransporte.TCTe cTeField;

        private ConhecimentoDeTransporte.TProtCTe protCTeField;

        private string versaoField;

        /// <remarks/>
        public ConhecimentoDeTransporte.TCTe CTe
        {
            get
            {
                return this.cTeField;
            }
            set
            {
                this.cTeField = value;
            }
        }

        /// <remarks/>
        public ConhecimentoDeTransporte.TProtCTe protCTe
        {
            get
            {
                return this.protCTeField;
            }
            set
            {
                this.protCTeField = value;
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
    }
}
