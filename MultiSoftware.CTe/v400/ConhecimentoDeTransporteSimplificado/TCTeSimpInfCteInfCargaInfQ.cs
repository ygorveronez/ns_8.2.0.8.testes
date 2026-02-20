namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeSimpInfCteInfCargaInfQ
    {

        private TCTeSimpInfCteInfCargaInfQCUnid cUnidField;

        private TCTeSimpInfCteInfCargaInfQTpMed tpMedField;

        private string qCargaField;

        /// <remarks/>
        public TCTeSimpInfCteInfCargaInfQCUnid cUnid
        {
            get
            {
                return this.cUnidField;
            }
            set
            {
                this.cUnidField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteInfCargaInfQTpMed tpMed
        {
            get
            {
                return this.tpMedField;
            }
            set
            {
                this.tpMedField = value;
            }
        }

        /// <remarks/>
        public string qCarga
        {
            get
            {
                return this.qCargaField;
            }
            set
            {
                this.qCargaField = value;
            }
        }
    }
}