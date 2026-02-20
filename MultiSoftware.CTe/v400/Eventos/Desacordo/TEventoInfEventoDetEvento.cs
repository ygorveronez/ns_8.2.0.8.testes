namespace MultiSoftware.CTe.v400.Eventos.Desacordo
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TEventoInfEventoDetEvento
    {

        private MultiSoftware.CTe.v400.Eventos.PrestacaoServicoDesacordo.evPrestDesacordo evPrestDesacordoField;

        private string versaoEventoField;

        /// <remarks/>
        public MultiSoftware.CTe.v400.Eventos.PrestacaoServicoDesacordo.evPrestDesacordo evPrestDesacordo
        {
            get
            {
                return this.evPrestDesacordoField;
            }
            set
            {
                this.evPrestDesacordoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string versaoEvento
        {
            get
            {
                return this.versaoEventoField;
            }
            set
            {
                this.versaoEventoField = value;
            }
        }
    }
}
