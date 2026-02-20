namespace MultiSoftware.CTe.v400.ConhecimentoDeTransporteSimplificado
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TCompraGov
    {

        private TEnteGov tpEnteGovField;

        private string pRedutorField;

        /// <remarks/>
        public TEnteGov tpEnteGov
        {
            get
            {
                return this.tpEnteGovField;
            }
            set
            {
                this.tpEnteGovField = value;
            }
        }

        /// <remarks/>
        public string pRedutor
        {
            get
            {
                return this.pRedutorField;
            }
            set
            {
                this.pRedutorField = value;
            }
        }
    }
}