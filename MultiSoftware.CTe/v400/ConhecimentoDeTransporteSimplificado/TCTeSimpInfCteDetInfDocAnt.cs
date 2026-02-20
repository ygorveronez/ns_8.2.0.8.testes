namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeSimpInfCteDetInfDocAnt
    {

        private string chCTeField;

        private TCTeSimpInfCteDetInfDocAntTpPrest tpPrestField;

        private TCTeSimpInfCteDetInfDocAntInfNFeTranspParcial[] infNFeTranspParcialField;

        /// <remarks/>
        public string chCTe
        {
            get
            {
                return this.chCTeField;
            }
            set
            {
                this.chCTeField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteDetInfDocAntTpPrest tpPrest
        {
            get
            {
                return this.tpPrestField;
            }
            set
            {
                this.tpPrestField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("infNFeTranspParcial")]
        public TCTeSimpInfCteDetInfDocAntInfNFeTranspParcial[] infNFeTranspParcial
        {
            get
            {
                return this.infNFeTranspParcialField;
            }
            set
            {
                this.infNFeTranspParcialField = value;
            }
        }
    }
}