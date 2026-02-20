namespace MultiSoftware.CTe.v104.ConhecimentoDeTransporte
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfCTeNormContQt
    {

        private string nContField;

        private TCTeInfCteInfCTeNormContQtLacContQt[] lacContQtField;

        private string dPrevField;

        /// <remarks/>
        public string nCont
        {
            get
            {
                return this.nContField;
            }
            set
            {
                this.nContField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("lacContQt")]
        public TCTeInfCteInfCTeNormContQtLacContQt[] lacContQt
        {
            get
            {
                return this.lacContQtField;
            }
            set
            {
                this.lacContQtField = value;
            }
        }

        /// <remarks/>
        public string dPrev
        {
            get
            {
                return this.dPrevField;
            }
            set
            {
                this.dPrevField = value;
            }
        }
    }
}
