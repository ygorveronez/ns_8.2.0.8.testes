namespace MultiSoftware.CTe.v300.ModalRodoviario
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class rodoOSVeic
    {

        private string placaField;

        private string rENAVAMField;

        private rodoOSVeicProp propField;

        private ConhecimentoDeTransporte.TUf ufField;

        /// <remarks/>
        public string placa
        {
            get
            {
                return this.placaField;
            }
            set
            {
                this.placaField = value;
            }
        }

        /// <remarks/>
        public string RENAVAM
        {
            get
            {
                return this.rENAVAMField;
            }
            set
            {
                this.rENAVAMField = value;
            }
        }

        /// <remarks/>
        public rodoOSVeicProp prop
        {
            get
            {
                return this.propField;
            }
            set
            {
                this.propField = value;
            }
        }

        /// <remarks/>
        public ConhecimentoDeTransporte.TUf UF
        {
            get
            {
                return this.ufField;
            }
            set
            {
                this.ufField = value;
            }
        }
    }
}
