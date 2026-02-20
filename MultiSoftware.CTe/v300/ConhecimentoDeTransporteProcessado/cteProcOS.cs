namespace MultiSoftware.CTe.v300.ConhecimentoDeTransporteProcessado
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.portalfiscal.inf.br/cte", IsNullable = false)]
    public partial class cteOSProc
    {

        private ConhecimentoDeTransporteOS.TCTeOS cTeOSField;

        private ConhecimentoDeTransporteOS.TProtCTeOS protCTeField;

        private string versaoField;

        private string ipTransmissorField;

        /// <remarks/>
        public ConhecimentoDeTransporteOS.TCTeOS CTeOS
        {
            get
            {
                return this.cTeOSField;
            }
            set
            {
                this.cTeOSField = value;
            }
        }

        /// <remarks/>
        public ConhecimentoDeTransporteOS.TProtCTeOS protCTe
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
    }
}
