namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeSimpInfCteCobr
    {

        private TCTeSimpInfCteCobrFat fatField;

        private TCTeSimpInfCteCobrDup[] dupField;

        /// <remarks/>
        public TCTeSimpInfCteCobrFat fat
        {
            get
            {
                return this.fatField;
            }
            set
            {
                this.fatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("dup")]
        public TCTeSimpInfCteCobrDup[] dup
        {
            get
            {
                return this.dupField;
            }
            set
            {
                this.dupField = value;
            }
        }
    }
}