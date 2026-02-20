namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteCompl
    {

        private string xCaracAdField;

        private string xCaracSerField;

        private string xEmiField;

        private string xObsField;

        private TCTeOSInfCteComplObsCont[] obsContField;

        private TCTeOSInfCteComplObsFisco[] obsFiscoField;

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
        public string xEmi
        {
            get
            {
                return this.xEmiField;
            }
            set
            {
                this.xEmiField = value;
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
        public TCTeOSInfCteComplObsCont[] ObsCont
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
        public TCTeOSInfCteComplObsFisco[] ObsFisco
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
