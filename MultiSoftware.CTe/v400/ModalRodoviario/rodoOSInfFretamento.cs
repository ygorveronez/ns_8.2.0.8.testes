namespace MultiSoftware.CTe.v400.ModalRodoviario
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class rodoOSInfFretamento
    {

        private rodoOSInfFretamentoTpFretamento tpFretamentoField;

        private string dhViagemField;

        /// <remarks/>
        public rodoOSInfFretamentoTpFretamento tpFretamento
        {
            get
            {
                return this.tpFretamentoField;
            }
            set
            {
                this.tpFretamentoField = value;
            }
        }

        /// <remarks/>
        public string dhViagem
        {
            get
            {
                return this.dhViagemField;
            }
            set
            {
                this.dhViagemField = value;
            }
        }
    }
}
