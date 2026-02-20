namespace MultiSoftware.CTe.v400.ModalAereo
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.portalfiscal.inf.br/cte")]
    public partial class aereoPeri
    {

        private string nONUField;

        private string qTotEmbField;

        private aereoPeriInfTotAP infTotAPField;

        /// <remarks/>
        public string nONU
        {
            get
            {
                return this.nONUField;
            }
            set
            {
                this.nONUField = value;
            }
        }

        /// <remarks/>
        public string qTotEmb
        {
            get
            {
                return this.qTotEmbField;
            }
            set
            {
                this.qTotEmbField = value;
            }
        }

        /// <remarks/>
        public aereoPeriInfTotAP infTotAP
        {
            get
            {
                return this.infTotAPField;
            }
            set
            {
                this.infTotAPField = value;
            }
        }
    }
}
