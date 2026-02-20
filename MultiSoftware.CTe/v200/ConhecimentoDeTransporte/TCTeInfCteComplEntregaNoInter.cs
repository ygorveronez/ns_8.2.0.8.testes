namespace MultiSoftware.CTe.v200.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteComplEntregaNoInter
    {

        private TCTeInfCteComplEntregaNoInterTpHor tpHorField;

        private string hIniField;

        private string hFimField;

        /// <remarks/>
        public TCTeInfCteComplEntregaNoInterTpHor tpHor
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
        public string hIni
        {
            get
            {
                return this.hIniField;
            }
            set
            {
                this.hIniField = value;
            }
        }

        /// <remarks/>
        public string hFim
        {
            get
            {
                return this.hFimField;
            }
            set
            {
                this.hFimField = value;
            }
        }
    }
}
