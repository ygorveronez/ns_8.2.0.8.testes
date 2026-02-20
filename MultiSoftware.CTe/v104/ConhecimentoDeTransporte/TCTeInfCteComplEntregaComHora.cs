namespace MultiSoftware.CTe.v104.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteComplEntregaComHora
    {

        private TCTeInfCteComplEntregaComHoraTpHor tpHorField;

        private string hProgField;

        /// <remarks/>
        public TCTeInfCteComplEntregaComHoraTpHor tpHor
        {
            get
            {
                return this.tpHorField;
            }
            set
            {
                this.tpHorField = value;
            }
        }

        /// <remarks/>
        public string hProg
        {
            get
            {
                return this.hProgField;
            }
            set
            {
                this.hProgField = value;
            }
        }
    }
}
