namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeSimpInfCteCompl
    {

        private string xCaracAdField;

        private string xCaracSerField;

        private TCTeSimpInfCteComplFluxo fluxoField;

        private string xObsField;

        private TCTeSimpInfCteComplObsCont[] obsContField;

        private TCTeSimpInfCteComplObsFisco[] obsFiscoField;

        /// <remarks/>
        public string xCaracAd
        {
            get
            {
                return this.xCaracAdField;
            }
            set
            {
                this.xCaracAdField = value;
            }
        }

        /// <remarks/>
        public string xCaracSer
        {
            get
            {
                return this.xCaracSerField;
            }
            set
            {
                this.xCaracSerField = value;
            }
        }

        /// <remarks/>
        public TCTeSimpInfCteComplFluxo fluxo
        {
            get
            {
                return this.fluxoField;
            }
            set
            {
                this.fluxoField = value;
            }
        }

        /// <remarks/>
        public string xObs
        {
            get
            {
                return this.xObsField;
            }
            set
            {
                this.xObsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ObsCont")]
        public TCTeSimpInfCteComplObsCont[] ObsCont
        {
            get
            {
                return this.obsContField;
            }
            set
            {
                this.obsContField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ObsFisco")]
        public TCTeSimpInfCteComplObsFisco[] ObsFisco
        {
            get
            {
                return this.obsFiscoField;
            }
            set
            {
                this.obsFiscoField = value;
            }
        }
    }
}