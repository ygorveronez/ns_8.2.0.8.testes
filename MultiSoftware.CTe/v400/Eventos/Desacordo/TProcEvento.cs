namespace MultiSoftware.CTe.v400.Eventos.Desacordo
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class TProcEvento
    {

        private TEvento eventoCTeField;

        private TRetEvento retEventoCTeField;

        private string versaoField;

        private string ipTransmissorField;

        private string nPortaConField;

        private string dhConexaoField;

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
        public TRetEvento retEventoCTe
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
