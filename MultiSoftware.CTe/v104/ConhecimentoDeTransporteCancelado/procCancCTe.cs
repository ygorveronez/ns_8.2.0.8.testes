namespace MultiSoftware.CTe.v104.ConhecimentoDeTransporteCancelado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute("procCancCTe", Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class TProcCancCTe
    {

        private TCancCTe cancCTeField;

        private TRetCancCTe retCancCTeField;

        private string versaoField;

        /// <remarks/>
        public TCancCTe cancCTe
        {
            get
            {
                return this.cancCTeField;
            }
            set
            {
                this.cancCTeField = value;
            }
        }

        /// <remarks/>
        public TRetCancCTe retCancCTe
        {
            get
            {
                return this.retCancCTeField;
            }
            set
            {
                this.retCancCTeField = value;
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
