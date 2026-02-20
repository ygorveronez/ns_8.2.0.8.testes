namespace MultiSoftware.CTe.v200.ConhecimentoDeTransporteInutilizado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute("procInutCTe", Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class TProcInutCTe
    {

        private TInutCTe inutCTeField;

        private TRetInutCTe retInutCTeField;

        private string versaoField;

        /// <remarks/>
        public TInutCTe inutCTe
        {
            get
            {
                return this.inutCTeField;
            }
            set
            {
                this.inutCTeField = value;
            }
        }

        /// <remarks/>
        public TRetInutCTe retInutCTe
        {
            get
            {
                return this.retInutCTeField;
            }
            set
            {
                this.retInutCTeField = value;
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
