namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporteOS
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeOSInfCteInfCTeNorm
    {

        private TCTeOSInfCteInfCTeNormInfServico infServicoField;

        private TCTeOSInfCteInfCTeNormInfDocRef[] infDocRefField;

        private TCTeOSInfCteInfCTeNormSeg[] segField;

        private TCTeOSInfCteInfCTeNormInfModal infModalField;

        private TCTeOSInfCteInfCTeNormInfCteSub infCteSubField;

        private string refCTeCancField;

        private TCTeOSInfCteInfCTeNormCobr cobrField;

        /// <remarks/>
        public TCTeOSInfCteInfCTeNormInfServico infServico
        {
            get
            {
                return this.infServicoField;
            }
            set
            {
                this.infServicoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("infDocRef")]
        public TCTeOSInfCteInfCTeNormInfDocRef[] infDocRef
        {
            get
            {
                return this.infDocRefField;
            }
            set
            {
                this.infDocRefField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("seg")]
        public TCTeOSInfCteInfCTeNormSeg[] seg
        {
            get
            {
                return this.segField;
            }
            set
            {
                this.segField = value;
            }
        }

        /// <remarks/>
        public TCTeOSInfCteInfCTeNormInfModal infModal
        {
            get
            {
                return this.infModalField;
            }
            set
            {
                this.infModalField = value;
            }
        }

        /// <remarks/>
        public TCTeOSInfCteInfCTeNormInfCteSub infCteSub
        {
            get
            {
                return this.infCteSubField;
            }
            set
            {
                this.infCteSubField = value;
            }
        }

        /// <remarks/>
        public string refCTeCanc
        {
            get
            {
                return this.refCTeCancField;
            }
            set
            {
                this.refCTeCancField = value;
            }
        }

        /// <remarks/>
        public TCTeOSInfCteInfCTeNormCobr cobr
        {
            get
            {
                return this.cobrField;
            }
            set
            {
                this.cobrField = value;
            }
        }
    }
}
