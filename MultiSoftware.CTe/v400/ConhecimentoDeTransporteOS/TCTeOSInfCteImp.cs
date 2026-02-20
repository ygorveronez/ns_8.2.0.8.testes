namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteImp
    {

        private TImpOS iCMSField;

        private string vTotTribField;

        private string infAdFiscoField;

        private TCTeOSInfCteImpICMSUFFim iCMSUFFimField;

        private TCTeOSInfCteImpInfTribFed infTribFedField;

        private TTribCTe iBSCBSField;

        private string vTotDFeField;

        /// <remarks/>
        public TImpOS ICMS
        {
            get
            {
                return this.iCMSField;
            }
            set
            {
                this.iCMSField = value;
            }
        }

        /// <remarks/>
        public string vTotTrib
        {
            get
            {
                return this.vTotTribField;
            }
            set
            {
                this.vTotTribField = value;
            }
        }

        /// <remarks/>
        public string infAdFisco
        {
            get
            {
                return this.infAdFiscoField;
            }
            set
            {
                this.infAdFiscoField = value;
            }
        }

        /// <remarks/>
        public TCTeOSInfCteImpICMSUFFim ICMSUFFim
        {
            get
            {
                return this.iCMSUFFimField;
            }
            set
            {
                this.iCMSUFFimField = value;
            }
        }

        /// <remarks/>
        public TCTeOSInfCteImpInfTribFed infTribFed
        {
            get
            {
                return this.infTribFedField;
            }
            set
            {
                this.infTribFedField = value;
            }
        }

        /// <remarks/>
        public TTribCTe IBSCBS
        {
            get
            {
                return this.iBSCBSField;
            }
            set
            {
                this.iBSCBSField = value;
            }
        }

        /// <remarks/>
        public string vTotDFe
        {
            get
            {
                return this.vTotDFeField;
            }
            set
            {
                this.vTotDFeField = value;
            }
        }
    }
}
