namespace MultiSoftware.CTe.v400.Eventos.PrestacaoServicoDesacordo
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class evPrestDesacordo
    {

        private evPrestDesacordoDescEvento descEventoField;

        private evPrestDesacordoIndDesacordoOper indDesacordoOperField;

        private string xObsField;

        /// <remarks/>
        public evPrestDesacordoDescEvento descEvento
        {
            get
            {
                return this.descEventoField;
            }
            set
            {
                this.descEventoField = value;
            }
        }

        /// <remarks/>
        public evPrestDesacordoIndDesacordoOper indDesacordoOper
        {
            get
            {
                return this.indDesacordoOperField;
            }
            set
            {
                this.indDesacordoOperField = value;
            }
        }

        /// <remarks/>
        public string xObs
        {
            get
            {
                return this.xObsField;
            }
            set
            {
                this.xObsField = value;
            }
        }
    }
}
