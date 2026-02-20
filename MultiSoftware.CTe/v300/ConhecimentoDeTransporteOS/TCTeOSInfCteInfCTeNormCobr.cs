namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteInfCTeNormCobr
    {

        private TCTeOSInfCteInfCTeNormCobrFat fatField;

        private TCTeOSInfCteInfCTeNormCobrDup[] dupField;

        /// <remarks/>
        public TCTeOSInfCteInfCTeNormCobrFat fat
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
        public TCTeOSInfCteInfCTeNormCobrDup[] dup
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
