namespace MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Envio
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TProcEvento
    {

        private MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Envio.TEvento eventoCTeField;

        private MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Retorno.TRetEvento retEventoCTeField;

        private string versaoField;

        private string ipTransmissorField;

        /// <remarks/>
        public TEvento eventoCTe
        {
            get
            {
                return this.eventoCTeField;
            }
            set
            {
                this.eventoCTeField = value;
            }
        }

        /// <remarks/>
        public MultiSoftware.CTe.v300.Eventos.PrestacaoServicoDesacordo.Retorno.TRetEvento retEventoCTe
        {
            get
            {
                return this.retEventoCTeField;
            }
            set
            {
                this.retEventoCTeField = value;
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
    }
}
