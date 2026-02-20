namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporte
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCTeInfCteInfCTeNorm
    {

        private TCTeInfCteInfCTeNormInfCarga infCargaField;

        private TCTeInfCteInfCTeNormInfDoc infDocField;

        private TCTeInfCteInfCTeNormEmiDocAnt[] docAntField;

        private TCTeInfCteInfCTeNormInfModal infModalField;

        private TCTeInfCteInfCTeNormVeicNovos[] veicNovosField;

        private TCTeInfCteInfCTeNormCobr cobrField;

        private TCTeInfCteInfCTeNormInfCteSub infCteSubField;

        private TCTeInfCteInfCTeNormInfGlobalizado infGlobalizadoField;

        private TCTeInfCteInfCTeNormInfCTeMultimodal[] infServVincField;

        /// <remarks/>
        public TCTeInfCteInfCTeNormInfCarga infCarga
        {
            get
            {
                return this.infCargaField;
            }
            set
            {
                this.infCargaField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteInfCTeNormInfDoc infDoc
        {
            get
            {
                return this.infDocField;
            }
            set
            {
                this.infDocField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("emiDocAnt", IsNullable = false)]
        public TCTeInfCteInfCTeNormEmiDocAnt[] docAnt
        {
            get
            {
                return this.docAntField;
            }
            set
            {
                this.docAntField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteInfCTeNormInfModal infModal
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
        [System.Xml.Serialization.XmlElementAttribute("veicNovos")]
        public TCTeInfCteInfCTeNormVeicNovos[] veicNovos
        {
            get
            {
                return this.veicNovosField;
            }
            set
            {
                this.veicNovosField = value;
            }
        }

        /// <remarks/>
        public TCTeInfCteInfCTeNormCobr cobr
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

        /// <remarks/>
        public TCTeInfCteInfCTeNormInfCteSub infCteSub
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
        public TCTeInfCteInfCTeNormInfGlobalizado infGlobalizado
        {
            get
            {
                return this.infGlobalizadoField;
            }
            set
            {
                this.infGlobalizadoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("infCTeMultimodal", IsNullable = false)]
        public TCTeInfCteInfCTeNormInfCTeMultimodal[] infServVinc
        {
            get
            {
                return this.infServVincField;
            }
            set
            {
                this.infServVincField = value;
            }
        }
    }
}
