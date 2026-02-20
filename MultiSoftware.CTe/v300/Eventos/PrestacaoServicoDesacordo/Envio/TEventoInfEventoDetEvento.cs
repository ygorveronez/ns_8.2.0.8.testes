namespace MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Envio
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TEventoInfEventoDetEvento
    {

        private Envio.evPrestDesacordo evPrestDesacordoField;

        private string versaoEventoField;

        /// <remarks/>
        public Envio.evPrestDesacordo evPrestDesacordo
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
