namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteProcessado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class cteSimpProc
    {

        private ConhecimentoDeTransporteSimplificado.TCTeSimp cTeSimpField;

        private ConhecimentoDeTransporteSimplificado.TProtCTe protCTeField;

        private string versaoField;

        private string ipTransmissorField;

        private string nPortaConField;

        private string dhConexaoField;

        /// <remarks/>
        public ConhecimentoDeTransporteSimplificado.TCTeSimp CTeSimp
        {
            get
            {
                return this.cTeSimpField;
            }
            set
            {
                this.cTeSimpField = value;
            }
        }

        /// <remarks/>
        public ConhecimentoDeTransporteSimplificado.TProtCTe protCTe
        {
            get
            {
                return this.protCTeField;
            }
            set
            {
                this.protCTeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string versao
        {
            get
            {
                return this.versaoField;
            }
            set
            {
                this.versaoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ipTransmissor
        {
            get
            {
                return this.ipTransmissorField;
            }
            set
            {
                this.ipTransmissorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string nPortaCon
        {
            get
            {
                return this.nPortaConField;
            }
            set
            {
                this.nPortaConField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string dhConexao
        {
            get
            {
                return this.dhConexaoField;
            }
            set
            {
                this.dhConexaoField = value;
            }
        }
    }
}